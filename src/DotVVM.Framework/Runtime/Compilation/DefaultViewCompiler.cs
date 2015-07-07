using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#if DNXCORE50
using Microsoft.CSharp.RuntimeBinder;
#endif
using DotVVM.Framework.Binding;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Controls.Infrastructure;
using DotVVM.Framework.Parser;
using DotVVM.Framework.Parser.Dothtml.Parser;
using DotVVM.Framework.Parser.Dothtml.Tokenizer;
using DotVVM.Framework.Utils;

namespace DotVVM.Framework.Runtime.Compilation
{
    public class DefaultViewCompiler : IViewCompiler
    {
        private object locker = new object();

        public DefaultViewCompiler(DotvvmConfiguration configuration)
        {
            this.configuration = configuration;
            this.controlResolver = configuration.ServiceProvider.GetService<IControlResolver>();
            this.assemblyMetadataCache = configuration.ServiceProvider.GetService<IAssemblyMetadataCache>();
        }

        private readonly IAssemblyMetadataCache assemblyMetadataCache;
        private readonly IControlResolver controlResolver;
        private readonly DotvvmConfiguration configuration;

        private DefaultViewCompilerCodeEmitter emitter;
        private int currentTemplateIndex = 0;

        /// <summary>
        /// Compiles the view and returns a function that can be invoked repeatedly. The function builds full control tree and activates the page.
        /// </summary>
        public IControlBuilder CompileView(IReader reader, string fileName, string assemblyName, string namespaceName, string className)
        {
            lock (locker)
            {
                emitter = new DefaultViewCompilerCodeEmitter();

                // parse the document
                var tokenizer = new DothtmlTokenizer();
                tokenizer.Tokenize(reader);
                var parser = new DothtmlParser();
                var node = parser.Parse(tokenizer.Tokens);

                // determine wrapper type
                string wrapperClassName;
                var wrapperType = ResolveWrapperType(node, className, out wrapperClassName);
                var metadata = controlResolver.ResolveControl(new ControlType(wrapperType, virtualPath: fileName));

                // build the statements
                emitter.PushNewMethod(DefaultViewCompilerCodeEmitter.BuildControlFunctionName);
                var pageName = wrapperClassName == null ? emitter.EmitCreateObject(wrapperType) : emitter.EmitCreateObject(wrapperClassName);
                emitter.EmitSetAttachedProperty(pageName, typeof(Internal).FullName, Internal.UniqueIDProperty.Name, pageName);
                foreach (var child in node.Content)
                {
                    ProcessNode(child, pageName, metadata);
                }

                var directivesToApply = node.Directives.Where(d => d.Name != Constants.BaseTypeDirective).ToList();
                if (wrapperType.IsAssignableFrom(typeof(DotvvmView)))
                {
                    foreach (var directive in directivesToApply)
                    {
                        emitter.EmitAddDirective(pageName, directive.Name, directive.Value);
                    }
                }
                emitter.EmitReturnClause(pageName);
                emitter.PopMethod();

                // create the assembly
                var assembly = BuildAssembly(assemblyName, namespaceName, className);
                var viewType = assembly.GetType(namespaceName + "." + className);
                var controlBuilder = (IControlBuilder)Activator.CreateInstance(viewType);
                metadata.ControlBuilderType = controlBuilder.GetType();
                return controlBuilder;
            }
        }

        /// <summary>
        /// Resolves the type of the wrapper.
        /// </summary>
        private Type ResolveWrapperType(DothtmlRootNode node, string className, out string controlClassName)
        {
            var wrapperType = typeof(DotvvmView);

            var baseControlDirective = node.Directives.SingleOrDefault(d => d.Name == Constants.BaseTypeDirective);
            if (baseControlDirective != null)
            {
                wrapperType = Type.GetType(baseControlDirective.Value);
                if (wrapperType == null)
                {
                    throw new Exception(string.Format(Resources.Controls.ViewCompiler_TypeSpecifiedInBaseTypeDirectiveNotFound, baseControlDirective.Value));
                }
                if (!typeof(DotvvmMarkupControl).IsAssignableFrom(wrapperType))
                {
                    throw new Exception(string.Format(Resources.Controls.ViewCompiler_MarkupControlMustDeriveFromDotvvmMarkupControl));
                }

                controlClassName = null;
            }
            else
            {
                controlClassName = className + "Control";
                emitter.EmitControlClass(wrapperType, className);
            }

            return wrapperType;
        }

        /// <summary>
        /// Builds the assembly.
        /// </summary>
        private Assembly BuildAssembly(string assemblyName, string namespaceName, string className)
        {
            // static references
            var staticReferences = new[]
            {
                typeof (object).GetTypeInfo().Assembly,
#if DOTNETCORE50
                typeof(RuntimeBinderException).GetTypeInfo().Assembly,
#endif
                typeof (System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly,
                typeof (DotvvmConfiguration).GetTypeInfo().Assembly
            }
                .Concat(configuration.Markup.Assemblies.Select(a => Assembly.Load(new AssemblyName(a))))
                .Distinct();

            // add dynamic references
            var dynamicReferences = emitter.UsedControlBuilderTypes
                .Select(t => t.GetTypeInfo().Assembly)
                .Concat(emitter.UsedAssemblies)
                .Distinct();

            // compile
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            
            var compilation = CSharpCompilation.Create(
                assemblyName,
                emitter.BuildTree(namespaceName, className),
                Enumerable.Concat(staticReferences, dynamicReferences).Select(a => assemblyMetadataCache.GetAssemblyMetadata(a)),
                options);
             
            using (var assemblyStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var result = compilation.Emit(assemblyStream, pdbStream);
                if (result.Success)
                {
                    assemblyStream.Position = 0;
                    pdbStream.Position = 0;

                    var assembly = configuration.AssemblyHelper.LoadAssembly(assemblyStream, pdbStream);
                    assemblyMetadataCache.AddAssembly(assembly, compilation.ToMetadataReference());
                    return assembly;
                }
                else
                {
                    throw new Exception("The compilation failed! This is most probably bug in the DotVVM framework.\r\n\r\n"
                        + string.Join("\r\n", result.Diagnostics)
                        + "\r\n\r\n" + compilation.SyntaxTrees[0] + "\r\n\r\n");
                }
            }
        }



        /// <summary>
        /// Processes the node.
        /// </summary>
        private void ProcessNode(DothtmlNode node, string parentName, ControlResolverMetadata parentMetadata)
        {
            if (node is DothtmlBindingNode)
            {
                EnsureContentAllowed(parentMetadata);

                // binding in text
                var binding = (DothtmlBindingNode)node;
                var currentObjectName = emitter.EmitCreateObject(typeof(Literal), new object[] { ((DothtmlLiteralNode)node).Value, true });
                var bindingObjectName = emitter.EmitCreateObject(controlResolver.ResolveBinding(binding.Name), new object[] { binding.Value });
                emitter.EmitSetBinding(currentObjectName, Literal.TextProperty.DescriptorFullName, bindingObjectName);
                emitter.EmitAddCollectionItem(parentName, currentObjectName);
            }
            else if (node is DothtmlLiteralNode)
            {
                // text content
                var literalValue = ((DothtmlLiteralNode)node).Value;
                if (!string.IsNullOrWhiteSpace(literalValue))
                {
                    EnsureContentAllowed(parentMetadata);
                }
                var currentObjectName = emitter.EmitCreateObject(typeof(Literal), new object[] { literalValue });
                emitter.EmitAddCollectionItem(parentName, currentObjectName);
            }
            else if (node is DothtmlElementNode)
            {
                // HTML element
                var element = (DothtmlElementNode)node;
                var parentProperty = FindProperty(parentMetadata, element.TagName);
                if (parentProperty != null && string.IsNullOrEmpty(element.TagPrefix) && parentProperty.MarkupOptions.MappingMode == MappingMode.InnerElement)
                {
                    ProcessElementProperty(parentName, parentProperty, element);
                }
                else
                {
                    EnsureContentAllowed(parentMetadata);

                    // the element is the content
                    var currentObjectName = ProcessObjectElement(element);
                    emitter.EmitAddCollectionItem(parentName, currentObjectName);
                }
            }
            else
            {
                throw new NotSupportedException();      // TODO: exception handling
            }
        }

        private void EnsureContentAllowed(ControlResolverMetadata controlMetadata)
        {
            if (!controlMetadata.IsContentAllowed)
            {
                throw new Exception(string.Format("The content is not allowed inside the <{0}></{0}> control!", controlMetadata.Name));
            }
        }

        /// <summary>
        /// Processes the element which contains property value.
        /// </summary>
        private void ProcessElementProperty(string parentName, DotvvmProperty parentProperty, DothtmlElementNode element)
        {
            // the element is a property 
            if (IsTemplateProperty(parentProperty))
            {
                // template
                var templateName = ProcessTemplate(element);
                emitter.EmitSetValue(parentName, parentProperty.DescriptorFullName, templateName);
            }
            else if (IsCollectionProperty(parentProperty))
            {
                // collection of elements
                foreach (var child in GetInnerPropertyElements(element, parentProperty))
                {
                    var childObject = ProcessObjectElement(child);
                    emitter.EmitAddCollectionItem(parentName, childObject, parentProperty.Name);
                }
            }
            else
            {
                // new object
                var children = GetInnerPropertyElements(element, parentProperty).ToList();
                if (children.Count > 1)
                {
                    throw new NotSupportedException(string.Format("The property {0} can have only one child element!", parentProperty.MarkupOptions.Name)); // TODO: exception handling
                }
                else if (children.Count == 1)
                {
                    var childObject = ProcessObjectElement(children[0]);
                    emitter.EmitSetValue(parentName, parentProperty.DescriptorFullName, childObject);
                }
                else
                {
                    emitter.EmitSetValue(parentName, parentProperty.DescriptorFullName, emitter.EmitIdentifier("null"));
                }
            }
        }

        private DotvvmProperty FindProperty(ControlResolverMetadata parentMetadata, string name)
        {
            return parentMetadata.FindProperty(name) ?? DotvvmProperty.ResolveProperty(name);
        }

        /// <summary>
        /// Gets the inner property elements and makes sure that no other content is present.
        /// </summary>
        private IEnumerable<DothtmlElementNode> GetInnerPropertyElements(DothtmlElementNode element, DotvvmProperty parentProperty)
        {
            foreach (var child in element.Content)
            {
                if (child is DothtmlElementNode)
                {
                    yield return (DothtmlElementNode)child;
                }
                else if (child is DothtmlLiteralNode && string.IsNullOrWhiteSpace(((DothtmlLiteralNode)child).Value))
                {
                    continue;
                }
                else
                {
                    throw new NotSupportedException("Content cannot be inside collection inner property!"); // TODO: exception handling
                }
            }
        }


        /// <summary>
        /// Processes the template.
        /// </summary>
        private string ProcessTemplate(DothtmlElementNode element)
        {
            var templateName = emitter.EmitCreateObject(typeof(DelegateTemplate));
            emitter.EmitSetProperty(
                templateName,
                ReflectionUtils.GetPropertyNameFromExpression<DelegateTemplate>(t => t.BuildContentBody),
                emitter.EmitIdentifier(CompileTemplate(element)));
            return templateName;
        }

        /// <summary>
        /// Compiles the template.
        /// </summary>
        private string CompileTemplate(DothtmlElementNode element)
        {
            var methodName = DefaultViewCompilerCodeEmitter.BuildTemplateFunctionName + currentTemplateIndex;
            currentTemplateIndex++;
            emitter.PushNewMethod(methodName);

            // build the statements
            var wrapperType = typeof(Placeholder);
            var parentName = emitter.EmitCreateObject(wrapperType);
            foreach (var child in element.Content)
            {
                object[] activationParameters;
                ProcessNode(child, parentName, controlResolver.ResolveControl(DotvvmConfiguration.DotvvmControlTagPrefix, typeof(Placeholder).Name, out activationParameters));
            }
            emitter.EmitReturnClause(parentName);
            emitter.PopMethod();
            return methodName;
        }

        /// <summary>
        /// Processes the HTML element that represents a new object.
        /// </summary>
        private string ProcessObjectElement(DothtmlElementNode element)
        {
            object[] constructorParameters;
            string currentObjectName;

            var controlMetadata = controlResolver.ResolveControl(element.TagPrefix, element.TagName, out constructorParameters);
            if (controlMetadata.ControlBuilderType == null)
            {
                // compiled control
                currentObjectName = emitter.EmitCreateObject(controlMetadata.Type, constructorParameters);
            }
            else
            {
                // markup control
                currentObjectName = emitter.EmitInvokeControlBuilder(controlMetadata.Type, controlMetadata.VirtualPath);
            }
            emitter.EmitSetAttachedProperty(currentObjectName, typeof(Internal).FullName, Internal.UniqueIDProperty.Name, currentObjectName);

            // set properties from attributes
            foreach (var attribute in element.Attributes)
            {
                ProcessAttribute(attribute, controlMetadata, currentObjectName);
            }

            // process inner elements
            foreach (var child in element.Content)
            {
                ProcessNode(child, currentObjectName, controlMetadata);
            }
            return currentObjectName;
        }

        /// <summary>
        /// Processes the HTML attribute.
        /// </summary>
        private void ProcessAttribute(DothtmlAttributeNode attribute, ControlResolverMetadata controlMetadata, string currentObjectName)
        {
            if (!string.IsNullOrEmpty(attribute.AttributePrefix))
            {
                throw new NotSupportedException("Attributes with XML namespaces are not supported!"); // TODO: exception handling
            }

            // find the property
            var property = FindProperty(controlMetadata, attribute.AttributeName);
            if (property != null)
            {
                // set the property
                if (attribute.Literal is DothtmlBindingNode)
                {
                    // binding
                    var binding = (DothtmlBindingNode)attribute.Literal;
                    var bindingObjectName = emitter.EmitCreateObject(controlResolver.ResolveBinding(binding.Name), new object[] { attribute.Literal.Value });
                    emitter.EmitSetBinding(currentObjectName, property.DescriptorFullName, bindingObjectName);
                }
                else
                {
                    // hard-coded value in markup
                    var value = ReflectionUtils.ConvertValue(attribute.Literal.Value, property.PropertyType);
                    emitter.EmitSetValue(currentObjectName, property.DescriptorFullName, emitter.EmitValue(value));
                }
            }
            else if (controlMetadata.HasHtmlAttributesCollection)
            {
                // if the property is not found, add it as an HTML attribute
                if (attribute.Literal is DothtmlBindingNode)
                {
                    var binding = (DothtmlBindingNode)attribute.Literal;
                    var bindingObjectName = emitter.EmitCreateObject(controlResolver.ResolveBinding(binding.Name), new object[] { attribute.Literal.Value });
                    emitter.EmitAddHtmlAttribute(currentObjectName, attribute.AttributeName, emitter.EmitIdentifier(bindingObjectName));
                }
                else
                {
                    emitter.EmitAddHtmlAttribute(currentObjectName, attribute.AttributeName, attribute.Literal.Value);
                }
            }
            else
            {
                // TODO: exception handling
                throw new NotSupportedException(string.Format("The control {0} does not have a property {1}!", controlMetadata.Type, attribute.AttributeName));
            }
        }



        private static bool IsCollectionProperty(DotvvmProperty parentProperty)
        {
            return parentProperty.PropertyType.GetInterfaces().Contains(typeof(ICollection));
        }

        private static bool IsTemplateProperty(DotvvmProperty parentProperty)
        {
            return parentProperty.PropertyType == typeof(ITemplate);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotVVM.Framework.Configuration;
using DotVVM.VS2015Extension.DothtmlEditorExtensions.Completions.Dothtml;
using DotVVM.VS2015Extension.DothtmlEditorExtensions.Completions.Dothtml.Base;
using Microsoft.CodeAnalysis.Text;

namespace DotVVM.Framework.Tests.VS2015Extension
{
    [TestClass]
    public class MetadataControlResolverTests
    {
        private AdhocWorkspace workspace;
        private ProjectInfo project;
        private DothtmlCompletionContext context;

        
        [TestInitialize]
        public void TestInit()
        {
            try
            {
                workspace = new AdhocWorkspace();

                project = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "TestProj", "TestProj", LanguageNames.CSharp)
                    .WithMetadataReferences(new[] {
                        MetadataReference.CreateFromFile(typeof(DotvvmConfiguration).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                    });
                workspace.AddProject(project);

                workspace.AddDocument(project.Id, "test", SourceText.From("class A {}"));

                context = new DothtmlCompletionContext()
                {
                    Configuration = DotvvmConfiguration.CreateDefault(),
                    RoslynWorkspace = workspace
                };

            }
            catch (ReflectionTypeLoadException ex)
            {
                throw new Exception(string.Join("\r\n", ex.LoaderExceptions.Select(e => e.ToString())));
            }
        }


        [TestMethod]
        public void MetadataControlResolver_ReloadAllControls()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var allControls = resolver.ReloadAllControls(context);

            Assert.IsTrue(allControls.Any(c => c.DisplayText == "dot:TextBox"));
        }

        [TestMethod]
        public void MetadataControlResolver_ReloadAllControls_PropertyMetadata()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var allControls = resolver.ReloadAllControls(context);

            var repeater = allControls.FirstOrDefault(c => c.DisplayText == "dot:Repeater");

            var itemTemplateProperty = resolver.GetMetadata("dot:Repeater").Properties.FirstOrDefault(p => p.Name == "ItemTemplate");
            Assert.IsTrue(itemTemplateProperty.IsTemplate);
            Assert.IsTrue(itemTemplateProperty.IsElement);
        }

        [TestMethod]
        public void MetadataControlResolver_ReloadAttachedProperties()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var attachedProperties = resolver.GetAttachedPropertyNames(context);

            Assert.IsTrue(attachedProperties.Any(c => c.DisplayText == "RenderSettings.Mode"));
        }

        [TestMethod]
        public void MetadataControlResolver_AttachedPropertyEnumValues()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var values = resolver.GetControlAttributeValues(context, new List<string>() { "html" }, "RenderSettings.Mode");

            Assert.IsTrue(values.Any(v => v.DisplayText == "Server"));
            Assert.IsTrue(values.Any(v => v.DisplayText == "Client"));
        }

        [TestMethod]
        public void MetadataControlResolver_ElementContext_Control()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var allControls = resolver.ReloadAllControls(context);

            ControlMetadata control;
            ControlPropertyMetadata property;
            resolver.GetElementContext(new List<string>() { "html", "body", "dot:Button" }, out control, out property);

            Assert.IsNotNull(control);
            Assert.IsNull(property);
        }

        [TestMethod]
        public void MetadataControlResolver_ElementContext_HtmlGenericControl()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var allControls = resolver.ReloadAllControls(context);

            ControlMetadata control;
            ControlPropertyMetadata property;
            resolver.GetElementContext(new List<string>() { "html" }, out control, out property);

            Assert.IsNotNull(control);
            Assert.IsNull(property);

            Assert.IsTrue(control.Properties.Any(p => p.Name == "Visible"));
            Assert.IsTrue(control.Properties.Any(p => p.Name == "DataContext"));
        }

        [TestMethod]
        public void MetadataControlResolver_ElementContext_ElementProperty()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var allControls = resolver.ReloadAllControls(context);

            ControlMetadata control;
            ControlPropertyMetadata property;
            resolver.GetElementContext(new List<string>() { "html", "body", "dot:Repeater", "ItemTemplate" }, out control, out property);

            Assert.IsNotNull(control);
            Assert.IsNotNull(property);
        }

        [TestMethod]
        public void MetadataControlResolver_ElementContext_AttributeProperty()
        {
            TestInit();

            var resolver = new MetadataControlResolver();
            var allControls = resolver.ReloadAllControls(context);

            ControlMetadata control;
            ControlPropertyMetadata property;
            resolver.GetElementContext(new List<string>() { "html", "body", "dot:Repeater", "WrapperTagName" }, out control, out property);

            Assert.IsNotNull(control);
            Assert.IsNull(property);        // the property cannot be used as element
        }

        

    }
}

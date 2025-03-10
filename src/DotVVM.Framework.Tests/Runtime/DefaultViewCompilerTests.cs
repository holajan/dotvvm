using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Controls.Infrastructure;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Parser;
using DotVVM.Framework.Runtime;
using DotVVM.Framework.Runtime.Compilation;

namespace DotVVM.Framework.Tests.Runtime
{
    [TestClass]
    public class DefaultViewCompilerTests
    {
        private DotvvmRequestContext context;

        [TestInitialize]
        public void TestInit()
        {
            context = new DotvvmRequestContext();
            context.Configuration = DotvvmConfiguration.CreateDefault();
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_ElementWithAttributeProperty()
        {
            var markup = @"@viewModel System.Object, mscorlib
test <dot:Literal Text='test' />";
            var page = CompileMarkup(markup);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.AreEqual(2, page.Children.Count);

            Assert.IsInstanceOfType(page.Children[0], typeof(Literal));
            Assert.AreEqual("test ", ((Literal)page.Children[0]).Text);
            Assert.IsInstanceOfType(page.Children[1], typeof(Literal));
            Assert.AreEqual("test", ((Literal)page.Children[1]).Text);
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_ElementWithBindingProperty()
        {
            var markup = string.Format("@viewModel {0}, {1}\r\ntest <dot:Literal Text='{{{{value: FirstName}}}}' />", typeof(ViewCompilerTestViewModel).FullName, typeof(ViewCompilerTestViewModel).Assembly.GetName().Name);
            var page = CompileMarkup(markup);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.AreEqual(2, page.Children.Count);

            Assert.IsInstanceOfType(page.Children[0], typeof(Literal));
            Assert.AreEqual("test ", ((Literal)page.Children[0]).Text);
            Assert.IsInstanceOfType(page.Children[1], typeof(Literal));
            
            var binding = ((Literal)page.Children[1]).GetBinding(Literal.TextProperty) as ValueBindingExpression;
            Assert.IsNotNull(binding);
            Assert.AreEqual("FirstName", binding.OriginalString);
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_BindingInText()
        {
            var markup = string.Format("@viewModel {0}, {1}\r\ntest {{{{value: FirstName}}}}", typeof(ViewCompilerTestViewModel).FullName, typeof(ViewCompilerTestViewModel).Assembly.GetName().Name);
            var page = CompileMarkup(markup);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.AreEqual(2, page.Children.Count);

            Assert.IsInstanceOfType(page.Children[0], typeof(Literal));
            Assert.AreEqual("test ", ((Literal)page.Children[0]).Text);
            Assert.IsInstanceOfType(page.Children[1], typeof(Literal));

            var binding = ((Literal)page.Children[1]).GetBinding(Literal.TextProperty) as ValueBindingExpression;
            Assert.IsNotNull(binding);
            Assert.AreEqual("FirstName", binding.OriginalString);
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_NestedControls()
        {
            var markup = @"@viewModel System.Object, mscorlib
<dot:Placeholder>test <dot:Literal /></dot:Placeholder>";
            var page = CompileMarkup(markup);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.AreEqual(1, page.Children.Count);

            Assert.IsInstanceOfType(page.Children[0], typeof(Placeholder));

            Assert.AreEqual(2, page.Children[0].Children.Count);
            Assert.IsTrue(page.Children[0].Children.All(c => c is Literal));
            Assert.AreEqual("test ", ((Literal)page.Children[0].Children[0]).Text);
            Assert.AreEqual("", ((Literal)page.Children[0].Children[1]).Text);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DefaultViewCompiler_CodeGeneration_ElementCannotHaveContent_TextInside()
        {
            var markup = @"@viewModel System.Object, mscorlib
test <dot:Literal>aaa</dot:Literal>";
            var page = CompileMarkup(markup);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DefaultViewCompiler_CodeGeneration_ElementCannotHaveContent_BindingAndWhiteSpaceInside()
        {
            var markup = @"@viewModel System.Object, mscorlib
test <dot:Literal>{{value: FirstName}}  </dot:Literal>";
            var page = CompileMarkup(markup);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DefaultViewCompiler_CodeGeneration_ElementCannotHaveContent_ElementInside()
        {
            var markup = @"@viewModel System.Object, mscorlib
test <dot:Literal><a /></dot:Literal>";
            var page = CompileMarkup(markup);
        }
        
        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_Template()
        {
            var markup = string.Format("@viewModel {0}, {1}\r\n", typeof(ViewCompilerTestViewModel).FullName, typeof(ViewCompilerTestViewModel).Assembly.GetName().Name) +
@"<dot:Repeater DataSource=""{value: FirstName}"">
    <ItemTemplate>
        <p>This is a test</p>
    </ItemTemplate>
</dot:Repeater>";
            var page = CompileMarkup(markup);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.AreEqual(1, page.Children.Count);

            Assert.IsInstanceOfType(page.Children[0], typeof(Repeater));

            DotvvmControl placeholder = new Placeholder();
            ((Repeater)page.Children[0]).ItemTemplate.BuildContent(context, placeholder);
            placeholder = placeholder.Children[0];
            
            Assert.AreEqual(3, placeholder.Children.Count);
            Assert.IsTrue(string.IsNullOrWhiteSpace(((Literal)placeholder.Children[0]).Text));
            Assert.AreEqual("p", ((HtmlGenericControl)placeholder.Children[1]).TagName);
            Assert.AreEqual("This is a test", ((Literal)placeholder.Children[1].Children[0]).Text);
            Assert.IsTrue(string.IsNullOrWhiteSpace(((Literal)placeholder.Children[2]).Text));
        }



        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_AttachedProperty()
        {
            var markup = @"@viewModel System.Object, mscorlib
<dot:Button Validate.Enabled=""false"" /><dot:Button Validate.Enabled=""true"" /><dot:Button />";
            var page = CompileMarkup(markup);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));

            var button1 = page.Children[0];
            Assert.IsInstanceOfType(button1, typeof(Button));
            Assert.IsFalse((bool)button1.GetValue(Validate.EnabledProperty));

            var button2 = page.Children[1];
            Assert.IsInstanceOfType(button2, typeof(Button));
            Assert.IsTrue((bool)button2.GetValue(Validate.EnabledProperty));

            var button3 = page.Children[2];
            Assert.IsInstanceOfType(button3, typeof(Button));
            Assert.IsTrue((bool)button3.GetValue(Validate.EnabledProperty));
        }



        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_MarkupControl()
        {
            var markup = @"@viewModel System.Object, mscorlib
<cc:Test1 />";
            var page = CompileMarkup(markup, new Dictionary<string, string>()
            {
                { "test1.dothtml", @"@viewModel System.Object, mscorlib
<dot:Literal Text='aaa' />" }
            });

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.IsInstanceOfType(page.Children[0], typeof(DotvvmView));
            
            var literal = page.Children[0].Children[0];
            Assert.IsInstanceOfType(literal, typeof(Literal));
            Assert.AreEqual("aaa", ((Literal)literal).Text);
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_MarkupControlWithBaseType()
        {
            var markup = @"@viewModel System.Object, mscorlib
<cc:Test2 />";
            var page = CompileMarkup(markup, new Dictionary<string, string>()
            {
                { "test2.dothtml", string.Format("@baseType {0}, {1}\r\n@viewModel System.Object, mscorlib\r\n<dot:Literal Text='aaa' />", typeof(TestControl), typeof(TestControl).Assembly.GetName().Name) }
            });

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.IsInstanceOfType(page.Children[0], typeof(TestControl));

            var literal = page.Children[0].Children[0];
            Assert.IsInstanceOfType(literal, typeof(Literal));
            Assert.AreEqual("aaa", ((Literal)literal).Text);
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_MarkupControl_InTemplate()
        {
            var markup = string.Format("@viewModel {0}, {1}\r\n", typeof(ViewCompilerTestViewModel).FullName, typeof(ViewCompilerTestViewModel).Assembly.GetName().Name) +
@"<dot:Repeater DataSource=""{value: FirstName}"">
    <ItemTemplate>
        <cc:Test3 />
    </ItemTemplate>
</dot:Repeater>";
            var page = CompileMarkup(markup, new Dictionary<string, string>()
            {
                { "test3.dothtml", "@viewModel System.Char, mscorlib\r\n<dot:Literal Text='aaa' />" }
            });

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.IsInstanceOfType(page.Children[0], typeof(Repeater));

            var container = new Placeholder();
            ((Repeater)page.Children[0]).ItemTemplate.BuildContent(context, container);
            Assert.IsInstanceOfType(container.Children[0], typeof(Placeholder));

            var literal1 = container.Children[0].Children[0];
            Assert.IsInstanceOfType(literal1, typeof(Literal));
            Assert.IsTrue(string.IsNullOrWhiteSpace(((Literal)literal1).Text));

            var markupControl = container.Children[0].Children[1];
            Assert.IsInstanceOfType(markupControl, typeof(DotvvmView));
            Assert.IsInstanceOfType(markupControl.Children[0], typeof(Literal));
            Assert.AreEqual("aaa", ((Literal)markupControl.Children[0]).Text);

            var literal2 = container.Children[0].Children[2];
            Assert.IsInstanceOfType(literal2, typeof(Literal));
            Assert.IsTrue(string.IsNullOrWhiteSpace(((Literal)literal2).Text));
        }

        [TestMethod]
        public void DefaultViewCompiler_CodeGeneration_MarkupControl_InTemplate_CacheTest()
        {
            var markup = string.Format("@viewModel {0}, {1}\r\n", typeof(ViewCompilerTestViewModel).FullName, typeof(ViewCompilerTestViewModel).Assembly.GetName().Name) +
@"<dot:Repeater DataSource=""{value: FirstName}"">
    <ItemTemplate>
        <cc:Test4 />
    </ItemTemplate>
</dot:Repeater>";
            var page = CompileMarkup(markup, new Dictionary<string, string>()
            {
                { "test4.dothtml", "@viewModel System.Char, mscorlib\r\n<dot:Literal Text='aaa' />" }
            }, compileTwice: true);

            Assert.IsInstanceOfType(page, typeof(DotvvmView));
            Assert.IsInstanceOfType(page.Children[0], typeof(Repeater));

            var container = new Placeholder();
            ((Repeater)page.Children[0]).ItemTemplate.BuildContent(context, container);
            Assert.IsInstanceOfType(container.Children[0], typeof(Placeholder));

            var literal1 = container.Children[0].Children[0];
            Assert.IsInstanceOfType(literal1, typeof(Literal));
            Assert.IsTrue(string.IsNullOrWhiteSpace(((Literal)literal1).Text));

            var markupControl = container.Children[0].Children[1];
            Assert.IsInstanceOfType(markupControl, typeof(DotvvmView));
            Assert.IsInstanceOfType(markupControl.Children[0], typeof(Literal));
            Assert.AreEqual("aaa", ((Literal)markupControl.Children[0]).Text);

            var literal2 = container.Children[0].Children[2];
            Assert.IsInstanceOfType(literal2, typeof(Literal));
            Assert.IsTrue(string.IsNullOrWhiteSpace(((Literal)literal2).Text));
        }



        private DotvvmControl CompileMarkup(string markup, Dictionary<string, string> markupFiles = null, bool compileTwice = false, [CallerMemberName]string fileName = null)
        {
            if (markupFiles == null)
            {
                markupFiles = new Dictionary<string, string>();
            }
            markupFiles[fileName + ".dothtml"] = markup;

            var dotvvmConfiguration = context.Configuration;
            dotvvmConfiguration.Markup.Controls.Add(new DotvvmControlConfiguration() { TagPrefix = "cc", TagName = "Test1", Src = "test1.dothtml" });
            dotvvmConfiguration.Markup.Controls.Add(new DotvvmControlConfiguration() { TagPrefix = "cc", TagName = "Test2", Src = "test2.dothtml" });
            dotvvmConfiguration.Markup.Controls.Add(new DotvvmControlConfiguration() { TagPrefix = "cc", TagName = "Test3", Src = "test3.dothtml" });
            dotvvmConfiguration.Markup.Controls.Add(new DotvvmControlConfiguration() { TagPrefix = "cc", TagName = "Test4", Src = "test4.dothtml" });
            dotvvmConfiguration.ServiceLocator.RegisterSingleton<IMarkupFileLoader>(() => new FakeMarkupFileLoader(markupFiles));
            dotvvmConfiguration.Markup.AddAssembly(Assembly.GetExecutingAssembly().GetName().Name);

            var controlBuilderFactory = dotvvmConfiguration.ServiceLocator.GetService<IControlBuilderFactory>();
            var controlBuilder = controlBuilderFactory.GetControlBuilder(fileName + ".dothtml");
            
            var result = controlBuilder.BuildControl(controlBuilderFactory);
            if (compileTwice)
            {
                result = controlBuilder.BuildControl(controlBuilderFactory);
            }
            return result;
        }
        
    }

    public class ViewCompilerTestViewModel
    {
        public string FirstName { get; set; }
    }

    public class TestControl : DotvvmMarkupControl
    {
        
    }

    public class FakeMarkupFileLoader : IMarkupFileLoader
    {
        private readonly Dictionary<string, string> markupFiles;

        public FakeMarkupFileLoader(Dictionary<string, string> markupFiles = null)
        {
            this.markupFiles = markupFiles ?? new Dictionary<string, string>();
        }

        public MarkupFile GetMarkup(DotvvmConfiguration configuration, string virtualPath)
        {
            return new MarkupFile(virtualPath, virtualPath, markupFiles[virtualPath]);
        }

        public string GetMarkupFileVirtualPath(DotvvmRequestContext context)
        {
            throw new NotImplementedException();
        }
    }
}

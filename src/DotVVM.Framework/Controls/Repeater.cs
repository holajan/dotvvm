using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime;
using System.Collections;
using System.Diagnostics;
using DotVVM.Framework.Runtime.Compilation.JavascriptCompilation;

namespace DotVVM.Framework.Controls
{
    /// <summary>
    /// Repeats a specified template for each of the items in the <see cref="DotvvmBindableControl.DataContext"/> property.
    /// </summary>
    [ControlMarkupOptions(AllowContent = false, DefaultContentProperty = "ItemTemplate")]
    public class Repeater : ItemsControl
    {

        /// <summary>
        /// Gets or sets the template for each <see cref="Repeater"/> item.
        /// </summary>
        [MarkupOptions(MappingMode = MappingMode.InnerElement)]
        [ControlPropertyBindingDataContextChange("DataSource")]
        [CollectionElementDataContextChange]
        public ITemplate ItemTemplate
        {
            get { return (ITemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly DotvvmProperty ItemTemplateProperty =
            DotvvmProperty.Register<ITemplate, Repeater>(t => t.ItemTemplate, null);

        /// <summary>
        /// Gets or sets the name of the tag that wraps the Repeater.
        /// </summary>
        public string WrapperTagName
        {
            get { return (string)GetValue(WrapperTagNameProperty); }
            set { SetValue(WrapperTagNameProperty, value); }
        }
        public static readonly DotvvmProperty WrapperTagNameProperty =
            DotvvmProperty.Register<string, Repeater>(t => t.WrapperTagName, "div");

        /// <summary>
        /// Initializes a new instance of the <see cref="Repeater"/> class.
        /// </summary>
        public Repeater()
        {
        }


        /// <summary>
        /// Occurs after the viewmodel is applied to the page and before the commands are executed.
        /// </summary>
        protected internal override void OnLoad(DotvvmRequestContext context)
        {
            DataBind(context);
            base.OnLoad(context);
        }

        /// <summary>
        /// Occurs after the page commands are executed.
        /// </summary>
        protected internal override void OnPreRender(DotvvmRequestContext context)
        {
            DataBind(context);     // TODO: we should handle observable collection operations to persist controlstate of controls inside the Repeater
            base.OnPreRender(context);
        }

        private object[] lastBoundArray = null;

        /// <summary>
        /// Performs the data-binding and builds the controls inside the <see cref="Repeater"/>.
        /// </summary>
        private void DataBind(DotvvmRequestContext context)
        {
            var dataSourceBinding = GetDataSourceBinding();

            var index = 0;
            var dataSource = DataSource;
            if (dataSource != null)
            {
                var items = GetIEnumerableFromDataSource(dataSource).Cast<object>().ToArray();
                if (lastBoundArray != null)
                {
                    if (lastBoundArray.SequenceEqual(items)) return;
                }
                Children.Clear();
                foreach (var item in items)
                {
                    var placeholder = new DataItemContainer { DataItemIndex = index };
                    ItemTemplate.BuildContent(context, placeholder);
                    Children.Add(placeholder);
                    placeholder.SetBinding(DataContextProperty, GetItemBinding((IList)items, dataSourceBinding.GetKnockoutBindingExpression(), index));
                    Debug.Assert(placeholder.properties[DataContextProperty] != null);
                    index++;
                }
            }
        }


        /// <summary>
        /// Adds all attributes that should be added to the control begin tag.
        /// </summary>
        protected override void AddAttributesToRender(IHtmlWriter writer, RenderContext context)
        {
            TagName = WrapperTagName;

            if (!RenderOnServer)
            {
                writer.AddKnockoutForeachDataBind(GetDataSourceBinding().GetKnockoutBindingExpression());
            }

            base.AddAttributesToRender(writer, context);
        }

        /// <summary>
        /// Renders the contents inside the control begin and end tags.
        /// </summary>
        protected override void RenderContents(IHtmlWriter writer, RenderContext context)
        {
            var dataSourceBinding = GetDataSourceBinding();

            if (RenderOnServer)
            {
                // render on server
                var index = 0;
                foreach (var child in Children)
                {
                    Children[index].Render(writer, context);
                    index++;
                }
            }
            else
            {
                // render on client
                var placeholder = new DataItemContainer() { DataContext = null };
                placeholder.SetValue(Internal.PathFragmentProperty, JavascriptCompilationHelper.AddIndexerToViewModel(dataSourceBinding.GetKnockoutBindingExpression(), "$index"));
                Children.Add(placeholder);
                ItemTemplate.BuildContent(context.RequestContext, placeholder);

                placeholder.Render(writer, context);
            }
        }
    }
}

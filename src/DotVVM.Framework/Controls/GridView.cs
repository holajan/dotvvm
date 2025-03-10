using DotVVM.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime;
using System.Collections;
using DotVVM.Framework.Runtime.Compilation.JavascriptCompilation;

namespace DotVVM.Framework.Controls
{
    public class GridView : ItemsControl
    {

        public GridView() : base("table")
        {
            Columns = new List<GridViewColumn>();
            RowDecorators = new List<Decorator>();
        }


        [MarkupOptions(AllowBinding = false, MappingMode = MappingMode.InnerElement)]
        [ControlPropertyBindingDataContextChange("DataSource")]
        [CollectionElementDataContextChange]
        public List<GridViewColumn> Columns
        {
            get { return (List<GridViewColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public static readonly DotvvmProperty ColumnsProperty =
            DotvvmProperty.Register<List<GridViewColumn>, GridView>(c => c.Columns);


        [MarkupOptions(AllowBinding = false, MappingMode = MappingMode.InnerElement)]
        [ControlPropertyBindingDataContextChange("DataSource")]
        [CollectionElementDataContextChange]
        public List<Decorator> RowDecorators
        {
            get { return (List<Decorator>)GetValue(RowDecoratorsProperty); }
            set { SetValue(RowDecoratorsProperty, value); }
        }
        public static readonly DotvvmProperty RowDecoratorsProperty =
            DotvvmProperty.Register<List<Decorator>, GridView>(c => c.RowDecorators);

        [ConstantDataContextChange(typeof(string))]
        [MarkupOptions(AllowHardCodedValue = false)]
        public Action<string> SortChanged
        {
            get { return (Action<string>)GetValue(SortChangedProperty); }
            set { SetValue(SortChangedProperty, value); }
        }
        public static readonly DotvvmProperty SortChangedProperty =
            DotvvmProperty.Register<Action<string>, GridView>(c => c.SortChanged, null);


        protected internal override void OnLoad(DotvvmRequestContext context)
        {
            DataBind(context);
            base.OnLoad(context);
        }

        protected internal override void OnPreRender(DotvvmRequestContext context)
        {
            DataBind(context);     // TODO: support for observable collection
            base.OnPreRender(context);
        }


        private void DataBind(DotvvmRequestContext context)
        {
            Children.Clear();

            var dataSourceBinding = GetDataSourceBinding();
            // var dataSourcePath = dataSourceBinding.GetViewModelPathExpression(this, DataSourceProperty);
            var dataSource = DataSource;

            Action<string> sortCommand = null;
            if (dataSource is IGridViewDataSet)
            {
                sortCommand = ((IGridViewDataSet)dataSource).SetSortExpression; // dataSourcePath + ".SetSortExpression";
            }
            else
            {
                var sortCommandBinding = GetCommandBinding(SortChangedProperty);
                if (sortCommandBinding != null)
                {
                    sortCommand = s => sortCommandBinding.Delegate(new []{ s }.Concat(BindingExpression.GetDataContexts(this, true)).ToArray(), null);
                }
            }

            var index = 0;
            if (dataSource != null)
            {
                // create header row
                CreateHeaderRow(context, sortCommand);
                var items = GetIEnumerableFromDataSource(dataSource);
                foreach (var item in items)
                {
                    // create row
                    var placeholder = new DataItemContainer { DataItemIndex = index };
                    placeholder.SetBinding(DataContextProperty, GetItemBinding((IList)items, dataSourceBinding.GetKnockoutBindingExpression(), index));
                    Children.Add(placeholder);

                    CreateRow(context, placeholder);

                    index++;
                }
            }
        }

        private void CreateHeaderRow(DotvvmRequestContext context, Action<string> sortCommand)
        {
            var head = new HtmlGenericControl("thead");
            Children.Add(head);

            var headerRow = new HtmlGenericControl("tr");
            head.Children.Add(headerRow);
            foreach (var column in Columns)
            {
                var cell = new HtmlGenericControl("th");
                SetCellAttributes(column, cell, true);
                headerRow.Children.Add(cell);

                column.CreateHeaderControls(context, this, sortCommand, cell);
            }
        }

        private static void SetCellAttributes(GridViewColumn column, HtmlGenericControl cell, bool isHeaderCell)
        {
            if (!string.IsNullOrEmpty(column.Width))
            {
                cell.Attributes["style"] = "width: " + column.Width;
            }

            var cssClassBinding = column.GetValueBinding(isHeaderCell ? GridViewColumn.CssClassProperty : GridViewColumn.HeaderCssClassProperty);

            if (cssClassBinding != null)
            {
                cell.Attributes["class"] = cssClassBinding;
            }
            else if (!string.IsNullOrWhiteSpace(column.CssClass))
            {
                cell.Attributes["class"] = column.CssClass;
            }
        }

        private void CreateRow(DotvvmRequestContext context, DataItemContainer placeholder)
        {
            var row = new HtmlGenericControl("tr");

            DotvvmControl container = row;
            foreach (var decorator in RowDecorators)
            {
                var decoratorInstance = decorator.Clone();
                decoratorInstance.Children.Add(container);
                container = decoratorInstance;
            }
            placeholder.Children.Add(container);

            // create cells
            foreach (var column in Columns)
            {
                var cell = new HtmlGenericControl("td");
                SetCellAttributes(column, cell, false);
                row.Children.Add(cell);
                column.CreateControls(context, cell);
            }
        }

        protected override void RenderContents(IHtmlWriter writer, RenderContext context)
        {
            if (Children.Count == 0) return;

            // render the header
            Children[0].Render(writer, context);

            // render body
            var dataSourceBinding = GetDataSourceBinding();
            if (!RenderOnServer)
            {
                var expression = dataSourceBinding.GetKnockoutBindingExpression();
                writer.AddKnockoutForeachDataBind(expression);
            }
            writer.RenderBeginTag("tbody");

            // render contents
            if (RenderOnServer)
            {
                // render on server
                var index = 0;
                foreach (var child in Children.Skip(1))
                {
                    Children[index].Render(writer, context);
                    index++;
                }
            }
            else
            {
                // render on client
                var placeholder = new DataItemContainer { DataContext = null };
                placeholder.SetValue(Internal.PathFragmentProperty, 
                    JavascriptCompilationHelper.AddIndexerToViewModel(dataSourceBinding.GetKnockoutBindingExpression(), "$index"));
                Children.Add(placeholder);

                CreateRow(context.RequestContext, placeholder);

                placeholder.Render(writer, context);
            }

            writer.RenderEndTag();
        }
    }
}

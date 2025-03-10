using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime;
using DotVVM.Framework.Utils;

namespace DotVVM.Framework.Controls
{
    /// <summary>
    /// Renders a HTML drop-down list.
    /// </summary>
    public class ComboBox : SelectHtmlControlBase
    {

        [MarkupOptions(AllowBinding = false)]
        public string EmptyItemText
        {
            get { return (string) GetValue(EmptyItemTextProperty); }
            set { SetValue(EmptyItemTextProperty, value); }
        }
        public static readonly DotvvmProperty EmptyItemTextProperty 
            = DotvvmProperty.Register<string, ComboBox>(c => c.EmptyItemText, string.Empty);



        protected override void AddAttributesToRender(IHtmlWriter writer, RenderContext context)
        {
            if (!RenderOnServer)
            {
                if (!string.IsNullOrWhiteSpace(EmptyItemText))
                {
                    writer.AddKnockoutDataBind("optionsCaption", KnockoutHelper.MakeStringLiteral(EmptyItemText));
                }
            }

            base.AddAttributesToRender(writer, context);
        }

        protected override void RenderContents(IHtmlWriter writer, RenderContext context)
        {
            if (RenderOnServer)
            {
                if (!string.IsNullOrWhiteSpace(EmptyItemText))
                {
                    writer.RenderBeginTag("option");
                    writer.WriteText(EmptyItemText);
                    writer.RenderEndTag();
                }
            }

            base.RenderContents(writer, context);
        }
    }
}

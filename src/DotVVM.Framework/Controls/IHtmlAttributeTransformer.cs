using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime;

namespace DotVVM.Framework.Controls
{
    public interface IHtmlAttributeTransformer
    {

        /// <summary>
        /// Renders the attribute name and value into a specified writer.
        /// </summary>
        void RenderHtmlAttribute(IHtmlWriter writer, DotvvmRequestContext requestContext, string attributeName, string attributeValue);

    }
}
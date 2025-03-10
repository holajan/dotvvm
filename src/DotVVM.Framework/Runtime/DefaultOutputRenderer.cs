using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Controls.Infrastructure;
using DotVVM.Framework.Hosting;

namespace DotVVM.Framework.Runtime
{
    public class DefaultOutputRenderer : IOutputRenderer
    {
        public void RenderPage(DotvvmRequestContext context, DotvvmView view)
        {
            // embed resource links
            EmbedResourceLinks(view);

            // prepare the render context
            var renderContext = new RenderContext(context);

            // get the HTML
            using (var textWriter = new StringWriter())
            {
                var htmlWriter = new HtmlWriter(textWriter, context);
                view.Render(htmlWriter, renderContext);
                context.RenderedHtml = textWriter.ToString();
            }
        }

        public async Task WriteHtmlResponse(DotvvmRequestContext context)
        {
            // return the response
            context.OwinContext.Response.ContentType = "text/html; charset=utf-8";
            await context.OwinContext.Response.WriteAsync(context.RenderedHtml);
        }


        public async Task WriteViewModelResponse(DotvvmRequestContext context, DotvvmView view)
        {
            // return the response
            context.OwinContext.Response.ContentType = "application/json; charset=utf-8";
            var serializedViewModel = context.GetSerializedViewModel();
            await context.OwinContext.Response.WriteAsync(serializedViewModel);
        }

        public async Task RenderPlainJsonResponse(IOwinContext context, object data)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(data));
        }

        public async Task RenderHtmlResponse(IOwinContext context, string html)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        }

        public async Task RenderPlainTextResponse(IOwinContext context, string text)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(text);
        }


        /// <summary>
        /// Embeds the resource links in the page.
        /// </summary>
        private void EmbedResourceLinks(DotvvmView view)
        {
            var sections = view.GetThisAndAllDescendants()
                .OfType<HtmlGenericControl>()
                .Where(t => t.TagName == "head" || t.TagName == "body")
                .OrderBy(t => t.TagName)
                .ToList();

            if (sections.Count != 2 || sections[0].TagName == sections[1].TagName)
            {
                throw new Exception("The page must have exactly one <head> and one <body> section!");
            }

            sections[0].Children.Add(new BodyResourceLinks());
            sections[1].Children.Add(new HeadResourceLinks());
        }
    }
}
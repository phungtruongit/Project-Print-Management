using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("RazorTemplateEngineTests")]

namespace PrintManagement.EmailHub
{
    public sealed class RazorTemplateEngine
    {
        private const string TemplateFolderName = "Templates";
        private readonly Dictionary<string, RazorCompiledItem> _razorCompiledItems = new Dictionary<string, RazorCompiledItem>();

        public RazorTemplateEngine()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var viewAssembly = RelatedAssemblyAttribute.GetRelatedAssemblies(thisAssembly, false).Single();
            var razorCompiledItems = new RazorCompiledItemLoader().LoadItems(viewAssembly);

            foreach (var item in razorCompiledItems)
            {
                _razorCompiledItems.Add(item.Identifier, item);
            }
        }

        public async Task<string> RenderTemplateAsync<TModel>(TModel model)
        {
            var templateNamePrefix = model.GetType().Name;

            using var stringWriter = new StringWriter();
            await stringWriter.WriteAsync(await RenderTemplateAsync(TemplateFolderName, templateNamePrefix, model));

            stringWriter.Flush();
            return stringWriter.ToString();
        }

        private async Task<string> RenderTemplateAsync<TModel>(string templateFolderName, string templateName, TModel model)
        {
            var razorTemplate = GetRazorTemplateName(templateFolderName, templateName);
            var razorCompiledItem = _razorCompiledItems[razorTemplate];
            return await GetRenderedOutput(razorCompiledItem, model);
        }

        private static string GetRazorTemplateName(string templateFolderName, string templateName)
        {
            return $"/{templateFolderName}/{templateName}.cshtml";
        }

        private static async Task<string> GetRenderedOutput<TModel>(RazorCompiledItem razorCompiledItem, TModel model)
        {
            using var stringWriter = new StringWriter();
            var razorPage = GetRazorPageInstance(razorCompiledItem, model, stringWriter);
            await razorPage.ExecuteAsync();
            return stringWriter.ToString();
        }

        private static RazorPage GetRazorPageInstance<TModel>(RazorCompiledItem razorCompiledItem, TModel model, TextWriter textWriter)
        {
            var razorPage = (RazorPage<TModel>)Activator.CreateInstance(razorCompiledItem.Type);

            razorPage.ViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            razorPage.ViewContext = new ViewContext
            {
                Writer = textWriter
            };

            razorPage.HtmlEncoder = HtmlEncoder.Default;
            return razorPage;
        }
    }
}

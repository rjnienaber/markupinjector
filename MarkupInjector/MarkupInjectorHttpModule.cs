using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using MarkupInjector;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(MarkupInjectorHttpModule), "Register")]
namespace MarkupInjector
{
    public class MarkupInjectorHttpModule : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.ReleaseRequestState += ReleaseRequestState;
        }

        public void ReleaseRequestState(object sender, EventArgs e)
        {
            ReleaseRequestState(new HttpContextWrapper(HttpContext.Current));
        }

        internal void ReleaseRequestState(HttpContextBase context)
        {
            if (!Settings.ShouldInterceptResponse) return;

            var response = context.Response;
            if (response.ContentType != "text/html" || IsARedirect(response))
                return;

            var filter = GetFilter(response);

            foreach (var headInject in Settings.HeadListeners)
                filter.EndOfHeadDetected += writer => writer.Write(headInject());

            foreach (var bodyInject in Settings.BodyListeners)
                filter.EndOfBodyDetected += writer => writer.Write(bodyInject());
        }

        public virtual InsertMarkupFilter GetFilter(HttpResponseBase response) {
            return InsertMarkupFilter.InterceptResponse(response);
        }

        bool IsARedirect(HttpResponseBase response)
        {
            return response.StatusCode == 301 || response.StatusCode == 302;
        }

        public static void Register()
        {
            DynamicModuleUtility.RegisterModule(typeof(MarkupInjectorHttpModule));
        }
    }
}

using PriorityQueueWebAPI.Models;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace PriorityQueueWebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            

            // New Code
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customer");
            builder.EntitySet<Job>("Job");
            builder.EntitySet<Manager>("Manager");
            builder.EntitySet<Technician>("Technician");
            builder.EntitySet<DailyStatistic>("DailyStatistic");
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel(),
                defaultHandler: HttpClientFactory.CreatePipeline(innerHandler: new HttpControllerDispatcher(config),
                handlers: new[] { new ODataNullValueMessageHandler() })
                );

            //Enable $select, $expand, and others
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
        }
    }
}

using System.Linq;
using System.Net;
using System.Web.Http;
using Skybrud.Umbraco.Analytics.Extensions;
using Skybrud.Umbraco.Analytics.Models.Api;
using Skybrud.Umbraco.Analytics.Models.Config;
using Skybrud.Umbraco.Analytics.Models.Json;

namespace Skybrud.Umbraco.Analytics.Controllers.Api {

    public partial class AnalyticsController {

        [System.Web.Mvc.HttpGet]
        public object GetClients() {
            return (
                from client in AnalyticsConfig.Current.GetClients()
                select new {
                    id = client.Id,
                    name = client.Name,
                    clientId = client.ClientId,
                    clientSecret = client.ClientSecret
                }
            );
        }

        [System.Web.Mvc.HttpDelete]
        public object DeleteClient(string id) {

            var config = AnalyticsConfig.Current;

            var client = config.GetClientById(id);
            if (client == null) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.NotFound, "OAuth client not found."));

            config.DeleteClient(client, Security.CurrentUser);

            return true;

        }

        [System.Web.Mvc.HttpPost]
        public object AddClient([FromBody] ClientModel model) {

            var config = AnalyticsConfig.Current;

            return new ClientModel(config.AddClient(model.Name, model.ClientId, model.ClientSecret, Security.CurrentUser));

        }

        [System.Web.Mvc.HttpPost]
        public object SaveClient([FromBody] ClientModel model) {

            var config = AnalyticsConfig.Current;

            var client = config.GetClientById(model.Id);
            if (client == null) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.NotFound, "OAuth client not found."));

            client.Name = model.Name;
            client.ClientId = model.ClientId;
            client.ClientSecret = model.ClientSecret;

            return new ClientModel(config.SaveClient(client, Security.CurrentUser));

        }

    }

}
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
        public object GetUsers() {
            return (
                from user in AnalyticsConfig.Current.GetUsers()
                select new UserModel(user)
            );
        }

        [HttpPost]
        public object AddUser([FromBody] AddUserModel model) {

            var config = AnalyticsConfig.Current;

            var client = config.GetClientById(model.Client.Id);

            return new UserModel(config.AddUser(client, model.User, Security.CurrentUser));

        }

        [System.Web.Mvc.HttpDelete]
        public object DeleteUser(string id) {

            var config = AnalyticsConfig.Current;

            AnalyticsConfigUser user = config.GetUserById(id);
            if (user == null) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.NotFound, "User not found."));

            config.DeleteUser(user, Security.CurrentUser);

            return true;

        }

    }

}
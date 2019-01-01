using Newtonsoft.Json;
using Skybrud.Umbraco.Analytics.Models.Config;

namespace Skybrud.Umbraco.Analytics.Models.Api {

    public class UserModel {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public UserModel() { }

        public UserModel(AnalyticsConfigUser user) {
            Id = user.Id;
            UserId = user.UserId;
            Email = user.Email;
            Name = user.Name;
        }

    }

}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Config {

    public class AnalyticsConfigUser {

        [JsonIgnore]
        public AnalyticsConfigClient Client { get; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        public  AnalyticsConfigUser() { }

        private AnalyticsConfigUser(AnalyticsConfigClient client, JObject obj) {
            Client = client;
            Id = obj.GetString("id");
            UserId = obj.GetString("userId");
            Email = obj.GetString("email");
            Name = obj.GetString("name");
            RefreshToken = obj.GetString("refreshToken");
        }

        public static AnalyticsConfigUser Parse(AnalyticsConfigClient client, JObject obj) {
            return obj == null ? null : new AnalyticsConfigUser(client, obj);
        }

    }

}
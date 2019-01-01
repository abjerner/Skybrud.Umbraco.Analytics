using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Config {

    public class AnalyticsConfigClient {

        #region Properties

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty("users")]
        public List<AnalyticsConfigUser> Users { get; set; }

        #endregion

        #region Constructors

        public AnalyticsConfigClient() {
            Users = new List<AnalyticsConfigUser>();
        }

        public AnalyticsConfigClient(JObject obj) {
            Id = obj.GetString("id");
            Name = obj.GetString("name");
            ClientId = obj.GetString("clientId");
            ClientSecret = obj.GetString("clientSecret");
            Users = obj.GetArrayItems("users", x => AnalyticsConfigUser.Parse(this, x)).ToList();
        }

        #endregion

        #region Static methods

        public static AnalyticsConfigClient Parse(JObject obj) {
            return obj == null ? null : new AnalyticsConfigClient(obj);
        }

        #endregion

    }

}
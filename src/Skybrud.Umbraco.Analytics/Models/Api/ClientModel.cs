using Newtonsoft.Json;
using Skybrud.Umbraco.Analytics.Models.Config;

namespace Skybrud.Umbraco.Analytics.Models.Api {

    public class ClientModel {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }

        public ClientModel() { }

        public ClientModel(AnalyticsConfigClient client) {
            Id = client.Id;
            Name = client.Name;
            ClientId = client.ClientId;
            ClientSecret = client.ClientSecret;
        }

    }

}
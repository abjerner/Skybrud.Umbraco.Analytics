using Newtonsoft.Json;

namespace Skybrud.Umbraco.Analytics.Models.Api {

    public class AddUserModel {

        [JsonProperty("client")]
        public ClientModel Client { get; set; }

        [JsonProperty("user")]
        public UserModel User { get; set; }

        public class UserModel {

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("refreshToken")]
            public string RefreshToken { get; set; }

            [JsonProperty("authenticatedAt")]
            public string AuthenticatedAt { get; set; }

        }

    }

}
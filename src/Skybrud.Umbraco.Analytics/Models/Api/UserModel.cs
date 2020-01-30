using Newtonsoft.Json;
using Skybrud.Essentials.Time;
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

        [JsonProperty("authenticatedAt")]
        public EssentialsTime AuthenticatedAt { get; set; }

        [JsonProperty("authenticatedAtAgo")]
        public string AuthenticatedAtAgo { get; set; }

        public UserModel() { }

        public UserModel(AnalyticsConfigUser user) {
            Id = user.Id;
            UserId = user.UserId;
            Email = user.Email;
            Name = user.Name;
            AuthenticatedAt = user.AuthenticatedAt;

            if (AuthenticatedAt.IsToday) {
                AuthenticatedAtAgo = "today";
            } else if (AuthenticatedAt.AddDays(1).IsToday) {
                AuthenticatedAtAgo = "yesterday";
            } else {
                AuthenticatedAtAgo = $"{EssentialsTime.Now.Subtract(AuthenticatedAt).TotalDays:N0} days ago";
            }

        }

    }

}
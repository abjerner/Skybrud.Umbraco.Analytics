using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Essentials.Time;
using Skybrud.Social.Google.Analytics.Models.Accounts;
using Skybrud.Social.Google.Analytics.Models.Profiles;
using Skybrud.Social.Google.Analytics.Models.WebProperties;

namespace Skybrud.Umbraco.Analytics.Models.Api.Selector {

    public class AccountModel {

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("webProperties")]
        public List<WebPropertyModel> WebProperties { get; }

        public AccountModel(string id, string name) {
            Id = id;
            Name = name;
            WebProperties = new List<WebPropertyModel>();
        }

        public AccountModel(AnalyticsAccount account) {
            Id = account.Id;
            Name = account.Name;
            WebProperties = new List<WebPropertyModel>();
        }

    }

    public class WebPropertyModel {

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("profiles")]
        public List<ProfileModel> Profiles { get; }

        public WebPropertyModel(string id, string name) {
            Id = id;
            Name = name;
            Profiles = new List<ProfileModel>();
        }

        public WebPropertyModel(AnalyticsWebProperty webProperty, ProfileModel[] profiles) {
            Id = webProperty.Id;
            Name = webProperty.Name;
            Profiles = profiles?.ToList() ?? new List<ProfileModel>();
        }

        public WebPropertyModel(AnalyticsWebProperty webProperty, AnalyticsProfile[] profiles) {
            Id = webProperty.Id;
            Name = webProperty.Name;
            Profiles = profiles?.Select(x => new ProfileModel(x)).ToList() ?? new List<ProfileModel>();
        }

    }

    public class ProfileModel {

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("currency")]
        public string Currency { get; }

        [JsonProperty("timezone")]
        public string Timezone { get; }

        [JsonProperty("websiteUrl")]
        public string WebsiteUrl { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("created")]
        public string Created { get; }

        [JsonProperty("updated")]
        public string Updated { get; }

        public ProfileModel(string id, string name) {
            Id = id;
            Name = name;
        }

        public ProfileModel(AnalyticsProfile profile) {
            Id = profile.Id;
            Name = profile.Name;
            Currency = profile.Currency;
            Timezone = profile.Timezone;
            WebsiteUrl = AddHttp(profile.WebsiteUrl);
            Type = profile.Type;
            Created = TimeUtils.ToIso8601(profile.Created);
            Updated = TimeUtils.ToIso8601(profile.Updated);
        }

        private string AddHttp(string url) {
            if (url == null || url.StartsWith("http")) return url;
            return "http://" + url;
        }

    }

}
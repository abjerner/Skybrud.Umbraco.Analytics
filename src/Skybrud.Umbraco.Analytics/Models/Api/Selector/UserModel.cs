using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Social.Google.Analytics.Models.Accounts;
using Skybrud.Social.Google.Analytics.Models.Profiles;
using Skybrud.Social.Google.Analytics.Models.WebProperties;
using Skybrud.Umbraco.Analytics.Models.Config;

namespace Skybrud.Umbraco.Analytics.Models.Api.Selector {

    public class UserModel {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("accounts")]
        public AccountModel[] Accounts { get; set; }

        public UserModel(AnalyticsConfigUser user, AnalyticsAccount[] accounts, AnalyticsWebProperty[] webProperties, AnalyticsProfile[] profiles) {

            Id = user.Id;

            Dictionary<string, AnalyticsProfile[]> profiles2 = profiles.GroupBy(x => x.WebPropertyId).ToDictionary(x => x.Key, x => x.ToArray());

            Dictionary<string, AccountModel> accounts2 = (
                from account in accounts
                select new AccountModel(account)
            ).ToDictionary(x => x.Id);

            foreach (AnalyticsWebProperty webProperty in webProperties) {

                // Attempt to get the parent account (if not found, skip th web property)
                if (accounts2.TryGetValue(webProperty.AccountId, out AccountModel am) == false) continue;

                profiles2.TryGetValue(webProperty.Id, out AnalyticsProfile[] wpp);

                WebPropertyModel wpm = new WebPropertyModel(webProperty, wpp);

                am.WebProperties.Add(wpm);

            }

            Accounts = accounts2.Values.ToArray();

        }

    }

}
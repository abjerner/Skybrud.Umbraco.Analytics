using System.Web.Configuration;
using Umbraco.Core.Configuration;

namespace Skybrud.Umbraco.Analytics.Extensions {

    public static class UmbracoExtensions {

        public static AnalyticsConfig SkybrudAnalytics(this UmbracoConfig config) {
            return new AnalyticsConfig();
        }

    }

    public class AnalyticsConfig {

        public string GoogleClientId => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleClientId"];

        public string GoogleClientSecret => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleClientSecret"];

        public string GoogleRefreshToken => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleRefreshToken"];

        public string AnalyticsProfileId => WebConfigurationManager.AppSettings["SkybrudAnalytics.AnalyticsProfileId"];

    }

}
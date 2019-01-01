using System.Web;
using Skybrud.Umbraco.Analytics.Models.Config;
using Umbraco.Core.Configuration;

namespace Skybrud.Umbraco.Analytics.Extensions {

    public static class UmbracoExtensions {

        public static AnalyticsConfig SkybrudAnalytics(this UmbracoConfig config) {

            if (HttpContext.Current == null) return new AnalyticsConfig();

            AnalyticsConfig analytics = HttpContext.Current.Items["Skybrud.AnalyticsConfig"] as AnalyticsConfig;

            if (analytics == null) {
                HttpContext.Current.Items["Skybrud.AnalyticsConfig"] = analytics = new AnalyticsConfig();
            }

            return analytics;

        }

    }
}
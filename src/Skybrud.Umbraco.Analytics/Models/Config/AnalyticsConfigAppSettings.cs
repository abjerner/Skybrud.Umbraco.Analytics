using System.Web.Configuration;
using Skybrud.Essentials.Strings.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Config {

    public class AnalyticsConfigAppSettings {

        #region Properties

        public string GoogleClientId => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleClientId"];

        public string GoogleClientSecret => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleClientSecret"];

        public string GoogleRefreshToken => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleRefreshToken"];

        public string AnalyticsProfileId => WebConfigurationManager.AppSettings["SkybrudAnalytics.AnalyticsProfileId"];

        public bool IsValid => GoogleClientId.HasValue() && GoogleClientSecret.HasValue() && GoogleRefreshToken.HasValue() && AnalyticsProfileId.HasValue();

        #endregion

        #region COnstructors

        public AnalyticsConfigAppSettings() { }
        
        #endregion

    }

}
using System;
using System.Linq;
using System.Net;
using Skybrud.Social.Google.Analytics.Endpoints;
using Skybrud.Social.Google.Analytics.Models.Common;
using Skybrud.Social.Google.Analytics.Models.Data;
using Skybrud.Social.Google.Analytics.Models.Dimensions;
using Skybrud.Social.Google.Analytics.Models.Metrics;
using Skybrud.Social.Google.Analytics.Options.Data;
using Skybrud.Social.Google.Analytics.Options.Data.Dimensions;
using Skybrud.Social.Google.Analytics.Options.Management;
using Skybrud.Social.Google.Analytics.Responses.Data;
using Skybrud.Social.Google.Common;
using Skybrud.Umbraco.Analytics.Extensions;
using Skybrud.Umbraco.Analytics.Models;
using Skybrud.Umbraco.Analytics.Models.Config;
using Skybrud.Umbraco.Analytics.Models.Json;
using Skybrud.Umbraco.Analytics.Models.Selection;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Analytics.Controllers.Api {

    [PluginController("Skybrud")]
    public partial class AnalyticsController : UmbracoAuthorizedApiController {

        [System.Web.Mvc.HttpGet]
        public object GetStatus() {

            AnalyticsConfig cfg = AnalyticsConfig.Current;

            return new {
                clients = new {
                    count = cfg.GetClients().Length,
                    add = true
                },
                users = new {
                    count = cfg.GetUsers().Length,
                    add = true,
                    authenticate = true
                }
            };

        }

        [System.Web.Mvc.HttpGet]
        public object GetAccounts(string userId) {

            AnalyticsConfig cfg = AnalyticsConfig.Current;

            AnalyticsConfigUser user = cfg.GetUserById(userId);

            if (user == null) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.NotFound, "User not found."));

            GoogleService google = GoogleService.CreateFromRefreshToken(
                user.Client.ClientId,
                user.Client.ClientSecret,
                user.RefreshToken
            );

            var response1 = google.Analytics.Management.GetAccounts(new AnalyticsGetAccountsOptions(1000));
            var response2 = google.Analytics.Management.GetWebProperties(new AnalyticsGetWebPropertiesOptions(1000));
            var response3 = google.Analytics.Management.GetProfiles(new AnalyticsGetProfilesOptions(1000));

            var accounts = response1.Body.Items;
            var webProperties = response2.Body.Items;
            var profiles = response3.Body.Items;

            var body = new Models.Api.Selector.UserModel(user, accounts, webProperties, profiles);

            return body;

        }

        [System.Web.Mvc.HttpGet]
        public object GetBlocks(int pageId) {

            IPublishedContent content = Umbraco.Content(pageId);
            if (content == null) return Request.CreateResponse(JsonMetaResponse.GetError("Page not found or not published."));

            return new {
                blocks = new [] {
                    new {
                        title = "History",
                        view = "/App_Plugins/Skybrud.Analytics/Views/Blocks/History.html",
                        periods = new [] {
                            new { name = "Yesterday", alias = "yesterday" },
                            new { name = "Last week", alias = "lastweek" },
                            new { name = "Last month", alias = "lastmonth" },
                            new { name = "Last year", alias = "lastyear" }
                        }
                    }
                }
            };

        }

        
        [System.Web.Mvc.HttpGet]
        public object GetHistory(int pageId, string period = "yesterday") {
            return GetData(pageId, period);
        }

        [System.Web.Mvc.HttpGet]
        public object GetData(int pageId, string period = "yesterday") {

            IPublishedContent content = Umbraco.Content(pageId);
            if (content == null) return Request.CreateResponse(JsonMetaResponse.GetError("Page not found or not published."));

            IPublishedContent site = GetSiteNode(content);
            if (site == null) return Request.CreateResponse(JsonMetaResponse.GetError("Unable to determine site node."));

            AnalyticsProfileSelection selection = site.Value("analyticsProfile") as AnalyticsProfileSelection;

            // Get a reference to the configuration of this package
            AnalyticsConfig config = AnalyticsConfig.Current;

            string profileId = null;
            GoogleService service = null;

            if (selection != null && selection.IsValid) {

                profileId = selection.Profile.Id;

                AnalyticsConfigUser user = config.GetUserById(selection.User.Id);

                if (user != null) {
                    service = GoogleService.CreateFromRefreshToken(
                        user.Client.ClientId,
                        user.Client.ClientSecret,
                        user.RefreshToken
                    );
                }

            }

            // Fallback to app settings (if specified)
            if (service == null && config.HasAppSettings) {

                profileId = config.AppSettings.AnalyticsProfileId;

                service = GoogleService.CreateFromRefreshToken(
                    config.AppSettings.GoogleClientId,
                    config.AppSettings.GoogleClientSecret,
                    config.AppSettings.GoogleRefreshToken
                );

            }

            if (String.IsNullOrWhiteSpace(profileId) || service == null) {
                return Request.CreateResponse(JsonMetaResponse.GetError("The Analytics package is not configured."));
            }



            AnalyticsDataMode mode = content.Id == site.Id ? AnalyticsDataMode.Site : AnalyticsDataMode.Page;

            Period p = Period.Parse(period);

            return new {
                period = p,
                page = new {
                    id = content.Id,
                    name = content.Name,
                    url = content.Url
                },
                history = GetHistory(service.Analytics, profileId, content, mode, p)
            };

        }

        private IPublishedContent GetSiteNode(IPublishedContent content) {

            IPublishedContent scope = content;

            while (scope != null) {

                // The "analyticsProfile" should only be present at the site node
                if (scope.HasValue("analyticsProfile")) return scope;

                // If checks fail, the site node is the top most node
                if (scope.Level == 1) return scope;

                scope = scope.Parent;

            }

            return null;

        }

        public enum AnalyticsDataMode {
            Site,
            Page
        }

        private object GetHistory(AnalyticsEndpoint analytics, string profileId, IPublishedContent content, AnalyticsDataMode mode, Period period) {

            // Declare the options for the request
            AnalyticsGetDataOptions options = new AnalyticsGetDataOptions {
                ProfileId = profileId,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Metrics = AnalyticsMetrics.Sessions + AnalyticsMetrics.Pageviews,
                Dimensions = AnalyticsDimensions.Hour
            };

            if (mode == AnalyticsDataMode.Page) {

                // Google Analytics sees the same URL with and without a trailing slash as two different pages, so we should tell the query to check both
                string pageUrlTrimmed = content.Url.TrimEnd('/');
                string pageUrlSlashed = pageUrlTrimmed + '/';

                options.Filters.Add(new AnalyticsDimensionFilter(AnalyticsDimensions.PagePath, AnalyticsDimensionOperator.ExactMatch, pageUrlTrimmed));
                options.Filters.Add(new AnalyticsDimensionFilter(AnalyticsDimensions.PagePath, AnalyticsDimensionOperator.ExactMatch, pageUrlSlashed));

            }
            
            if (period.Days <= 1) {
                options.Dimensions = AnalyticsDimensions.Hour;
            } else if (period.Days <= 31) {
                options.Dimensions = AnalyticsDimensions.Date;
            } else {
                options.Dimensions = AnalyticsDimensions.YearWeek;
            }

            // Get the data from the Analytics API
            AnalyticsGetDataResponse response = analytics.Data.GetData(options);

            if (response.Body.Rows.Length == 0) {
                return new {
                    columns = new object[0],
                    rows = new object[0]
                };
            }

            object ddata;

            object[] columns = new object[response.Body.ColumnHeaders.Length];
            object[] rows = new object[response.Body.Rows.Length];

            for (int i = 0; i < response.Body.ColumnHeaders.Length; i++) {
                var column = response.Body.ColumnHeaders[i];
                columns[i] = new {
                    alias = column.Name.Substring(3),
                    label = column.Name
                };
            }

            for (int i = 0; i < response.Body.Rows.Length; i++) {

                AnalyticsDataRow row = response.Body.Rows[i];

                object[] rowdata = new object[row.Cells.Length];

                for (int j = 0; j < row.Cells.Length; j++) {
                    rowdata[j] = GetCellData(row.Cells[j]);
                }

                rows[i] = rowdata;

            }

            ddata = new { columns, rows };

            var datasets = new object[] {
                new {
                    label = "Pageviews",
                    fillColor = "#35353d",
                    strokeColor = "#35353d"
                },
                new  {
                    label = "Sessions",
                    fillColor = "red",//"rgba(141, 146, 157, 1)",
                    strokeColor = "red"//"rgba(141, 146, 157, 1)"
                }
            };

            object[] items = (
                from row in response.Body.Rows
                let first = row.Cells[0]
                select new {
                    label = FormatCell(first),
                    visits = FormatInt32(AnalyticsMetrics.Sessions, row),
                    pageviews = FormatInt32(AnalyticsMetrics.Pageviews, row)
                }
            ).ToArray();

            return new {
                hasData = response.Body.Rows.Any(x => x.GetInt32(AnalyticsMetrics.Sessions) > 0),
                data = ddata,
                datasets = datasets,
                items = items.ToArray()
            };

        }
        
        private static object GetCellData(AnalyticsDataCell cell) {

            object raw;
            object value;

            switch (cell.Column.DataType) {

                case AnalyticsDataType.Integer:
                    raw = cell.GetInt32();
                    value = cell.GetInt32().ToString("N0");
                    break;

                case AnalyticsDataType.Float:
                    raw = cell.GetDouble();
                    value = cell.GetDouble().ToString("N2");
                    break;

                default:
                    raw = cell.GetString();
                    value = cell.GetString() + " (" + cell.Column.ColumnType + ")";
                    break;

            }

            return new {
                raw, value
            };

        }

        internal object FormatInt32(IAnalyticsField field, AnalyticsDataRow row) {

            string key = field.Name.Substring(3);

            int value = (row == null ? 0 : row.GetInt32(field));

            return new {
                alias = key,
                label = field.ToString(),
                value = new { raw = value, text = value.ToString() }
            };

        }

        internal object FormatCell(AnalyticsDataCell cell) {

            string key = cell.Column.Name.Substring(3);

            string text = cell.Value;

            //switch (cell.Column.Name) {

            //    case "ga:date":
            //        {
            //            DateTime date = DateTime.ParseExact(text, "yyyyMMdd", null);
            //            if (Context.Culture.TwoLetterISOLanguageName == "en")
            //            {
            //                text = date.Day + GetDaySuffix(date.Day) + " of " + date.ToString("MMM");
            //            }
            //            else
            //            {
            //                text = DateTime.ParseExact(text, "yyyyMMdd", null).ToString("d. MMM", Context.Culture);
            //            }
            //            break;
            //        }

            //    case "ga:yearWeek":
            //        text = Context.Translate("analytics_week_x", text.Substring(4));
            //        break;

            //}

            return new {
                alias = key,
                label = cell.Column.Name,
                value = new { raw = cell.Value, text }
            };

        }

}

}
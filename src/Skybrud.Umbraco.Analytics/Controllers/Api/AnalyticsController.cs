using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Skybrud.Social.Google;
using Skybrud.Social.Google.Analytics;
using Skybrud.Social.Google.Analytics.Models.Data;
using Skybrud.Social.Google.Analytics.Models.Dimensions;
using Skybrud.Social.Google.Analytics.Models.Metrics;
using Skybrud.Social.Google.Analytics.Options.Data;
using Skybrud.Social.Google.Analytics.Options.Data.Dimensions;
using Skybrud.Social.Google.Analytics.Options.Management;
using Skybrud.Social.Google.Analytics.Responses.Data;
using Skybrud.Umbraco.Analytics.Extensions;
using Skybrud.Umbraco.Analytics.Models;
using Skybrud.Umbraco.Analytics.Models.ChartJs;
using Skybrud.Umbraco.Analytics.Models.Config;
using Skybrud.Umbraco.Analytics.Models.Data.History;
using Skybrud.Umbraco.Analytics.Models.Json;
using Skybrud.Umbraco.Analytics.Models.Selection;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Analytics.Controllers.Api {

    [PluginController("Skybrud")]
    [JsonOnlyConfiguration]
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

            var response1 = google.Analytics().Management.GetAccounts(new AnalyticsGetAccountsOptions(1000));
            var response2 = google.Analytics().Management.GetWebProperties(new AnalyticsGetWebPropertiesOptions(1000));
            var response3 = google.Analytics().Management.GetProfiles(new AnalyticsGetProfilesOptions(1000));

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
                history = GetHistory(service.Analytics(), profileId, content, mode, p)
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

        private object GetHistory(AnalyticsService analytics, string profileId, IPublishedContent content, AnalyticsDataMode mode, Period period) {

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
            
            // Determine the dimension based on the length of the period
            if (period.Days <= 1) {
                options.Dimensions = AnalyticsDimensions.Hour;
            } else if (period.Days <= 31) {
                options.Dimensions = AnalyticsDimensions.Date;
            } else {
                options.Dimensions = AnalyticsDimensions.YearWeek;
            }

            // Get the data from the Analytics API
            AnalyticsGetDataResponse response = analytics.Data.GetData(options);

            // Return an empty model if there are no valid rows
            if (response.Body.Rows.All(x => x.GetInt32(AnalyticsMetrics.Sessions) == 0)) {
                return new AnalyticsHistory();
            }

            // Initialize the data sets
            ChartJsDataSet pageviews = new ChartJsDataSet("Pageviews", "#F1BFBD");
            ChartJsDataSet sessions = new ChartJsDataSet("Sessions", "#1D274E");

            // Initialize the chart data
            ChartJsData chart = new ChartJsData {
                DataSets = new List<ChartJsDataSet> {  pageviews, sessions }
            };

            // Populate the labels and data of each data set
            foreach (AnalyticsDataRow row in response.Body.Rows) {

                if (row.TryGetValue(AnalyticsDimensions.Date, out string date)) {
                    DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
                    chart.Labels.Add(dt.ToString("MMM d"));
                } else if (row.TryGetValue(AnalyticsDimensions.Hour, out string hour)) {
                    chart.Labels.Add(hour);
                } else if (row.TryGetValue(AnalyticsDimensions.YearWeek, out string yearWeek)) {
                    chart.Labels.Add("W" + Int32.Parse(yearWeek.Substring(4)));
                } else {
                    chart.Labels.Add(row.Cells[0].Value);
                }

                chart.Rows.Add(row);

                pageviews.Data.Add(row.GetString(AnalyticsMetrics.Pageviews));
                sessions.Data.Add(row.GetString(AnalyticsMetrics.Sessions));

            }

            return new AnalyticsHistory {
                Chart = chart
            };

        }

    }

}
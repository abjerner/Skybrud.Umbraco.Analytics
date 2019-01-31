using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Umbraco.Analytics.Models.Api;
using Umbraco.Core.IO;
using Umbraco.Core.Models.Membership;

namespace Skybrud.Umbraco.Analytics.Models.Config {

    public class AnalyticsConfig {

        private AnalyticsConfigElement _config;

        private readonly string _path = IOHelper.MapPath("~/Config/Skybrud.Analytics.config");

        #region Properties

        public AnalyticsConfigAppSettings AppSettings { get; }

        public bool HasAppSettings => AppSettings?.IsValid ?? false;

        public static AnalyticsConfig Current {

            get {
                
                if (HttpContext.Current == null) return new AnalyticsConfig();

                AnalyticsConfig analytics = HttpContext.Current.Items["Skybrud.AnalyticsConfig"] as AnalyticsConfig;

                if (analytics == null) {
                    HttpContext.Current.Items["Skybrud.AnalyticsConfig"] = analytics = new AnalyticsConfig();
                }

                return analytics;

            }

        }


        #endregion

        #region Constructors

        public AnalyticsConfig() {

            AppSettings = new AnalyticsConfigAppSettings();

            if (System.IO.File.Exists(_path) == false) {
                Save();
            } else {
                Load();
            }

        }

        #endregion

        #region Member methods

        internal void Load() {

            string path = IOHelper.MapPath("~/Config/Skybrud.Analytics.config");

            _config = JsonUtils.LoadJsonObject(path, AnalyticsConfigElement.Parse);

        }

        internal void Save() {

            string path = IOHelper.MapPath("~/Config/Skybrud.Analytics.config");

            _config = _config ?? new AnalyticsConfigElement();

            JsonUtils.SaveJsonObject(path, JObject.FromObject(_config), Formatting.Indented);

        }

        public AnalyticsConfigClient[] GetClients() {

            if (_config == null) throw new Exception("_clients is NULL");
            if (_config.Clients == null) throw new Exception("_clients is NULL");

            return _config.Clients.ToArray();
        }

        public string GoogleClientId => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleClientId"];

        public string GoogleClientSecret => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleClientSecret"];

        public string GoogleRefreshToken => WebConfigurationManager.AppSettings["SkybrudAnalytics.GoogleRefreshToken"];

        public string AnalyticsProfileId => WebConfigurationManager.AppSettings["SkybrudAnalytics.AnalyticsProfileId"];

        public AnalyticsConfigClient AddClient(string name, string clientId, string clientSecret, IUser user) {

            if (String.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));
            if (String.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));

            string id = Guid.NewGuid().ToString();

            AnalyticsConfigClient client = new AnalyticsConfigClient {
                Id = id,
                Name = String.IsNullOrWhiteSpace(name) ? "Untitled client" : name,
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            _config.Clients.Add(client);

            Save();

            return client;

        }

        public void DeleteClient(AnalyticsConfigClient client, IUser user) {

            if (client == null) throw new ArgumentNullException(nameof(client));

            _config.Clients.Remove(client);

            Save();

        }

        public AnalyticsConfigClient GetClientById(string id) {
            return _config.Clients.FirstOrDefault(x => x.Id == id);
        }

        public AnalyticsConfigClient SaveClient(AnalyticsConfigClient client, IUser user) {
            Save();
            return client;
        }

        #region Users

        public AnalyticsConfigUser[] GetUsers() {
            return _config.Clients.SelectMany(x => x.Users).ToArray();
        }

        public AnalyticsConfigUser GetUserById(string id) {
            return _config.Clients.SelectMany(x => x.Users).FirstOrDefault(x => x.Id == id);
        }

        public AnalyticsConfigUser AddUser(AnalyticsConfigClient client, AddUserModel.UserModel userModel, IUser user) {

            if (client == null) throw new ArgumentNullException(nameof(client));
            if (userModel == null) throw new ArgumentNullException(nameof(userModel));

            string id = Guid.NewGuid().ToString();

            AnalyticsConfigUser u = new AnalyticsConfigUser {
                Id = id,
                UserId = userModel.Id,
                Name = userModel.Name,
                Email = userModel.Email,
                RefreshToken = userModel.RefreshToken
            };

            client.Users.Add(u);

            Save();

            return u;

        }

        public AnalyticsConfigUser AddUser(AnalyticsConfigClient client, string name, string userId, string email, string refreshToken, IUser user) {

            if (client == null) throw new ArgumentNullException(nameof(client));
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            if (String.IsNullOrWhiteSpace(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            string id = Guid.NewGuid().ToString();

            AnalyticsConfigUser u = new AnalyticsConfigUser {
                Id = id,
                Name = name,
                Email = email,
                RefreshToken = refreshToken
            };

            client.Users.Add(u);

            Save();

            return u;

        }
        
        public void DeleteUser(AnalyticsConfigUser user, IUser backofficeUser) {

            if (user == null) throw new ArgumentNullException(nameof(user));

            user.Client.Users = user.Client.Users.Where(x => x != user).ToList();

            Save();

        }

        #endregion

        #endregion

    }

}
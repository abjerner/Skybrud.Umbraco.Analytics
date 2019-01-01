using System;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Google.Analytics.Scopes;
using Skybrud.Social.Google.Common;
using Skybrud.Social.Google.Common.Enums;
using Skybrud.Social.Google.Common.Models;
using Skybrud.Social.Google.Common.OAuth;
using Skybrud.Social.Google.Common.Responses;
using Skybrud.Social.Google.Common.Responses.Authentication;
using Skybrud.Social.Google.Common.Scopes;
using Skybrud.Umbraco.Analytics.Extensions;
using Skybrud.Umbraco.Analytics.Models.Config;
using Umbraco.Core.Configuration;

namespace Umbraco8.App_Plugins.Skybrud.Analytics.Dialogs {

    public partial class GoogleOAuth : System.Web.UI.Page {

        private string _state;

        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode {
            get { return Request.QueryString["code"]; }
        }

        public string AuthState {
            get { return Request.QueryString["state"]; }
        }

        public string AuthErrorReason {
            get { return Request.QueryString["error_reason"]; }
        }

        public string AuthError {
            get { return Request.QueryString["error"]; }
        }

        public string AuthErrorDescription {
            get { return Request.QueryString["error_description"]; }
        }



        public string State {
            get => _state ?? Request.QueryString["state"];
            set => _state = value;
        }

        public bool HasState => String.IsNullOrWhiteSpace(State) == false;

        public OAuthState SessionState => HasState ? Session["Skybrud.Social_" + State] as OAuthState : null;

        public bool HasSessionState => SessionState != null;

        public string Id => HasSessionState ? SessionState.Id : Request.QueryString["id"];
        public string Callback => HasSessionState ? SessionState.Callback : Request.QueryString["callback"];

        public class OAuthState {
            public string Callback { get; set; }
            public string Id { get; set; }
        }












        protected override void OnPreInit(EventArgs e) {

            base.OnPreInit(e);

        }

        private void ListClients() {

            var config = UmbracoConfig.For.SkybrudAnalytics();

            var clients = config.GetClients();

            if (clients.Length == 0) {
                Content.Text = "<p>You have not yet configured any OAuth clients.</p>";
                return;
            }

            if (clients.Length == 1) {
                Response.Redirect(Request.RawUrl + "&id=" + clients[0].Id);
                return;
            }

            foreach (var client in clients) {

                string url = Request.RawUrl + "&id=" + client.Id;

                Content.Text += "<p><a href=\"" + url + "\">" + client.Name + "</a></p>";
            }


        }

        protected void Page_Load(object sender, EventArgs e) {

            if (HasState && SessionState == null) {
                Content.Text = "<div class=\"error\">Session expired?</div>";
                return;
            }

            if (String.IsNullOrWhiteSpace(Id) && SessionState == null) {
                ListClients();
                return;
            }

            AnalyticsConfigClient client = client = UmbracoConfig.For.SkybrudAnalytics().GetClientById(Id);

            if (client == null) {
                Content.Text += "<p>WTF?</p>";
                return;
            }

            // Configure the OAuth client based on the options of the prevalue options
            GoogleOAuthClient oauth = new GoogleOAuthClient {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                RedirectUri = Request.Url.ToString().Split('?')[0]
            };
            
            // Redirect the user to the Google login dialog
            if (AuthCode == null) {

                // Generate a new unique/random state
                State = Guid.NewGuid().ToString();

                // Save the state in the current user session
                Session["Skybrud.Social_" + State] = new OAuthState {
                    Callback = Callback,
                    Id = Id
                };

                // Declare the scope
                GoogleScopeCollection scope = new[] {
                    GoogleScopes.Email,
                    GoogleScopes.Profile,
                    AnalyticsScopes.Readonly
                };

                // Construct the authorization URL
                string url = oauth.GetAuthorizationUrl(State, scope, GoogleAccessType.Offline, GoogleApprovalPrompt.Force);

                // Redirect the user
                Response.Redirect(url);
                return;

            }
            

            GoogleTokenResponse info;
            try {
                info = oauth.GetAccessTokenFromAuthorizationCode(AuthCode);
            } catch (Exception ex) {
                Content.Text = "<div class=\"error\"><b>Unable to acquire access token</b><br />" + ex.Message + "</div>";
                return;
            }

            try {

                // Initialize the Google service
                GoogleService service = GoogleService.CreateFromRefreshToken(client.ClientId, client.ClientSecret, info.Body.RefreshToken);
                
                // Get information about the authenticated user
                GoogleGetUserInfoResponse userResponse = service.GetUserInfo();

                var user = userResponse.Body;
                
                Content.Text += "<p>Hi <strong>" + user.Name + "</strong></p>";
                Content.Text += "<p>Please wait while you're being redirected...</p>";

                // Set the callback data
                var data = new {
                    id = Guid.NewGuid().ToString(),
                    userId = user.Id,
                    name = user.Name,
                    email = user.Email,
                    avatar = user.Picture,
                    clientId = client.ClientId,
                    clientSecret = client.ClientSecret,
                    refreshToken = info.Body.RefreshToken
                };

                // Update the UI and close the popup window
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "callback", String.Format(
                    "self.opener." + Callback + "({0}); window.close();",
                    JObject.FromObject(data)
                ), true);

            } catch (Exception ex) {
                Content.Text += "<div class=\"error\"><b>Unable to get user information</b><br />" + ex.Message + "</div>";
                return;
            }







            //Title = "Google OAuth";

            //Callback = Request.QueryString["callback"];
            //ContentTypeAlias = Request.QueryString["contentTypeAlias"];
            //PropertyAlias = Request.QueryString["propertyAlias"];
            //Feature = Request.QueryString["feature"];

            //if (AuthState != null) {
            //    NameValueCollection stateValue = Session["Skybrud.Social_" + AuthState] as NameValueCollection;
            //    if (stateValue != null) {
            //        Callback = stateValue["Callback"];
            //        ContentTypeAlias = stateValue["ContentTypeAlias"];
            //        PropertyAlias = stateValue["PropertyAlias"];
            //        Feature = stateValue["Feature"];
            //    }
            //}

            //// Get the prevalue options
            //GoogleOAuthPreValueOptions options = GoogleOAuthPreValueOptions.Get(ContentTypeAlias, PropertyAlias);
            //if (!options.IsValid) {
            //    Content.Text += "Hold on now! The options of the underlying prevalue editor isn't valid.";
            //    return;
            //}

            //// Configure the OAuth client based on the options of the prevalue options
            //GoogleOAuthClient client = new GoogleOAuthClient {
            //    ClientId = options.ClientId,
            //    ClientSecret = options.ClientSecret,
            //    RedirectUri = options.RedirectUri
            //};

            //// Session expired?
            //if (AuthState != null && Session["Skybrud.Social_" + AuthState] == null) {
            //    Content.Text = "<div class=\"error\">Session expired?</div>";
            //    return;
            //}

            //// Check whether an error response was received from Google
            //if (AuthError != null) {
            //    Content.Text = "<div class=\"error\">Error: " + AuthErrorDescription + "</div>";
            //    if (AuthState != null) Session.Remove("Skybrud.Social:" + AuthState);
            //    return;
            //}

            //string state;

            //// Redirect the user to the Google login dialog
            //if (AuthCode == null) {

            //    // Generate a new unique/random state
            //    state = Guid.NewGuid().ToString();

            //    // Save the state in the current user session
            //    Session["Skybrud.Social_" + state] = new NameValueCollection {
            //        { "Callback", Callback},
            //        { "ContentTypeAlias", ContentTypeAlias},
            //        { "PropertyAlias", PropertyAlias},
            //        { "Feature", Feature}
            //    };

            //    // Declare the scope
            //    GoogleScopeCollection scope = new[] {
            //        GoogleScope.OpenId,
            //        GoogleScope.Email,
            //        GoogleScope.Profile
            //    };

            //    // Construct the authorization URL
            //    string url = client.GetAuthorizationUrl(state, scope, GoogleAccessType.Offline, GoogleApprovalPrompt.Force);

            //    // Redirect the user
            //    Response.Redirect(url);
            //    return;

            //}

            //GoogleAccessTokenResponse info;
            //try {
            //    info = client.GetAccessTokenFromAuthorizationCode(AuthCode);
            //} catch (Exception ex) {
            //    Content.Text = "<div class=\"error\"><b>Unable to acquire access token</b><br />" + ex.Message + "</div>";
            //    return;
            //}

            //try {

            //    // Initialize the Google service
            //    GoogleService service = GoogleService.CreateFromRefreshToken(client.ClientIdFull, client.ClientSecret, info.RefreshToken);

            //    // Get information about the authenticated user
            //    GoogleUserInfo user = service.GetUserInfo();

            //    Content.Text += "<p>Hi <strong>" + user.Name + "</strong></p>";
            //    Content.Text += "<p>Please wait while you're being redirected...</p>";

            //    // Set the callback data
            //    GoogleOAuthData data = new GoogleOAuthData {
            //        Id = user.Id,
            //        Name = user.Name,
            //        Avatar = user.Picture,
            //        ClientId = client.ClientIdFull,
            //        ClientSecret = client.ClientSecret,
            //        RefreshToken = info.RefreshToken
            //    };

            //    // Update the UI and close the popup window
            //    Page.ClientScript.RegisterClientScriptBlock(GetType(), "callback", String.Format(
            //        "self.opener." + Callback + "({0}); window.close();",
            //        data.Serialize()
            //    ), true);

            //} catch (Exception ex) {
            //    Content.Text = "<div class=\"error\"><b>Unable to get user information</b><br />" + ex.Message + "</div>";
            //    return;
            //}

        }

    }

}
using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Skybrud.Social.Google.Analytics.Scopes;
using Skybrud.Social.Google.Common;
using Skybrud.Social.Google.Common.Enums;
using Skybrud.Social.Google.Common.Models;
using Skybrud.Social.Google.Common.OAuth;
using Skybrud.Social.Google.Common.Responses;
using Skybrud.Social.Google.Common.Responses.Authentication;
using Skybrud.Social.Google.Common.Scopes;
using Skybrud.Umbraco.Analytics.Models.Config;
using Umbraco.Web.Mvc;
using Umbraco.Web.Security;

namespace Skybrud.Umbraco.Analytics.Controllers.Surface {

    [PluginController("Skybrud")]
    public class AnalyticsOAuthController : SurfaceController {

        #region Properties

        private string _state;

        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode => Request.QueryString["code"];

        public string AuthState => Request.QueryString["state"];

        public string AuthErrorReason => Request.QueryString["error_reason"];

        public string AuthError => Request.QueryString["error"];

        public bool HasAuthError => String.IsNullOrWhiteSpace(AuthError) == false;

        public string AuthErrorDescription => Request.QueryString["error_description"];

        public string State {
            get => _state ?? Request.QueryString["state"];
            set => _state = value;
        }

        public bool HasState => String.IsNullOrWhiteSpace(State) == false;

        public OAuthState SessionState => HasState ? Session["Skybrud.Social_" + State] as OAuthState : null;

        public bool HasSessionState => SessionState != null;

        public string Id => HasSessionState ? SessionState.Id : Request.QueryString["id"];

        public string Callback => HasSessionState ? SessionState.Callback : Request.QueryString["callback"];

        #endregion

        public ActionResult Authenticate() {

            // Did we receive an error callback from Google?
            if (HasAuthError) {
                return Error("Authentication failed", "The authentication with Google Analytics failed. Close this window and try again.");
            }

            // Handle Umbraco authentication stuff
            HttpContextWrapper http = new HttpContextWrapper(System.Web.HttpContext.Current);
            AuthenticationTicket ticket = http.GetUmbracoAuthTicket();

            // User must be logged in
            if (http.AuthenticateCurrentRequest(ticket, true) == false) {
                return Error("Not logged in", "You must be logged in to Umbraco to access this page. Close this window and try again.");
            }

            // Do we have a valid session?
            if (HasState && SessionState == null) {
                return Error("Session expired?", "It seems that your browser session has expired. You can try to close this window and try again.");
            }

            // Get the specified OAuth client
            AnalyticsConfigClient client = AnalyticsConfig.Current.GetClientById(Id);
            if (client == null) return Error("Client not found", "An OAuth client matching the specified ID could not be found.");
            
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
                return Redirect(url);

            }
            
            // Exchange the authorization code with a new access token (and refresh token)
            GoogleTokenResponse info;
            try {
                info = oauth.GetAccessTokenFromAuthorizationCode(AuthCode);
            } catch (Exception ex) {
                return Error("Authentication failed", "Unable to acquire access token.", ex);
            }

            try {

                // Initialize the Google service
                GoogleService service = GoogleService.CreateFromRefreshToken(client.ClientId, client.ClientSecret, info.Body.RefreshToken);
                
                // Get information about the authenticated user
                GoogleGetUserInfoResponse userResponse = service.GetUserInfo();

                // The the response body
                GoogleUserInfo user = userResponse.Body;
                
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

                return View("~/App_Plugins/Skybrud.Analytics/Razor/AuthenticateSuccess.cshtml", new AnalyticsAuthenticatedPageModel {
                    Title = "Auhentication",
                    User = user,
                    Callback = Callback,
                    CallbackData = data
                });

            } catch (Exception ex) {

                return Error("Authenticated failed", "Unable to get user information.", ex);

            }

        }

        private ActionResult Error(string title, string message, Exception exception = null) {
            return View("~/App_Plugins/Skybrud.Analytics/Razor/AuthenticateError.cshtml", new AnalyticsErrorPageModel {
                Title = title,
                Message = message,
                Exception = exception
            });
        }

        public class AnalyticsPageModel {
            
            public string Title { get; set; }

        }

        public class AnalyticsErrorPageModel : AnalyticsPageModel {
            
            public string Message { get; set; }

            public Exception Exception { get; set; }

        }

        public class AnalyticsAuthenticatedPageModel : AnalyticsPageModel {
            
            public GoogleUserInfo User { get; set; }

            public string Callback { get; set; }

            public object CallbackData { get; set; }

        }

        public class OAuthState {

            public string Callback { get; set; }

            public string Id { get; set; }

        }

    }

}
angular.module("umbraco.services").config(function ($httpProvider) {

    $httpProvider.interceptors.push(function ($q) {
        return {
            "request": function (request) {

                let url = request.url.split("?");

                switch (url[0].toLowerCase()) {

                    case "views/skybrud.analytics/overview.html":
                        request.url = "/App_Plugins/Skybrud.Analytics/Views/Trees/Overview.html" + (url.length > 0 ? "?" + url[1] : "");
                        break;

                    case "views/skybrud.analytics/edit.html":
                        request.url = "/App_Plugins/Skybrud.Analytics/Views/Trees/Edit.html" + (url.length > 0 ? "?" + url[1] : "");
                        break;

                    case "views/skybrud.analytics/oauth.html":
                        request.url = "/App_Plugins/Skybrud.Analytics/Views/Trees/OAuth.html" + (url.length > 0 ? "?" + url[1] : "");
                        break;

                }

                return request || $q.when(request);
            }
        };
    });

});


angular.module("umbraco").controller("hejsa", function ($scope, editorState, userService, $http, analyticsService) {

    // Define an alias for the editor (eg. used for callbacks)
    var alias = $scope.alias = ("skybrudanalytics_" + Math.random()).replace(".", "");

    $scope.clients = [];
    $scope.mode = "select";

    analyticsService.getClients().then(function(res) {
        $scope.clients = res.data;
    });

    $scope.selectClient = function (client) {

        //$scope.mode = "authenticate";
        $scope.model.client = client;

        var url = "/App_Plugins/Skybrud.Analytics/Dialogs/GoogleOAuth.aspx?callback=" + alias + "&id=" + client.id;
        window.open(url, "Google OAuth", "scrollbars=no,resizable=yes,menubar=no,width=800,height=600");

    };
    
    window[alias] = function(user) {
        $scope.model.user = user;
        $scope.mode = "authenticated";
        $scope.model.disableSubmitButton = user === null;
    }

});



angular.module("umbraco.services").factory("analyticsService", function($http) {

    return {

        getClients: function() {
            return $http.get("/umbraco/backoffice/Skybrud/Analytics/GetClients");
        },

        addClient: function (client) {
            return $http.post("/umbraco/backoffice/Skybrud/Analytics/AddClient", client);
        },

        deleteClient: function (client) {
            var id = typeof(client) === "string" ? client : client.id;
            return $http.delete("/umbraco/backoffice/Skybrud/Analytics/DeleteClient?id=" + id);
        },

        saveClient: function (client) {
            return $http.post("/umbraco/backoffice/Skybrud/Analytics/SaveClient", client);
        },

        getUsers: function () {
            return $http.get("/umbraco/backoffice/Skybrud/Analytics/GetUsers");
        },

        addUser: function (client, user) {
            return $http.post("/umbraco/backoffice/Skybrud/Analytics/AddUser", {
                client: client,
                user: user
            });
        },

        deleteUser: function (user) {
            var id = typeof (user) === "string" ? user : user.id;
            return $http.delete("/umbraco/backoffice/Skybrud/Analytics/DeleteUser?id=" + id);
        }

    };

});

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

angular.module("umbraco.services").factory("analyticsService", function($http) {

    return {

        getStatus: function () {
            return $http.get("/umbraco/backoffice/Skybrud/Analytics/GetStatus");
        },

        getAccounts: function (userId) {
            return $http.get("/umbraco/backoffice/Skybrud/Analytics/GetAccounts?userId=" + userId);
        },

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

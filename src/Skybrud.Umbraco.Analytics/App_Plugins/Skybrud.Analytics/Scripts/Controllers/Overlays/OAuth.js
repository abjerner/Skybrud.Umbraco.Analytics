angular.module("umbraco").controller("Skybrud.Analytics.OAuthOverlay", function ($scope, editorState, userService, $http, analyticsService) {

    // Define an alias for the editor (eg. used for callbacks)
    var alias = $scope.alias = ("skybrudanalytics_" + Math.random()).replace(".", "");

    $scope.clients = [];
    $scope.mode = "select";

    analyticsService.getClients().then(function (res) {
        $scope.clients = res.data;
    });

    $scope.selectClient = function (client) {

        //$scope.mode = "authenticate";
        $scope.model.client = client;

        var url = "/umbraco/Skybrud/AnalyticsOAuth/Authenticate?callback=" + alias + "&id=" + client.id;
        window.open(url, "Google OAuth", "scrollbars=no,resizable=yes,menubar=no,width=800,height=700");

    };

    window[alias] = function (user) {
        $scope.$apply(function () {
            $scope.model.user = user;
            $scope.mode = "authenticated";
            $scope.model.disableSubmitButton = user === null;
        });
    }

});
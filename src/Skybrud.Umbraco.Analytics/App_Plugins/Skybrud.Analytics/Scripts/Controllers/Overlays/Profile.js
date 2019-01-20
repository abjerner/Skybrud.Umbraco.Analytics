angular.module("umbraco").controller("Skybrud.Analytics.Overlays.ProfileSelector", function ($scope, editorState, userService, $http, analyticsService, editorService) {

    $scope.profile = null;

    $scope.users = null;

    analyticsService.getUsers().then(function(res) {
        $scope.users = res.data;
        angular.forEach($scope.users, function(user) {
            loadAccounts(user);
        });
    });

    function loadAccounts(user) {

        analyticsService.getAccounts(user.id).then(function(res) {
            user.accounts = res.data.accounts;
        });

    }

});
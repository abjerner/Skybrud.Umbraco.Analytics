angular.module("umbraco").directive("skybrudAnalyticsUsers", function ($timeout, $http, editorState, notificationsService, analyticsService, overlayService) {
    return {
        restrict: "EA",
        scope: {},
        templateUrl: "/App_Plugins/Skybrud.Analytics/Views/Users.html",
        controller: ["$scope", "$element", function ($scope, $element) {



            // Get a reference to the current editor state
            var state = editorState.current;

            $scope.callback = function (data) {

                console.log(data);

                //$scope.$apply(function () {
                //    $scope.model.value = data;
                //});
            };

            $scope.users = [];

            $scope.overlay = null;
            
            $scope.delete = function (user) {


                //analyticsService.deleteUser(user).then(function () {
                //    notificationsService.success("Skybrud.Analytics", "The user was successfully deleted.");
                //    $scope.updateUsers();
                //});
            };

            $scope.add = function () {
                $scope.overlay = {
                    view: "/App_Plugins/Skybrud.Analytics/Views/Overlays/OAuth.html",
                    title: "Authenticate with Google and Analytics",
                    submitButtonLabel: "Continue",
                    disableSubmitButton: true,
                    show: true,
                    submit: function (model) {
                        $scope.overlay.state = "busy";
                        analyticsService.addUser(model.client, model.user).then(function (res) {
                            $scope.clients = res.data;
                            $scope.updateUsers();
                            $scope.overlay.show = false;
                            $scope.overlay = null;
                        });
                    }
                };
            };
            
            $scope.updateUsers = function() {
                analyticsService.getUsers().then(function (res) {
                    $scope.users = res.data;
                });
            };

            $scope.updateUsers();

        }]
    };
});
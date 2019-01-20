angular.module("umbraco").controller("Skybrud.Analytics.ProfileSelector", function ($scope, editorState, $http, analyticsService, editorService) {

    $scope.status = {};

    analyticsService.getStatus().then(function(res) {
        $scope.status = res.data;
    });

    $scope.select = function () {

        editorService.open({
            title: "Select Analytics profile",
            view: "/App_Plugins/Skybrud.Analytics/Views/Overlays/Profile.html",
            submit: function(user, account, webProperty, profile) {
                $scope.model.value = {
                    user: {
                        id: user.id
                    },
                    account: {
                        id: account.id,
                        name: account.name
                    },
                    webProperty: {
                        id: webProperty.id,
                        name: webProperty.name
                    },
                    profile: {
                        id: profile.id,
                        name: profile.name,
                        currency: profile.currency,
                        timezone: profile.timezone,
                        websiteUrl: profile.websiteUrl,
                        type: profile.type,
                        created: profile.created,
                        updated: profile.updated
                    }
                };
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        });

    };

    $scope.reset = function() {
        $scope.model.value = null;
    };

});
angular.module("umbraco").directive("skybrudAnalyticsClients", function ($timeout, $http, notificationsService, analyticsService) {
    return {
        restrict: "EA",
        templateUrl: "/App_Plugins/Skybrud.Analytics/Views/Clients.html",
        controller: ["$scope", "$element", function ($scope, $element) {

            $scope.clients = [];

            $scope.overlay = null;

            function toObject(properties) {

                var temp = {};

                properties.forEach(function(p) {
                    temp[p.alias] = p.value;
                });

                return temp;

            }

            $scope.delete = function(client) {

                analyticsService.deleteClient(client).then(function () {
                    notificationsService.success("Skybrud.Analytics", "The OAuth client was successfully delete.");
                    $scope.updateClients();
                });

            };

            $scope.add = function () {

                $scope.overlay = {
                    view: "/App_Plugins/Skybrud.Analytics/Views/Overlays/Properties.html",
                    title: "Add OAuth client",
                    submitButtonLabel: "Add",
                    show: true,
                    properties: [
                        {
                            alias: "name",
                            label: "Name",
                            description: "Specify a name for the app that will help you identify it once added to Umbraco.",
                            view: "textbox"
                        },
                        {
                            alias: "clientId",
                            label: "Client ID",
                            description: "The ID of your Google app/client.",
                            view: "textbox",
                            validation: {
                                mandatory: true
                            }
                        },
                        {
                            alias: "clientSecret",
                            label: "Client Secret",
                            description: "The secret of your Google app/client.",
                            view: "textbox",
                            validation: {
                                mandatory: true
                            }
                        }
                    ],
                    submit: function (model) {

                        var client = toObject(model.properties);

                        $scope.overlay.submitButtonState = "busy";

                        analyticsService.addClient(client).then(function () {
                            notificationsService.success("Skybrud.Analytics", "The OAuth client was successfully added.");
                            $scope.updateClients();


                            // Make sure we close the overlay again
                            $scope.overlay.show = false;
                            $scope.overlay = null;

                        });


                    }
                };

            };

            $scope.edit = function (client) {

                $scope.overlay = {
                    view: "/App_Plugins/Skybrud.Analytics/Views/Overlays/Properties.html",
                    title: "Edit OAuth client",
                    submitButtonLabel: "Save",
                    show: true,
                    properties: [
                        {
                            alias: "id",
                            label: "Id",
                            view: "readonlyvalue",
                            value: client.id
                        },
                        {
                            alias: "name",
                            label: "Name",
                            description: "Specify a name for the app that will help you identify it once added to Umbraco.",
                            view: "textbox",
                            value: client.name
                        },
                        {
                            alias: "clientId",
                            label: "Client ID",
                            description: "The ID of your Google app/client.",
                            view: "textbox",
                            value: client.clientId,
                            validation: {
                                mandatory: true
                            }
                        },
                        {
                            alias: "clientSecret",
                            label: "Client Secret",
                            description: "The secret of your Google app/client.",
                            view: "textbox",
                            value: client.clientSecret,
                            validation: {
                                mandatory: true
                            }
                        }
                    ],
                    submit: function (model) {

                        var client = toObject(model.properties);

                        $scope.overlay.submitButtonState = "busy";

                        analyticsService.saveClient(client).then(function () {
                            notificationsService.success("Skybrud.Analytics", "The OAuth client was successfully updated.");
                            $scope.updateClients();


                            // Make sure we close the overlay again
                            $scope.overlay.show = false;
                            $scope.overlay = null;

                        });


                    }
                };

            };

            $scope.updateClients = function() {
                analyticsService.getClients().then(function (res) {
                    $scope.clients = res.data;
                });
            };

            $scope.updateClients();

        }]
    };
});
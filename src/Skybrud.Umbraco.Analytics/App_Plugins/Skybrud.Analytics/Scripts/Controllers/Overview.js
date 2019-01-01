angular.module("umbraco").controller("Skybrud.Analytics.Overview", function ($scope, editorState, userService, notificationsService, $http, analyticsService) {

    $scope.clients = [];
    $scope.users = [];

    $scope.navigation = [
        {
            alias: "overview",
            name: "Overview",
            icon: "icon-chart-curve",
            view: "/App_Plugins/path/to/html.html",
            active: true            
        },
        {
            alias: "settings",
            name: "Settings",
            icon: "icon-settings",
            view: "/App_Plugins/path/to/html.html"
        }
    ];


    function propertiesToObject(properties) {
        var temp = {};
        properties.forEach(function (p) {
            temp[p.alias] = p.value;
        });
        return temp;
    }

    $scope.deleteClient = function (client) {
        analyticsService.deleteClient(client).then(function () {
            notificationsService.success("Skybrud.Analytics", "The OAuth client was successfully delete.");
            $scope.updateClients();
            $scope.updateUsers();
        });
    };
    
    $scope.addClient = function () {
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
                const client = propertiesToObject(model.properties);
                $scope.overlay.submitButtonState = "busy";
                analyticsService.addClient(client).then(function () {
                    notificationsService.success("Skybrud.Analytics", "The OAuth client was successfully added.");
                    $scope.updateClients();
                    $scope.overlay.show = false;
                    $scope.overlay = null;
                });
            }
        };
    };

    $scope.editClient = function (client) {
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
                const client = propertiesToObject(model.properties);
                $scope.overlay.submitButtonState = "busy";
                analyticsService.saveClient(client).then(function () {
                    notificationsService.success("Skybrud.Analytics", "The OAuth client was successfully updated.");
                    $scope.updateClients();
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

    $scope.deleteUser = function (user) {
        analyticsService.deleteUser(user).then(function () {
            notificationsService.success("Skybrud.Analytics", "The user was successfully deleted.");
            $scope.updateUsers();
        });
    };

    $scope.addUser = function () {
        $scope.overlay = {
            view: "/App_Plugins/Skybrud.Analytics/Views/Overlays/OAuth.html",
            title: "Authenticate with Google and Analytics",
            submitButtonLabel: "Continue",
            disableSubmitButton: true,
            show: true,
            submit: function (model) {
                $scope.overlay.state = "busy";
                analyticsService.addUser(model.client, model.user).then(function (res) {
                    notificationsService.success("Skybrud.Analytics", "The user was successfully added.");
                    $scope.updateUsers();
                    $scope.overlay.show = false;
                    $scope.overlay = null;
                });
            }
        };
    };

    $scope.updateUsers = function () {
        analyticsService.getUsers().then(function (res) {
            $scope.users = res.data;
        });
    };

    $scope.updateClients();
    $scope.updateUsers();

});
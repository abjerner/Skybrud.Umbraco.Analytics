angular.module("umbraco").controller("Skybrud.Analytics.History", function ($scope, editorState, userService, $http) {

    var vm = this;

    vm.block = $scope.block;
    vm.data = null;
    vm.period = "yesterday";
    vm.loading = false;

    vm.load = function (period) {

        vm.loading = true;

        vm.period = period ? period : "yesterday";

        $http.get("/umbraco/backoffice/Skybrud/Analytics/GetHistory?pageId=" + editorState.current.id + "&period=" + vm.period).then(function (res) {
            vm.loading = false;
            vm.data = res.data;
            vm.error = null;
        }, function (res) {
            vm.data = null;
            if (res.data && res.data.meta) {
                vm.error = {
                    message: res.data.meta.error
                };
            } else {
                vm.error = {
                    message: "An error occured."
                };
            }
        });

    };

    vm.load();

});
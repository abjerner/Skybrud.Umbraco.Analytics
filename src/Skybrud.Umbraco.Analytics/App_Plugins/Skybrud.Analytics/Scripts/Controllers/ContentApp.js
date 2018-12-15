angular.module("umbraco").controller("Skybrud.Analytics.ContentApp", function ($scope, editorState, userService, $http) {

    var vm = this;

    vm.CurrentNodeId = editorState.current.id;
    vm.CurrentNodeAlias = editorState.current.contentTypeAlias;

    var user = userService.getCurrentUser().then(function (user) {
        //console.log(user);
        vm.UserName = user.name;
    });

    vm.blocks = null;

    vm.getBlocks = function() {
        $http.get("/umbraco/backoffice/Skybrud/Analytics/GetBlocks?pageId=" + editorState.current.id).then(function (res) {
            vm.blocks = res.data.blocks;
        });
    };

    vm.getBlocks();

});
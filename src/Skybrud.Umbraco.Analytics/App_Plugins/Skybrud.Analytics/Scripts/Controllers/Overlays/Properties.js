angular.module("umbraco").controller("Skybrud.Analytics.Overlays.Properties", function ($scope, formHelper, editorService) {

    var modelCopy = {};

    //function makeModelCopy(object) {

    //    var newObject = {};

    //    for (var key in object) {
    //        if (key !== "event") {
    //            newObject[key] = angular.copy(object[key]);
    //        }
    //    }

    //    return newObject;

    //}

    function activate() {

        //modelCopy = makeModelCopy($scope.model);

    }

    $scope.submitForm = function(model) {
        if ($scope.model.submit) {
            if (formHelper.submitForm({ scope: $scope })) {
                formHelper.resetForm({ scope: $scope });
                $scope.model.submit(model/*, modelCopy*/);
            }
        } else {
            editorService.close();
        }
    };

    $scope.close = function() {
        if ($scope.model.close) {
            $scope.model.close($scope.model/*, modelCopy*/);
        } else {
            editorService.close();
        }
    };

    activate();

});
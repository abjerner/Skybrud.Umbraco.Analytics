angular.module("umbraco").directive('dashboardChart', function () {
    return {
        restrict: 'EA',
        templateUrl: '/App_Plugins/Skybrud.Analytics/Views/Blocks/Chart.html',
        controller: function ($scope, $element) {

            var chart = $(".sky-graph", $element).text("");

            $scope.width = chart.width;

            function init() {

                var history = $scope.vm.data ? $scope.vm.data.history : null;

                chart.text("");

                if (history == null) return;

                var canvas = $('<canvas width="' + (chart.width() - 150) + '" height="200"></canvas>').appendTo(chart);

                var ctx = canvas.get(0).getContext("2d");

                var c = new Chart(ctx).Line(history.chart, {
                    bezierCurve: false,
                    scaleFontSize: 10,
                    scaleFontColor: '#000',
                    pointDotRadius: 0,
                    showTooltips: true
                });

                var legend = c.generateLegend();

                var d = $("<div/>").appendTo(chart);

                $(legend).appendTo(d);

            }

            $scope.$watch("vm.data", function () {
                init();
            });

            $scope.initialized = false;
            $scope.$watch(function () {
                return $element.is(":visible");
            }, function (newValue) {
                if (!newValue) return;
                if ($scope.initialized) return;
                $scope.initialized = true;
                init();
            });

        }
    };
});
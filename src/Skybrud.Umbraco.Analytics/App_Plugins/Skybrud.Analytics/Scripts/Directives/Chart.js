angular.module("umbraco").directive('dashboardChart', function ($timeout) {
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


            // Hack: Listen for tab changes so we can lazy load the property editor
            angular.element(document.querySelectorAll(".umb-sub-views-nav-item[data-element='sub-view-skybrud-analytics']")).bind("click", function () {

                $timeout(function () {
                    if ($element.is(':visible')) {
                        init();
                    }
                }, 20);

            });




            //var t = setInterval(function() {

            //    var w = chart.width();

            //    if (w > 0) {
            //        clearInterval(t);
            //        console.log("clear t");
            //        init($scope.vm.data ? $scope.vm.data.history : null);
            //    }

            //}, 100);

            //window.hest = function() {


            //    init($scope.vm.data ? $scope.vm.data.history : null);
            //};

            //chart[0].addEventListener("resize", function () {
            //    console.log("heeeeeeeeeeeeeeeeeeeeeejsa");
            //});

        }
    };
});
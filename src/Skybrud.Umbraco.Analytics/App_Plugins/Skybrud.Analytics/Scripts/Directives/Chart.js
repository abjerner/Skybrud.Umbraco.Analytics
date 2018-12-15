angular.module("umbraco").directive('dashboardChart', ['$timeout', function ($timeout) {
    return {
        restrict: 'EA',
        templateUrl: '/App_Plugins/Skybrud.Analytics/Views/Blocks/Chart.html',
        controller: ['$scope', '$element', function ($scope, $element) {

            var chart = $(".sky-graph", $element).text("");

            $scope.width = chart.width;

            function init() {

                var history = $scope.vm.data ? $scope.vm.data.history : null;

                chart.text("");

                if (history == null) return;

                var canvas = $('<canvas width="' + (chart.width() - 150) + '" height="200"></canvas>').appendTo(chart);

                var visits = {
                    label: history.datasets[1].label,
                    fillColor: "rgba(65, 73, 92, 0.5)",
                    strokeColor: "rgb(65, 73, 92)",
                    pointColor: "rgb(65, 73, 92)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgb(65, 73, 92)",
                    data: []
                };

                var pageviews = {
                    label: history.datasets[0].label,
                    fillColor: "rgba(141, 146, 157, 0.2)",
                    strokeColor: "rgb(141, 146, 157)",
                    pointColor: "rgb(141, 146, 157)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgb(141, 146, 157)",
                    data: []
                };

                pageviews.strokeColor = 'rgba(141, 146, 157, 1)';
                pageviews.fillColor = 'rgba(141, 146, 157, 1)';

                visits.strokeColor = 'rgba(65, 73, 92, 1)';
                visits.fillColor = 'rgba(65, 73, 92, 1)';

                
                pageviews.strokeColor = "#31B0A2";
                pageviews.fillColor = "#31B0A2";

                visits.strokeColor = "#413658";
                visits.fillColor = "#413658";

                var data = {
                    labels: [],
                    datasets: [pageviews, visits]
                };

                $.each(history.items, function (i, row) {
                    data.labels.push(row.label.value.text);
                    pageviews.data.push(row.pageviews.value.raw + "");
                    visits.data.push(row.visits.value.raw + "");
                });

                var ctx = canvas.get(0).getContext("2d");

                var c = new Chart(ctx).Line(data, {
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

        }]
    };
}]);
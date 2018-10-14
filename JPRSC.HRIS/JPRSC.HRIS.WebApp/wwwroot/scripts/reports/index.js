(function () {
    angular
        .module('app')
        .controller('ReportIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', ReportIndexCtrl]);

    function ReportIndexCtrl($http, $scope, $timeout, $uibModal) {
        var vm = this;
        vm.clients = [];
        vm.generateClicked = generateClicked;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
        });

        function generateClicked(reportType) {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'generateReportModal.html',
                controller: 'GenerateReportModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clients,
                            reportType: reportType
                        }
                    }
                }
            });

            modalInstance.result.then(function (result) {

            }, function () {

            });
        };
    };
}());
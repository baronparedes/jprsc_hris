(function () {
    angular
        .module('app')
        .controller('ReportIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', ReportIndexCtrl]);

    function ReportIndexCtrl($http, $scope, $timeout, $uibModal) {
        var vm = this;
        vm.clients = [];
        vm.generateClicked = generateClicked;
        vm.generateSingleLoanTypeClicked = generateSingleLoanTypeClicked;
        vm.loanTypes = [];

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.loanTypes = vm.serverModel.loanTypes;
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

        function generateSingleLoanTypeClicked(loanType) {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'generateSingleLoanTypeReportModal.html',
                controller: 'GenerateSingleLoanTypeReportModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clients,
                            loanType: loanType
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
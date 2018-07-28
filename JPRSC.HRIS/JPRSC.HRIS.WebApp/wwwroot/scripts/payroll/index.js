(function () {
    angular
        .module('app')
        .controller('PayrollIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', 'globalSettings', PayrollIndexCtrl]);

    function PayrollIndexCtrl($http, $scope, $timeout, $uibModal, globalSettings) {
        var vm = this;
        vm.endProcessClicked = endProcessClicked;
        vm.payrollProcessBatches = [];
        vm.processClicked = processClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
        });

        searchClicked();

        function endProcessClicked(payrollProcessBatch) {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'endProcessModal.html',
                controller: 'EndProcessModalCtrl',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    params: function () {
                        return {
                            payrollProcessBatch: payrollProcessBatch
                        }
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();
            }, function () {
                searchClicked();
            });
        };

        function processClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'addPayrollModal.html',
                controller: 'AddPayrollModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clients
                        }
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();
            }, function () {
                searchClicked();
            });
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Payroll/Search', { params: vm.searchModel }).then(function (response) {
                vm.payrollProcessBatches = response.data.payrollProcessBatches;
                vm.searchInProgress = false;
            });
        };
    };
}());
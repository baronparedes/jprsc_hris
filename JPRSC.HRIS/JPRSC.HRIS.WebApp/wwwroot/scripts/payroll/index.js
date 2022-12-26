(function () {
    angular
        .module('app')
        .controller('PayrollIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', 'globalSettings', PayrollIndexCtrl]);

    function PayrollIndexCtrl($http, $scope, $timeout, $uibModal, globalSettings) {
        var vm = this;
        vm.clientChanged = clientChanged;
        vm.lastPageNumber = 1;
        vm.nextClicked = nextClicked;
        vm.payrollPeriodMonthChanged = payrollPeriodMonthChanged;
        vm.payrollPeriodYearChanged = payrollPeriodYearChanged;
        vm.payrollProcessBatches = [];
        vm.previousClicked = previousClicked;
        vm.processClicked = processClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1, payrollPeriodMonth: '-1', payrollPeriodYear: '-1' };
        vm.searchInProgress = false;
        vm.sendPayslipClicked = sendPayslipClicked;
        vm.totalResultsCount = 0;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { id: null, code: 'All Clients' });
            vm.searchModel.client = vm.clients[0];
        });

        searchClicked();

        function clientChanged() {
            searchClicked();
        };

        function nextClicked() {
            if (vm.searchModel.pageNumber == vm.lastPageNumber) return;

            vm.searchModel.pageNumber += 1;
            searchClicked();
        };

        function payrollPeriodMonthChanged() {
            searchClicked();
        };

        function payrollPeriodYearChanged() {
            searchClicked();
        };

        function previousClicked() {
            if (vm.searchModel.pageNumber == 1) return;

            vm.searchModel.pageNumber -= 1;
            searchClicked();
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
                            clients: vm.clients,
                            client: vm.searchModel.client
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
            if (vm.searchModel.client && vm.searchModel.client.id > 0) {
                vm.searchModel.clientId = vm.searchModel.client.id;
            }
            else {
                vm.searchModel.clientId = undefined;
            }

            vm.searchInProgress = true;

            $http.get('/Payroll/Search', { params: vm.searchModel }).then(function (response) {
                vm.payrollProcessBatches = response.data.payrollProcessBatches;
                vm.totalResultsCount = response.data.totalResultsCount;
                vm.lastPageNumber = response.data.lastPageNumber;

                vm.searchInProgress = false;
            });
        };

        function sendPayslipClicked(payrollProcessBatch) {
            if (!navigator.onLine) {
                alert('Send payslip failed. There is no internet connection.');
                return;
            }

            console.log('sendPayslipClicked');

            $http.post('/Payroll/SendPayslip', { payrollProcessBatchId: payrollProcessBatch.id }).then(function (response) {
                alert('Payslips sent.');
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('AddLoanModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', AddLoanModalCtrl]);

    function AddLoanModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addLoanSubmit = addLoanSubmit;
        vm.cancel = cancel;
        vm.clientChanged = clientChanged;
        vm.clients = params.clients;
        vm.currencySymbol = 'P';
        vm.getInterestAmount = getInterestAmount;
        vm.payrollPeriods = [];
        vm.loanTypesList = params.loanTypesList;
        vm.lookups = lookups;
        vm.transactionNumber = params.nextTransactionNumber;
        vm.validationErrors = {};

        init();

        $scope.$watch('vm.remainingBalance', updateDeductionAmount);
        $scope.$watch('vm.interestRate', updateDeductionAmount);
        $scope.$watch('vm.monthsPayable', updateDeductionAmount);

        $timeout(function () {
            if (vm.client && vm.client.id > 0) {
                vm.client = params.client;
            }
        });

        function addLoanSubmit(e) {
            var action = '/Loans/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function clientChanged() {
            populateEmployees();
            populatePayrollPeriods();
        };

        function getInterestAmount() {
            if (!vm.remainingBalance || vm.remainingBalance <= 0 || !vm.interestRate || vm.interestRate <= 0) return 0;

            return vm.remainingBalance * (vm.interestRate / 100)
        };

        function init() {
            populateEmployees();
            populatePayrollPeriods();
        };

        function populateEmployees() {
            if (vm.client && vm.client.id > 0) {
                vm.searchInProgress = true;

                $http.get('/Employees/GetByClientId', { params: { clientId: vm.client.id } }).then(function (response) {
                    vm.employees = response.data.employees;
                    vm.searchInProgress = false;
                });
            }
            else {
                vm.employees = [];
                vm.employees.splice(0, 0, { name: '-- Select a client --' });
            }            
        };

        function populatePayrollPeriods() {
            var payrollPeriods = [];

            if (vm.client && vm.client.id > 0) {
                for (var i = 0; i < vm.client.numberOfPayrollPeriodsAMonth; i++) {
                    payrollPeriods.push(i + 1);
                }
            }
            else {
                payrollPeriods.push('-- Select a client --');
                vm.payrollPeriod = '-- Select a client --';
            }

            vm.payrollPeriods = payrollPeriods;
        };

        function updateDeductionAmount() {
            if (vm.getInterestAmount() == 0 || !vm.monthsPayable || vm.monthsPayable <= 0) return 0;

            vm.deductionAmount = Math.round(vm.getInterestAmount() / vm.monthsPayable * 100) / 100;
        };
    };
}());
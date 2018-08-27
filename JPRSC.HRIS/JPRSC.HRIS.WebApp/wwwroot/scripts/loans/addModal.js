(function () {
    angular
        .module('app')
        .controller('AddLoanModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', AddLoanModalCtrl]);

    function AddLoanModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addInProgress = false;
        vm.addLoanSubmit = addLoanSubmit;
        vm.cancel = cancel;
        vm.clientChanged = clientChanged;
        vm.clients = params.clients;
        vm.currencySymbol = 'P';
        vm.getPayrollPeriodInput = getPayrollPeriodInput;
        vm.getTotalAmount = getTotalAmount;
        vm.loanPayrollPeriods = [];
        vm.loanTypesList = params.loanTypesList;
        vm.lookups = lookups;
        vm.transactionNumber = params.nextTransactionNumber;
        vm.validationErrors = {};

        init();

        $timeout(function () {
            if (params.client && params.client.id > 0) {
                vm.client = params.client;
                clientChanged();
            }
        });

        function addLoanSubmit(e) {
            vm.addInProgress = true;

            var action = '/Loans/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.addInProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function clientChanged() {
            populateEmployees();
            populatePayrollPeriods();
        };

        function getPayrollPeriodInput(payrollPeriods) {
            if (!payrollPeriods || !payrollPeriods.length) return;

            var selectedPayrollPeriods = [];

            for (var i = 0; i < payrollPeriods.length; i++) {
                var item = payrollPeriods[i];
                if (item.selected === true) {
                    selectedPayrollPeriods.push(item.payrollPeriod.toString());
                }
            }

            return selectedPayrollPeriods.join(',');
        };

        function getTotalAmount() {
            var totalAmount = 0;

            if (vm.deductionAmount > 0 && vm.monthsPayable > 0) {
                totalAmount = vm.deductionAmount * vm.monthsPayable;
            }

            return totalAmount;
        }

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
                vm.employees.splice(0, 0, { name: '-- Select a client --', employeeCode: '-- Select a client --' });
            }            
        };

        function populatePayrollPeriods() {
            var payrollPeriods = [];

            if (vm.client && vm.client.id > 0) {
                for (var i = 0; i < vm.client.numberOfPayrollPeriodsAMonth; i++) {
                    payrollPeriods.push({ payrollPeriod: i + 1, selected: false });
                }
            }

            vm.loanPayrollPeriods = angular.copy(payrollPeriods);
        };
    };
}());
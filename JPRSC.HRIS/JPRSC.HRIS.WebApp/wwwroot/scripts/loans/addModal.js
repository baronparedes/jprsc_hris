(function () {
    angular
        .module('app')
        .controller('AddLoanModalCtrl', ['$http', '$scope', '$uibModalInstance', 'lookups', 'params', AddLoanModalCtrl]);

    function AddLoanModalCtrl($http, $scope, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addLoanSubmit = addLoanSubmit;
        vm.cancel = cancel;
        vm.client = params.client;
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

        function getInterestAmount() {
            if (!vm.remainingBalance || vm.remainingBalance <= 0 || !vm.interestRate || vm.interestRate <= 0) return 0;

            return vm.remainingBalance * (vm.interestRate / 100)
        };

        function init() {
            populateEmployees();
            populatePayrollPeriods();
        };

        function populateEmployees() {
            vm.searchInProgress = true;

            $http.get('/Employees/GetByClientId', { params: { clientId: vm.client.id } }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });
        };

        function populatePayrollPeriods() {
            // TODO
            //o	Payroll period values option should auto populate depending on the client’s cutoff period setting.
            //	Weekly – Options should be: 1, 2, 3, 4
            //	Bi - monthly – Options should be: 1 and 2
            //	Monthly – Options should be: 1

            console.log('vm.client', vm.client);

            if (vm.client.cutOffPeriod === vm.lookups.cutOffPeriods.daily.value) {

            }

            var payrollPeriods = [];

            for (var i = 0; i < vm.client.numberOfPayrollPeriodsAMonth; i++) {
                payrollPeriods.push(i + 1);
            }

            vm.payrollPeriods = payrollPeriods;
        };

        function updateDeductionAmount() {
            if (vm.getInterestAmount() == 0 || !vm.monthsPayable || vm.monthsPayable <= 0) return 0;

            vm.deductionAmount = Math.round(vm.getInterestAmount() / vm.monthsPayable * 100) / 100;
        };
    };
}());
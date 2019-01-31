(function () {
    angular
        .module('app')
        .controller('EditLoanModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', EditLoanModalCtrl]);

    function EditLoanModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.editInProgress = false;
        vm.editLoanSubmit = editLoanSubmit;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.currencySymbol = 'P';
        vm.getPayrollPeriodInput = getPayrollPeriodInput;
        vm.getTotalAmount = getTotalAmount;
        vm.loan = params.loan;
        vm.loanPayrollPeriods = [];
        vm.lookups = lookups;
        vm.validationErrors = {};

        console.log('vm.loan', vm.loan);

        init();

        function editLoanSubmit(e) {
            vm.editInProgress = true;

            var action = '/Loans/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.editInProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
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
            populatePayrollPeriods();
        };

        function populatePayrollPeriods() {
            var payrollPeriods = [];

            if (vm.client && vm.client.id > 0) {
                for (var i = 0; i < vm.client.numberOfPayrollPeriodsAMonth; i++) {
                    var selected = vm.loan.loanPayrollPeriods.indexOf(i + 1) !== -1;

                    payrollPeriods.push({ payrollPeriod: i + 1, selected: selected });
                }
            }

            vm.loanPayrollPeriods = angular.copy(payrollPeriods);
        };
    };
}());
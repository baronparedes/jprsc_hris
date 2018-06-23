(function () {
    angular
        .module('app')
        .controller('AddLoanModalCtrl', ['$http', '$uibModalInstance', 'lookups', 'params', AddLoanModalCtrl]);

    function AddLoanModalCtrl($http, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addLoanSubmit = addLoanSubmit;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.payrollPeriods = [];
        vm.employees = params.employees;
        vm.loanTypesList = params.loanTypesList;
        vm.lookups = lookups;
        vm.transactionNumber = params.nextTransactionNumber;
        vm.validationErrors = {};

        init();

        function addLoanSubmit(e) {
            var action = '/Loans/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Loans/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function init() {
            console.log('init');

            populatePayrollPeriods();
        };

        function populatePayrollPeriods() {
            console.log('vm.lookups', vm.lookups);
            console.log('vm.client', vm.client);

            // TODO
            //o	Payroll period values option should auto populate depending on the client’s cutoff period setting.
            //	Weekly – Options should be: 1, 2, 3, 4
            //	Bi - monthly – Options should be: 1 and 2
            //	Monthly – Options should be: 1

            if (vm.client.cutOffPeriod === vm.lookups.cutOffPeriods.daily.value) {

            }
        };
    };
}());
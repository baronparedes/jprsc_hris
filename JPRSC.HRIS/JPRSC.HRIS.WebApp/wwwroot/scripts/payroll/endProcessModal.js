(function () {
    angular
        .module('app')
        .controller('EndProcessModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', EndProcessModalCtrl]);

    function EndProcessModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addInProgress = false;
        vm.cancel = cancel;
        vm.clients = params.clients;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.endProcessSubmit = endProcessSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            vm.client = vm.clients[0];
            vm.client.nextPayrollPeriodFrom = Date.parse(vm.client.nextPayrollPeriodFrom);
            vm.client.nextPayrollPeriodTo = Date.parse(vm.client.nextPayrollPeriodTo);
            vm.client.nextPayrollPeriodMonth = vm.client.nextPayrollPeriodMonth.toString();
        });

        function endProcessSubmit() {
            vm.endProcessInProgress = true;
            var action = '/Payroll/EndProcessCommand';
            var data = $('#endProcessForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                console.log('response', response);
                alert(`Successfully End processed! Payslip creation will continue in the background.\n\n${response.data.recordCount} payslips will be saved to: '${response.data.filesPath}'`);
                $uibModalInstance.close();

                vm.endProcessInProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.endProcessInProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
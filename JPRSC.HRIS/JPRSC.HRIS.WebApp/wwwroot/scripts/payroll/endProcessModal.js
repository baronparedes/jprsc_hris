(function () {
    angular
        .module('app')
        .controller('EndProcessModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', EndProcessModalCtrl]);

    function EndProcessModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addInProgress = false;
        vm.cancel = cancel;
        vm.endProcessSubmit = endProcessSubmit;
        vm.payrollProcessBatch = params.payrollProcessBatch;
        vm.validationErrors = {};

        $timeout(function () {
            vm.payrollProcessBatch.client.nextPayrollPeriodFrom = Date.parse(vm.payrollProcessBatch.client.nextPayrollPeriodFrom);
            vm.payrollProcessBatch.client.nextPayrollPeriodTo = Date.parse(vm.payrollProcessBatch.client.nextPayrollPeriodTo);
        });

        function endProcessSubmit() {
            vm.endProcessInProgress = true;
            var action = '/Payroll/EndProcessCommand';
            var data = $('#endProcessForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
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
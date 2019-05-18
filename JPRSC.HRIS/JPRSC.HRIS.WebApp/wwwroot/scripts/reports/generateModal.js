(function () {
    angular
        .module('app')
        .controller('GenerateReportModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', GenerateReportModalCtrl]);

    function GenerateReportModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.clients = params.clients;
        vm.fromPayrollPeriod = '1';
        vm.fromPayrollPeriodMonth = '10';
        vm.toPayrollPeriod = '1';
        vm.toPayrollPeriodMonth = '10';
        vm.payrollPeriodMonth = '-1';
        vm.payrollPeriodYear = new Date().getFullYear().toString();
        vm.payrollPeriodFromYear = new Date().getFullYear().toString();
        vm.payrollPeriodToYear = new Date().getFullYear().toString();
        vm.reportType = params.reportType;
        vm.validationErrors = {};

        $timeout(function () {
            vm.clients.splice(0, 0, { id: -1, code: 'All Clients', name: 'All Clients' });
            vm.client = vm.clients[0];
        });

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('GenerateSingleLoanTypeReportModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', GenerateSingleLoanTypeReportModalCtrl]);

    function GenerateSingleLoanTypeReportModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.clients = params.clients;
        vm.payrollPeriodMonth = '-1';
        vm.loanType = params.loanType;
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
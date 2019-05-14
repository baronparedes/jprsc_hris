(function () {
    angular
        .module('app')
        .controller('BulkChangeModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', BulkChangeModalCtrl]);

    function BulkChangeModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.clients = angular.copy(params.clients);
        vm.hourlyRate = 0;
        vm.dailyRate = 0;
        vm.monthlyRate = 0;
        vm.colaHourly = 0;
        vm.colaDaily = 0;
        vm.colaMonthly = 0;
        vm.bulkChangeProgress = false;
        vm.bulkChangeSubmit = bulkChangeSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            vm.clients.splice(0, 1);
            vm.client = vm.clients[0];
        });

        function bulkChangeSubmit() {
            var confirmMessage = `Are you sure you want to change the following?\n\nHourly Rate: ${vm.hourlyRate}\nDaily Rate: ${vm.dailyRate}\nMonthly Rate: ${vm.monthlyRate}\nCOLA Hourly: ${vm.colaHourly}\nCOLA Daily: ${vm.colaDaily}\nCOLA Monthly: ${vm.colaMonthly}`;
            if (!confirm(confirmMessage)) return;

            vm.bulkChangeProgress = true;
            var action = '/EmployeeRates/BulkChange';
            var data = $('#bulkChangeForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();

                vm.bulkChangeProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.bulkChangeProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
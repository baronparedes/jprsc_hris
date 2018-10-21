(function () {
    angular
        .module('app')
        .controller('RehireTransferModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', RehireTransferModalCtrl]);

    function RehireTransferModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.clients = params.clients;
        vm.employee = params.employee;
        vm.rehireTransferProgress = false;
        vm.rehireTransferSubmit = rehireTransferSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            vm.client = vm.clients[0];
        });

        function rehireTransferSubmit() {
            vm.rehireTransferProgress = true;
            var action = '/Employees/RehireTransfer';
            var data = $('#rehireTransferForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();

                vm.rehireTransferProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.rehireTransferProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
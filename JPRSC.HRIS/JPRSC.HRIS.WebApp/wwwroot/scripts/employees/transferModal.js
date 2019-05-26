(function () {
    angular
        .module('app')
        .controller('TransferModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', TransferModalCtrl]);

    function TransferModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.clients = params.clients;
        vm.employee = params.employee;
        vm.transferProgress = false;
        vm.transferSubmit = transferSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            vm.client = vm.clients[0];
        });

        function transferSubmit() {
            vm.transferProgress = true;
            var action = '/Employees/Transfer';
            var data = $('#transferForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();

                vm.transferProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.transferProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
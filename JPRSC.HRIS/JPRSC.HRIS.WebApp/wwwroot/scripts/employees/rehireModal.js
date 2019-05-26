(function () {
    angular
        .module('app')
        .controller('RehireModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'globalSettings', 'lookups', 'params', RehireModalCtrl]);

    function RehireModalCtrl($http, $scope, $timeout, $uibModalInstance, globalSettings, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.clients = params.clients;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.employee = params.employee;
        vm.rehireProgress = false;
        vm.rehireSubmit = rehireSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            vm.client = vm.clients[0];
        });

        function rehireSubmit() {
            vm.rehireProgress = true;
            var action = '/Employees/Rehire';
            var data = $('#rehireForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $uibModalInstance.close();

                vm.rehireProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.rehireProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
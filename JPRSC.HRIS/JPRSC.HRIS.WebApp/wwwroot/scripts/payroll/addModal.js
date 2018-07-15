﻿(function () {
    angular
        .module('app')
        .controller('AddPayrollModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', AddPayrollModalCtrl]);

    function AddPayrollModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addInProgress = false;
        vm.cancel = cancel;
        vm.clientChanged = clientChanged;
        vm.clients = params.clients;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.processSubmit = processSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            vm.client = vm.clients[0];
            clientChanged();
        });

        function processSubmit() {
            console.log('processSubmit');
            console.log('vm', vm);

            vm.processInProgress = true;
            var action = '/Payroll/Process';
            var data = $('#processForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                

                vm.processInProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.processInProgress = false;
            });
        };
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function clientChanged() {
            vm.payrollPeriod = vm.client.currentPayrollPeriod;
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('EditClientCtrl', ['$http', '$timeout', '$window', 'globalSettings', 'lookups', EditClientCtrl]);

    function EditClientCtrl($http, $timeout, $window, globalSettings, lookups) {
        var vm = this;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.editClientSubmit = editClientSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            var client = vm.serverModel;
            vm.cutOffPeriod = { value: client.cutOffPeriod };
            vm.taxTable = { value: client.taxTable };
            vm.payrollCode = { value: client.payrollCode };
            vm.payrollPeriodMonth = { value: client.payrollPeriodMonth };
        });

        function editClientSubmit(e) {
            var action = '/Clients/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Clients/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
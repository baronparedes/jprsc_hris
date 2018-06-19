(function () {
    angular
        .module('app')
        .controller('EditClientCtrl', ['$http', '$timeout', '$window', 'lookups', EditClientCtrl]);

    function EditClientCtrl($http, $timeout, $window) {
        var vm = this;
        vm.editClientSubmit = editClientSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            var client = vm.serverModel;
            console.log('client', client);
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
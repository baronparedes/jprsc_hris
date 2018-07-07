(function () {
    angular
        .module('app')
        .controller('AddClientCtrl', ['$http', '$window', 'globalSettings', 'lookups', AddClientCtrl]);

    function AddClientCtrl($http, $window, globalSettings, lookups) {
        var vm = this;
        vm.addClientSubmit = addClientSubmit;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.lookups = lookups;
        vm.validationErrors = {};

        function addClientSubmit(e) {
            var action = '/Clients/Add';
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
(function () {
    angular
        .module('app')
        .controller('AddCompanyCtrl', ['$http', '$window', 'globalSettings', AddCompanyCtrl]);

    function AddCompanyCtrl($http, $window, globalSettings) {
        var vm = this;
        vm.addCompanySubmit = addCompanySubmit;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.validationErrors = {};

        function addCompanySubmit(e) {
            var action = '/Companies/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Companies/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
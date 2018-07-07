(function () {
    angular
        .module('app')
        .controller('EditCompanyCtrl', ['$http', '$window', 'globalSettings', EditCompanyCtrl]);

    function EditCompanyCtrl($http, $window, globalSettings) {
        var vm = this;;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.editCompanySubmit = editCompanySubmit;
        vm.validationErrors = {};

        function editCompanySubmit(e) {
            var action = '/Companies/Edit';
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
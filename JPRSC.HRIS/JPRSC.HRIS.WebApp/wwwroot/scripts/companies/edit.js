(function () {
    angular
        .module('app')
        .controller('EditCompanyCtrl', ['$http', '$timeout', '$window', 'globalSettings', EditCompanyCtrl]);

    function EditCompanyCtrl($http, $timeout, $window, globalSettings) {
        var vm = this;;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.editCompanySubmit = editCompanySubmit;
        vm.validationErrors = {};

        $timeout(function () {
            var company = vm.serverModel;

            if (company.dateIssued) {
                vm.dateIssued = new Date(company.dateIssued);
            }
        });

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
(function () {
    angular
        .module('app')
        .controller('EditEmployeeCtrl', ['$http', '$timeout', '$window', 'globalSettings', 'lookups', EditEmployeeCtrl]);

    function EditEmployeeCtrl($http, $timeout, $window, globalSettings, lookups) {
        var vm = this;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.editEmployeeSubmit = editEmployeeSubmit;
        vm.editInProgress = false;
        vm.hasValidationErrors = false;
        vm.init = init;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            vm.datepickerOptions.maxMode = 'year';
        });

        function editEmployeeSubmit(e) {
            vm.editInProgress = true;
            vm.hasValidationErrors = false;

            var action = '/Employees/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Employees/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                    vm.hasValidationErrors = true;
                }

                vm.editInProgress = false;
            });
        };

        function init(key) {
            var employee = window[key];

            if (employee.dateOfBirth) {
                vm.dateOfBirth = new Date(employee.dateOfBirth);
            }

            if (employee.dateHired) {
                vm.dateHired = new Date(employee.dateHired);
            }

            if (employee.dateResigned) {
                vm.dateResigned = new Date(employee.dateResigned);
            }

            vm.gender = { value: employee.gender };
            vm.citizenship = { value: employee.citizenship };
            vm.civilStatus = { value: employee.civilStatus };
            vm.accountType = { value: employee.accountType };
            vm.region = { value: employee.region };
        };
    };
}());
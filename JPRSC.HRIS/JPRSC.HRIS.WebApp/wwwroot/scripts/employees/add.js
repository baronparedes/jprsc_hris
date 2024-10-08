﻿(function () {
    angular
        .module('app')
        .controller('AddEmployeeCtrl', ['$http', '$timeout', '$window', 'globalSettings', 'lookups', AddEmployeeCtrl]);

    function AddEmployeeCtrl($http, $timeout, $window, globalSettings, lookups) {
        var vm = this;
        vm.addEmployeeSubmit = addEmployeeSubmit;
        vm.addInProgress = false;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.hasValidationErrors = false;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {  
            vm.datepickerOptions.maxMode = 'year';
        });

        function addEmployeeSubmit(e) {
            vm.addInProgress = true;
            vm.hasValidationErrors = false;

            var action = '/Employees/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Employees/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                    vm.hasValidationErrors = true;
                }

                vm.addInProgress = false;
            });
        };
    };
}());
﻿(function () {
    angular
        .module('app')
        .controller('EditDepartmentCtrl', ['$http', '$window', EditDepartmentCtrl]);

    function EditDepartmentCtrl($http, $window) {
        var vm = this;
        vm.editDepartmentSubmit = editDepartmentSubmit;
        vm.validationErrors = {};

        function editDepartmentSubmit(e) {
            var action = '/Departments/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Departments/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
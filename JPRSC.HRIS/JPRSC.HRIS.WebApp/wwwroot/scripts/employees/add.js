(function () {
    angular
        .module('app')
        .controller('AddEmployeeCtrl', ['$http', '$window', AddEmployeeCtrl]);

    function AddEmployeeCtrl($http, $window) {
        var vm = this;
        vm.addEmployeeSubmit = addEmployeeSubmit;
        vm.validationErrors = {};

        function addEmployeeSubmit(e) {
            var action = '/Employees/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Employees/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
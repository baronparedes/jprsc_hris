(function () {
    angular
        .module('app')
        .controller('EditEmployeeCtrl', ['$http', '$window', EditEmployeeCtrl]);

    function EditEmployeeCtrl($http, $window) {
        var vm = this;
        vm.editEmployeeSubmit = editEmployeeSubmit;
        vm.validationErrors = {};

        function editEmployeeSubmit(e) {
            var action = '/Employees/Edit';
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
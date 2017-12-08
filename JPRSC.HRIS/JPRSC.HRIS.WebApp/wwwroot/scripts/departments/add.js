(function () {
    angular
        .module('app')
        .controller('AddDepartmentCtrl', ['$http', '$window', AddDepartmentCtrl]);

    function AddDepartmentCtrl($http, $window) {
        var vm = this;
        vm.addDepartmentSubmit = addDepartmentSubmit;
        vm.validationErrors = {};

        function addDepartmentSubmit(e) {
            var action = '/Departments/Add';
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
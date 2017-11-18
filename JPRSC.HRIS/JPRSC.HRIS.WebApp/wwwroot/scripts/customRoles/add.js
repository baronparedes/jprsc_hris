(function () {
    angular
        .module('app')
        .controller('AddCustomRoleCtrl', ['$http', '$window', AddCustomRoleCtrl]);

    function AddCustomRoleCtrl($http, $window) {
        var vm = this;
        vm.addCustomRoleSubmit = addCustomRoleSubmit;
        vm.validationErrors = {};

        function addCustomRoleSubmit(e) {
            var action = '/CustomRoles/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/CustomRoles/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
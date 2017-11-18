(function () {
    angular
        .module('app')
        .controller('EditCustomRoleCtrl', ['$http', '$window', EditCustomRoleCtrl]);

    function EditCustomRoleCtrl($http, $window) {
        var vm = this;
        vm.editCustomRoleSubmit = editCustomRoleSubmit;
        vm.validationErrors = {};

        function editCustomRoleSubmit(e) {
            var action = '/CustomRoles/Edit';
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
(function () {
    angular
        .module('app')
        .controller('ChangePasswordCtrl', ['$http', '$window', ChangePasswordCtrl]);

    function ChangePasswordCtrl($http, $window) {
        var vm = this;
        vm.changePasswordSubmit = changePasswordSubmit;
        vm.validationErrors = {};

        function changePasswordSubmit(e) {
            var action = '/Accounts/ChangePassword';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Accounts/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
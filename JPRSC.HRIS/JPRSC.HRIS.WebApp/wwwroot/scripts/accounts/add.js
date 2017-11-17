(function () {
    angular
        .module('app')
        .controller('AddAccountCtrl', ['$http', '$window', AddAccountCtrl]);

    function AddAccountCtrl($http, $window) {
        var vm = this;
        vm.addAccountSubmit = addAccountSubmit;
        vm.validationErrors = {};

        function addAccountSubmit(e) {
            var action = '/Accounts/Add';
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
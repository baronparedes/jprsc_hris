(function () {
    angular
        .module('app')
        .controller('EditOwnAccountCtrl', ['$http', '$window', EditOwnAccountCtrl]);

    function EditOwnAccountCtrl($http, $window) {
        var vm = this;
        vm.editAccountSubmit = editAccountSubmit;
        vm.validationErrors = {};

        function editAccountSubmit(e) {
            var action = '/Accounts/EditOwn';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Home/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('AddBankCtrl', ['$http', '$window', AddBankCtrl]);

    function AddBankCtrl($http, $window) {
        var vm = this;
        vm.addBankSubmit = addBankSubmit;
        vm.validationErrors = {};

        function addBankSubmit(e) {
            var action = '/Banks/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Banks/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
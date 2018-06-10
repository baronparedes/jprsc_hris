(function () {
    angular
        .module('app')
        .controller('EditBankCtrl', ['$http', '$window', EditBankCtrl]);

    function EditBankCtrl($http, $window) {
        var vm = this;
        vm.editBankSubmit = editBankSubmit;
        vm.validationErrors = {};

        function editBankSubmit(e) {
            var action = '/Banks/Edit';
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
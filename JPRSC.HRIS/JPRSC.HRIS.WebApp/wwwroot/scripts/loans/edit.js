(function () {
    angular
        .module('app')
        .controller('EditLoanCtrl', ['$http', '$window', EditLoanCtrl]);

    function EditLoanCtrl($http, $window) {
        var vm = this;
        vm.editLoanSubmit = editLoanSubmit;
        vm.validationErrors = {};

        function editLoanSubmit(e) {
            var action = '/Loans/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Loans/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
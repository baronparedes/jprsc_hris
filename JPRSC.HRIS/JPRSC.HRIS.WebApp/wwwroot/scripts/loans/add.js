(function () {
    angular
        .module('app')
        .controller('AddLoanCtrl', ['$http', '$window', AddLoanCtrl]);

    function AddLoanCtrl($http, $window) {
        var vm = this;
        vm.addLoanSubmit = addLoanSubmit;
        vm.validationErrors = {};

        function addLoanSubmit(e) {
            var action = '/Loans/Add';
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
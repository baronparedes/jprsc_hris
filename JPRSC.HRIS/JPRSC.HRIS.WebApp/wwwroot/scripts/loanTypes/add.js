(function () {
    angular
        .module('app')
        .controller('AddLoanTypeCtrl', ['$http', '$window', 'lookups', AddLoanTypeCtrl]);

    function AddLoanTypeCtrl($http, $window, lookups) {
        var vm = this;
        vm.addLoanTypeSubmit = addLoanTypeSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        function addLoanTypeSubmit(e) {
            var action = '/LoanTypes/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/LoanTypes/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
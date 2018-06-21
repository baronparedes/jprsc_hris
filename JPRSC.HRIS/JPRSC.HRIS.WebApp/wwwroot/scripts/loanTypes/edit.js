(function () {
    angular
        .module('app')
        .controller('EditLoanTypeCtrl', ['$http', '$timeout', '$window', 'lookups', EditLoanTypeCtrl]);

    function EditLoanTypeCtrl($http, $timeout, $window, lookups) {
        var vm = this;
        vm.editLoanTypeSubmit = editLoanTypeSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            var loanType = vm.serverModel;
            vm.loanTypeType = { value: loanType.loanTypeType };
        });

        function editLoanTypeSubmit(e) {
            var action = '/LoanTypes/Edit';
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
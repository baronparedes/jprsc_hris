(function () {
    angular
        .module('app')
        .controller('LoanDetailsModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', LoanDetailsModalCtrl]);

    function LoanDetailsModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.currencySymbol = 'P';
        vm.loan = params.loan;
        vm.loanResult = {};
        vm.searchInProgress = false;
        vm.validationErrors = {};

        init();

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function init() {
            populateLoanDetails();
        };

        function populateLoanDetails() {
            vm.searchInProgress = true;

            $http.get('/Loans/GetById', { params: { id: vm.loan.id } }).then(function (response) {
                vm.loanResult = response.data.loanResult;

                console.log(vm.loanResult);

                vm.searchInProgress = false;
            });
        };
    };
}());
﻿(function () {
    angular
        .module('app')
        .controller('LoanIndexCtrl', ['$http', '$timeout', '$uibModal', '$scope', LoanIndexCtrl]);

    function LoanIndexCtrl($http, $timeout, $uibModal, $scope) {
        var vm = this;
        vm.addLoanClicked = addLoanClicked;
        vm.currencySymbol = 'P';
        vm.getDeductionAmount = getDeductionAmount;
        vm.getInterestAmount = getInterestAmount;
        vm.editLoanClicked = editLoanClicked;
        vm.loans = [];
        vm.nextTransactionNumber = '';
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.loanTypesList = vm.serverModel.loanTypesList;
            vm.nextTransactionNumber = vm.serverModel.nextTransactionNumber;
        });

        function addLoanClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'addLoanModal.html',
                controller: 'AddLoanModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            client: vm.searchModel.client,
                            employees: vm.employees,
                            loanTypesList: vm.loanTypesList,
                            nextTransactionNumber: vm.nextTransactionNumber
                        }
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();
            });
        };

        function editLoanClicked(loan) {

        };

        function getDeductionAmount(loan) {
            if (vm.getInterestAmount(loan) == 0 || !loan.monthsPayable || loan.monthsPayable <= 0) return 0;

            return vm.getInterestAmount(loan) / loan.monthsPayable;
        };

        function getInterestAmount(loan) {
            if (!loan.principalAmount || loan.principalAmount <= 0 || !loan.interestRate || loan.interestRate <= 0) return 0;

            return loan.principalAmount * (loan.interestRate / 100)
        };

        function onSearchModelChange(newValue, oldValue) {
            if (!vm.searchModel.client || vm.searchModel.client.id <= 0) return;

            searchClicked();
        };

        function searchClicked() {
            vm.searchModel.clientId = vm.searchModel.client.id;
            vm.searchInProgress = true;

            $http.get('/Loans/Search', { params: vm.searchModel }).then(function (response) {
                vm.loans = response.data.loans;
                vm.searchInProgress = false;
            });
        };
    };
}());
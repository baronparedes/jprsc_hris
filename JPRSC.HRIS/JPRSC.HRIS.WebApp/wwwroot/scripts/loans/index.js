(function () {
    angular
        .module('app')
        .controller('LoanIndexCtrl', ['$http', '$timeout', '$uibModal', '$scope', '$window', 'globalSettings', LoanIndexCtrl]);

    function LoanIndexCtrl($http, $timeout, $uibModal, $scope, $window, globalSettings) {
        var vm = this;
        vm.addLoanClicked = addLoanClicked;
        vm.currencySymbol = 'P';
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.detailsClicked = detailsClicked;
        vm.getInterestAmount = getInterestAmount;
        vm.loans = [];
        vm.loanTypes = [];
        vm.nextTransactionNumber = '';
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;
        vm.zeroOutSubmit = zeroOutSubmit;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { code: '-- Select a client --', name: '-- Select a client --' });
            vm.loanTypesList = vm.serverModel.loanTypesList;
            vm.nextTransactionNumber = vm.serverModel.nextTransactionNumber;

            $http.get('/LoanTypes/Search').then(function (response) {
                vm.loanTypes = response.data.loanTypes;
                vm.loanTypes.splice(0, 0, { code: '-- Select a loan type --', description: ' -- Select a loan type -- ' });
            });
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
                            clients: vm.clients,
                            loanTypesList: vm.loanTypesList,
                            nextTransactionNumber: vm.nextTransactionNumber
                        };
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();
            });
        };

        function detailsClicked(loan) {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'loanDetailsModal.html',
                controller: 'LoanDetailsModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            loan: loan
                        };
                    }
                }
            });
        };

        function getInterestAmount(loan) {
            if (!loan.principalAmount || loan.principalAmount <= 0 || !loan.interestRate || loan.interestRate <= 0) return 0;

            return loan.principalAmount * (loan.interestRate / 100);
        };

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            if (vm.searchModel.client && vm.searchModel.client.id > 0) {
                vm.searchModel.clientId = vm.searchModel.client.id;
            }
            else {
                vm.searchModel.clientId = undefined;
            }

            if (vm.searchModel.loanType && vm.searchModel.loanType.id > 0) {
                vm.searchModel.loanTypeId = vm.searchModel.loanType.id;
            }
            else {
                vm.searchModel.loanTypeId = undefined;
            }

            vm.searchInProgress = true;

            $http.get('/Loans/Search', { params: vm.searchModel }).then(function (response) {
                vm.loans = response.data.loans;
                vm.searchInProgress = false;
            });
        };

        function zeroOutSubmit(e) {
            if (!confirm('Are you sure you want to zero out this loan?')) return;

            var action = '/Loans/ZeroOut';
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
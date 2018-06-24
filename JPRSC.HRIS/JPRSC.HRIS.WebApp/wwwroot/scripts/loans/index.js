(function () {
    angular
        .module('app')
        .controller('LoanIndexCtrl', ['$http', '$timeout', '$uibModal', '$scope', LoanIndexCtrl]);

    function LoanIndexCtrl($http, $timeout, $uibModal, $scope) {
        var vm = this;
        vm.addLoanClicked = addLoanClicked;
        vm.client = {};
        vm.clientsList = [];
        vm.editLoanClicked = editLoanClicked;
        vm.loans = [];
        vm.nextTransactionNumber = '';
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);
        $scope.$watch('vm.client', onSearchModelChange, true);

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
                            client: vm.client,
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

        function onSearchModelChange(newValue, oldValue) {
            if (vm.client.id <= 0) return;

            searchClicked();
        };

        function searchClicked() {
            vm.searchModel.clientId = vm.client.id;
            vm.searchInProgress = true;

            $http.get('/Loans/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });
        };
    };
}());
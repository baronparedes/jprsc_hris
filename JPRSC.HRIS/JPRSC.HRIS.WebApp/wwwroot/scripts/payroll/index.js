(function () {
    angular
        .module('app')
        .controller('PayrollIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', 'globalSettings', PayrollIndexCtrl]);

    function PayrollIndexCtrl($http, $scope, $timeout, $uibModal, globalSettings) {
        var vm = this;
        vm.processClicked = processClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            console.log(vm.clients);
        });

        searchClicked();

        function processClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'addPayrollModal.html',
                controller: 'AddPayrollModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clients
                        }
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();
            }, function () {
                searchClicked();
            });
        };

        function searchClicked() {
            vm.searchInProgress = true;

            //$http.get('/Payroll/Search', { params: vm.searchModel }).then(function (response) {
            //    vm.employees = response.data.employees;
                vm.searchInProgress = false;
            //});
        };
    };
}());
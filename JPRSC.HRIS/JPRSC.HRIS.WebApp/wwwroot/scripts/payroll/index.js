﻿(function () {
    angular
        .module('app')
        .controller('PayrollIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', 'globalSettings', PayrollIndexCtrl]);

    function PayrollIndexCtrl($http, $scope, $timeout, $uibModal, globalSettings) {
        var vm = this;
        vm.clientChanged = clientChanged;
        vm.payrollPeriodMonthChanged = payrollPeriodMonthChanged;
        vm.payrollProcessBatches = [];
        vm.processClicked = processClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { payrollPeriodMonth: '-1' };
        vm.searchInProgress = false;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { id: null, code: 'All Clients' });
            vm.searchModel.client = vm.clients[0];
        });

        searchClicked();

        function clientChanged() {
            searchClicked();
        };

        function payrollPeriodMonthChanged() {
            searchClicked();
        };

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
                            clients: vm.clients,
                            client: vm.searchModel.client
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
            if (vm.searchModel.client && vm.searchModel.client.id > 0) {
                vm.searchModel.clientId = vm.searchModel.client.id;
            }
            else {
                vm.searchModel.clientId = undefined;
            }

            vm.searchInProgress = true;

            $http.get('/Payroll/Search', { params: vm.searchModel }).then(function (response) {
                vm.payrollProcessBatches = response.data.payrollProcessBatches;
                vm.searchInProgress = false;
            });
        };
    };
}());
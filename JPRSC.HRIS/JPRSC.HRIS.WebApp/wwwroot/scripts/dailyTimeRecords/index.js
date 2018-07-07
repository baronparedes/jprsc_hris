﻿(function () {
    angular
        .module('app')
        .controller('DailyTimeRecordIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', 'globalSettings', DailyTimeRecordIndexCtrl]);

    function DailyTimeRecordIndexCtrl($http, $scope, $timeout, $uibModal, globalSettings) {
        var vm = this;
        vm.addDailyTimeRecordClicked = addDailyTimeRecordClicked;
        vm.clientsList = [];
        vm.currencySymbol = 'P';
        vm.dailyTimeRecords = [];
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.payRates = vm.serverModel.payRates;
        });

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function addDailyTimeRecordClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'addDailyTimeRecordModal.html',
                controller: 'AddDailyTimeRecordModalCtrl',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    params: function () {
                        return {
                            client: vm.searchModel.client,
                            employees: vm.employees,
                            payRates: vm.payRates
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

        function onSearchModelChange(newValue, oldValue) {
            if (!vm.searchModel.client || vm.searchModel.client.id <= 0) return;

            searchClicked();
        };

        function searchClicked() {
            vm.searchModel.clientId = vm.searchModel.client.id;
            vm.searchInProgress = true;

            $http.get('/DailyTimeRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('DailyTimeRecordIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', 'globalSettings', DailyTimeRecordIndexCtrl]);

    function DailyTimeRecordIndexCtrl($http, $scope, $timeout, $uibModal, globalSettings) {
        var vm = this;
        vm.addDailyTimeRecordClicked = addDailyTimeRecordClicked;
        vm.bulkUploadDTRClicked = bulkUploadDTRClicked;
        vm.bulkUploadEDRClicked = bulkUploadEDRClicked;
        vm.clientChanged = clientChanged;
        vm.currencySymbol = 'P';
        vm.dailyTimeRecords = [];
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.lastPageNumber = 1;
        vm.nextClicked = nextClicked;
        vm.payrollPeriodChanged = payrollPeriodChanged;
        vm.payrollPeriodMonthChanged = payrollPeriodMonthChanged;
        vm.payrollPeriodSelectionLoading = false;
        vm.payrollPeriodYearChanged = payrollPeriodYearChanged;
        vm.previousClicked = previousClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1, payrollPeriodMonth: '10', payrollPeriodYear: new Date().getFullYear().toString() };
        vm.searchModelCurrent = { pageNumber: 1, payrollPeriodMonth: '10', payrollPeriodYear: new Date().getFullYear().toString() };
        vm.searchInProgress = false;
        vm.totalResultsCount = 0;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { id: null, code: '-- Select a client --' });
            vm.searchModel.client = vm.clients[0];
            vm.earningDeductions = vm.serverModel.earningDeductions;
            vm.payRates = vm.serverModel.payRates;

            populatePayrollPeriodsSelection();
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
                            earningDeductions: vm.earningDeductions,
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

        function bulkUploadDTRClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'bulkUploadDTRModal.html',
                controller: 'BulkUploadDTRModalCtrl',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    params: function () {
                        return {
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

        function bulkUploadEDRClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'bulkUploadEDRModal.html',
                controller: 'BulkUploadEDRModalCtrl',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    params: function () {
                        return {
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

        function clientChanged() {
            populatePayrollPeriodsSelection();
        };

        function nextClicked() {
            if (vm.searchModel.pageNumber == vm.lastPageNumber) return;

            vm.searchModel.pageNumber += 1;
            searchClicked();
        };

        function onSearchModelChange(newValue, oldValue) {
            if (!vm.searchModel.client || vm.searchModel.client.id <= 0) return;

            searchClicked();
        };

        function payrollPeriodChanged() {
            searchClicked();
        };

        function payrollPeriodMonthChanged() {
            searchClicked();
        };

        function payrollPeriodYearChanged() {
            searchClicked();
        };

        function previousClicked() {
            if (vm.searchModel.pageNumber == 1) return;

            vm.searchModel.pageNumber -= 1;
            searchClicked();
        };

        function populatePayrollPeriodsSelection() {
            if (!vm.searchModel.client || !vm.searchModel.client.id) {
                vm.payrollPeriods = [{ value: '', text: '-- Select a client --' }];
                vm.dailyTimeRecordPayrollPeriodBasis = vm.payrollPeriods[0];
                vm.payrollPeriodSelectionDisabled = true;
                return;
            }

            vm.payrollPeriodSelectionDisabled = true;
            vm.payrollPeriodSelectionLoading = true;

            $http.get('/DailyTimeRecords/PayrollPeriodSelection', { params: { clientId: vm.searchModel.client.id } }).then(function (response) {
                vm.payrollPeriods = response.data.payrollPeriods;

                if (!vm.payrollPeriods.length) {
                    vm.payrollPeriods.push({ value: '', text: 'No previous payroll records found' });
                }
                else {
                    vm.payrollPeriods.splice(0, 0, { value: '', text: '-- Select a payroll period --' });
                    vm.payrollPeriodSelectionDisabled = false;
                }

                vm.payrollPeriodSelectionLoading = false;
                vm.dailyTimeRecordPayrollPeriodBasis = vm.payrollPeriods[0];
            });
        };

        function searchClicked() {
            // Prevent calling the API when the search parameters have not changed
            if (angular.equals(vm.searchModelCurrent, vm.searchModel)) {
                return;
            }

            vm.searchModelCurrent = angular.copy(vm.searchModel);

            if (vm.searchModel.client && vm.searchModel.client.id > 0) {
                vm.searchModel.clientId = vm.searchModel.client.id;
            }
            else {
                vm.searchModel.client.id = undefined;
            }

            if (vm.dailyTimeRecordPayrollPeriodBasis && vm.dailyTimeRecordPayrollPeriodBasis.value > 0) {
                vm.searchModel.dailyTimeRecordPayrollPeriodBasisId = vm.dailyTimeRecordPayrollPeriodBasis.value;
            }
            else {
                vm.searchModel.dailyTimeRecordPayrollPeriodBasisId = undefined;
            }

            vm.searchInProgress = true;
            
            $http.get('/Employees/GetByClientId', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });

            $http.get('/DailyTimeRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.dailyTimeRecords = response.data.dailyTimeRecords;
                vm.totalResultsCount = response.data.totalResultsCount;
                vm.lastPageNumber = response.data.lastPageNumber;
                vm.searchInProgress = false;
            });
        };
    };
}());
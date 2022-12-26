(function () {
    angular
        .module('app')
        .controller('EmployeeRateIndexCtrl', ['$http', '$timeout', '$uibModal', '$scope', EmployeeRateIndexCtrl]);

    function EmployeeRateIndexCtrl($http, $timeout, $uibModal, $scope) {
        var vm = this;
        vm.bulkChangeClicked = bulkChangeClicked;
        vm.cancelEditEmployeeRateClicked = cancelEditEmployeeRateClicked;
        vm.clientsList = [];
        vm.currencySymbol = 'P';
        vm.editEmployeeRateClicked = editEmployeeRateClicked;
        vm.employeeRates = [];
        vm.lastPageNumber = 1;
        vm.nextClicked = nextClicked;
        vm.previousClicked = previousClicked;
        vm.saveEditEmployeeRateClicked = saveEditEmployeeRateClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1 };
        vm.searchInProgress = false;
        vm.totalResultsCount = 0;

        $timeout(function () {
            vm.clientsList = vm.serverModel.clientsList;
            vm.clientsList.splice(0, 0, { text: '-- All Clients --' });
        });

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function bulkChangeClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'bulkChangeModal.html',
                controller: 'BulkChangeModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clientsList
                        };
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();

                alert('Successfully bulk changed!');
            }, function () {
                searchClicked();
            });
        };

        function cancelEditEmployeeRateClicked(employee) {
            employee.isEditing = false;
            employee.copy = {};
        };

        function editEmployeeRateClicked(employee) {
            employee.isEditing = true;
            employee.copy = angular.copy(employee);
        };

        function nextClicked() {
            if (vm.searchModel.pageNumber == vm.lastPageNumber) return;

            vm.searchModel.pageNumber += 1;
            searchClicked();
        };

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function previousClicked() {
            if (vm.searchModel.pageNumber == 1) return;

            vm.searchModel.pageNumber -= 1;
            searchClicked();
        };

        function saveEditEmployeeRateClicked(employee) {
            var action = '/EmployeeRates/Edit';
            var data = employee.copy;

            $http.post(action, data).then(function (response) {
                employee.hourlyRate = employee.copy.hourlyRate;
                employee.dailyRate = employee.copy.dailyRate;
                employee.monthlyRate = employee.copy.monthlyRate;
                employee.colaHourly = employee.copy.colaHourly;
                employee.colaDaily = employee.copy.colaDaily;
                employee.colaMonthly = employee.copy.colaMonthly;
                employee.isEditing = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function searchClicked() {
            if (vm.searchModel.client && vm.searchModel.client.value) {
                vm.searchModel.clientId = parseInt(vm.searchModel.client.value);
            }
            else {
                vm.searchModel.clientId = undefined;
            }
            vm.searchInProgress = true;

            $http.get('/EmployeeRates/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.totalResultsCount = response.data.totalResultsCount;
                vm.lastPageNumber = response.data.lastPageNumber;
                
                vm.searchInProgress = false;
            });
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('ForProcessingCtrl', ['$http', '$timeout', '$uibModal', '$scope', '$window', 'globalSettings', ForProcessingCtrl]);

    function ForProcessingCtrl($http, $timeout, $uibModal, $scope, $window, globalSettings) {
        var vm = this;
        vm.addToQueueClicked = addToQueueClicked;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.employees = [];
        vm.filteredEmployees = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;
        vm.validationErrors = {};

        $scope.$watch('vm.localSearchTerm', onLocalSearchTermChange);

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { code: '-- Select a client --', name: '-- Select a client --' });
        });

        function addToQueueClicked() {
            var includedEmployeeIds = [];

            for (var i = 0; i < vm.filteredEmployees.length; i++) {
                if (vm.filteredEmployees[i].includeInBatch === true) {
                    includedEmployeeIds.push(vm.filteredEmployees[i].id);
                }
            }

            var employeeIds = includedEmployeeIds.join();

            var action = '/Payroll/AddForProcessingBatch';
            var data = {
                clientId: vm.searchModel.clientId,
                employeeIds: employeeIds,
                payrollPeriodFrom: vm.payrollPeriodFrom,
                payrollPeriodTo: vm.payrollPeriodTo,
                payrollPeriodMonth: vm.payrollPeriodMonth
            };

            vm.searchInProgress = true;

            $http.post(action, data).then(function (response) {
                $window.location = '/Payroll/ForProcessingQueue';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.searchInProgress = false;
            });
        };

        function onLocalSearchTermChange(newValue, oldValue) {
            if (!newValue) {
                vm.filteredEmployees = angular.copy(vm.employees);
                return;
            }

            var trimmed = newValue.trim().toUpperCase();

            filteredEmployees = [];
            for (var i = 0; i < vm.employees.length; i++) {
                var employee = vm.employees[i];

                var match = (employee.employeeCode && employee.employeeCode.toUpperCase().includes(trimmed)) ||
                    (employee.firstName && employee.firstName.toUpperCase().includes(trimmed)) ||
                    (employee.lastName && employee.lastName.toUpperCase().includes(trimmed)) ||
                    (employee.middleInitial && employee.middleInitial.toUpperCase().includes(trimmed)) ||
                    (employee.resignStatus && employee.resignStatus.toUpperCase().includes(trimmed));

                if (match) {
                    filteredEmployees.push(employee);
                }
            }

            vm.filteredEmployees = filteredEmployees;
        };

        function searchClicked() {
            console.log('searchClicked');
            if (vm.searchModel.client && vm.searchModel.client.id > 0) {
                vm.searchModel.clientId = vm.searchModel.client.id;

                vm.employees = [];
                vm.filteredEmployees = [];
                vm.searchInProgress = true;

                $http.get('/Employees/GetByClientId', { params: vm.searchModel }).then(function (response) {
                    vm.employees = response.data.employees;

                    for (var i = 0; i < vm.employees.length; i++) {
                        vm.employees[i].includeInBatch = false;

                        vm.filteredEmployees.push(vm.employees[i]);
                    }

                    vm.searchInProgress = false;
                });
            }
            else {
                vm.searchModel.clientId = undefined;
                vm.employees = [];
                vm.filteredEmployees = [];
            }
        };
    };
}());
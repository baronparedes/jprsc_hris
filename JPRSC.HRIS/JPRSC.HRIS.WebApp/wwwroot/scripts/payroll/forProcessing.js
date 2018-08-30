(function () {
    angular
        .module('app')
        .controller('ForProcessingCtrl', ['$http', '$timeout', '$uibModal', '$scope', '$window', 'globalSettings', ForProcessingCtrl]);

    function ForProcessingCtrl($http, $timeout, $uibModal, $scope, $window, globalSettings) {
        var vm = this;
        vm.addToQueueClicked = addToQueueClicked;
        vm.employees = [];
        vm.filteredEmployees = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.localSearchTerm', onLocalSearchTermChange);

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { code: '-- Select a client --', name: '-- Select a client --' });
        });

        function addToQueueClicked() {
            console.log('addToQueueClicked');

            var includedEmployeeIds = [];

            for (var i = 0; i < vm.filteredEmployees.length; i++) {
                if (vm.filteredEmployees[i].includeInBatch === true) {
                    includedEmployeeIds.push(vm.filteredEmployees[i].id);
                    console.log(vm.filteredEmployees[i]);
                }
            }

            var employeeIds = includedEmployeeIds.join();

            console.log(employeeIds);

            var action = '/Payroll/AddForProcessingBatch';
            var data = {
                clientId: vm.searchModel.clientId,
                employeeIds: employeeIds
            };

            vm.searchInProgresss = true;

            $http.post(action, data).then(function (response) {
                console.log('success');
                $window.location = '/Payroll/ForProcessingQueue';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.searchInProgresss = false;
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
                    (employee.middleInitial && employee.middleInitial.toUpperCase().includes(trimmed));

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
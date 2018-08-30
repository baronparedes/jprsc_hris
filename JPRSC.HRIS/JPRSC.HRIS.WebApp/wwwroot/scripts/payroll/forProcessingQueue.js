(function () {
    angular
        .module('app')
        .controller('ForProcessingQueueCtrl', ['$http', '$timeout', '$uibModal', '$scope', 'globalSettings', ForProcessingQueueCtrl]);

    function ForProcessingQueueCtrl($http, $timeout, $uibModal, $scope, globalSettings) {
        var vm = this;
        vm.downloadQueueClicked = downloadQueueClicked;
        vm.employees = [];
        vm.filteredEmployees = [];
        //vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.localSearchTerm', onLocalSearchTermChange);

        $timeout(function () {
            vm.forProcessingBatches = vm.serverModel.forProcessingBatches;
        });

        function downloadQueueClicked(forProcessingBatch) {
            console.log('downloadQueueClicked');
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

        //function searchClicked() {
        //    console.log('searchClicked');
        //    if (vm.searchModel.client && vm.searchModel.client.id > 0) {
        //        vm.searchModel.clientId = vm.searchModel.client.id;

        //        vm.employees = [];
        //        vm.filteredEmployees = [];
        //        vm.searchInProgress = true;

        //        $http.get('/Employees/GetByClientId', { params: vm.searchModel }).then(function (response) {
        //            vm.employees = response.data.employees;

        //            for (var i = 0; i < vm.employees.length; i++) {
        //                vm.employees[i].includeInBatch = false;

        //                vm.filteredEmployees.push(vm.employees[i]);
        //            }

        //            vm.searchInProgress = false;
        //        });
        //    }
        //    else {
        //        vm.searchModel.clientId = undefined;
        //        vm.employees = [];
        //        vm.filteredEmployees = [];
        //    }


            
        //};
    };
}());
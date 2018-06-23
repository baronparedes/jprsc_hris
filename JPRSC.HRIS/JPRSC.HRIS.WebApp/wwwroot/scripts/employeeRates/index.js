(function () {
    angular
        .module('app')
        .controller('EmployeeRateIndexCtrl', ['$http', '$timeout', '$scope', EmployeeRateIndexCtrl]);

    function EmployeeRateIndexCtrl($http, $timeout, $scope) {
        var vm = this;
        vm.cancelEditEmployeeRateClicked = cancelEditEmployeeRateClicked;
        vm.client = {};
        vm.clientsList = [];
        vm.currencySymbol = 'P';
        vm.editEmployeeRateClicked = editEmployeeRateClicked;
        vm.employeeRates = [];
        vm.saveEditEmployeeRateClicked = saveEditEmployeeRateClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $timeout(function () {
            vm.clientsList = vm.serverModel.clientsList;
        });

        $scope.$watch('vm.searchModel', onSearchModelChange, true);
        $scope.$watch('vm.client', onSearchModelChange, true);

        function cancelEditEmployeeRateClicked(employee) {
            employee.isEditing = false;
            employee.copy = {};
        };

        function editEmployeeRateClicked(employee) {
            employee.isEditing = true;
            employee.copy = angular.copy(employee);
        };

        function onSearchModelChange(newValue, oldValue) {
            if (!vm.client.value || !vm.client.value.length) return;

            searchClicked();
        };

        function saveEditEmployeeRateClicked(employee) {
            var action = '/EmployeeRates/Edit';
            var data = employee.copy;

            $http.post(action, data).then(function (response) {
                employee.hourlyRate = employee.copy.hourlyRate;
                employee.dailyRate = employee.copy.dailyRate;
                employee.colaHourly = employee.copy.colaHourly;
                employee.colaDaily = employee.copy.colaDaily;
                employee.isEditing = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function searchClicked() {
            vm.searchModel.clientId = parseInt(vm.client.value);
            vm.searchInProgress = true;

            $http.get('/EmployeeRates/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });
        };
    };
}());
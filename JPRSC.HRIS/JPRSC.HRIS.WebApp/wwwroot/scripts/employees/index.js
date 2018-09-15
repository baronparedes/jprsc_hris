(function () {
    angular
        .module('app')
        .controller('EmployeeIndexCtrl', ['$http', '$scope', '$timeout', EmployeeIndexCtrl]);

    function EmployeeIndexCtrl($http, $scope, $timeout) {
        var vm = this;
        vm.clients = [];
        vm.employees = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1, pageSize: 100 };
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { code: '-- Select a client --', name: '-- Select a client --' });
        });

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            if (vm.searchModel.client && vm.searchModel.client.id > 0) {
                vm.searchModel.clientId = vm.searchModel.client.id;
            }
            else {
                vm.searchModel.clientId = undefined;
            }

            vm.searchInProgress = true;

            $http.get('/Employees/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });
        };
    };
}());
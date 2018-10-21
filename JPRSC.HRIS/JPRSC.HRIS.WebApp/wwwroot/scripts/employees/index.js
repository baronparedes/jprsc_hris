(function () {
    angular
        .module('app')
        .controller('EmployeeIndexCtrl', ['$http', '$scope', '$timeout', '$uibModal', EmployeeIndexCtrl]);

    function EmployeeIndexCtrl($http, $scope, $timeout, $uibModal) {
        var vm = this;
        vm.clients = [];
        vm.employees = [];
        vm.lastPageNumber = 1;
        vm.nextClicked = nextClicked;
        vm.previousClicked = previousClicked;
        vm.rehireAndTransferClicked = rehireAndTransferClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1 };
        vm.searchInProgress = false;
        vm.shouldEnablePrevious = true;
        vm.shouldDisablePrevious = true;
        vm.totalResultsCount = 0;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
            vm.clients.splice(0, 0, { code: '-- Select a client --', name: '-- Select a client --' });
        });

        function nextClicked() {
            if (vm.searchModel.pageNumber == vm.lastPageNumber) return;

            vm.searchModel.pageNumber += 1;
            searchClicked();
        };

        function previousClicked() {
            if (vm.searchModel.pageNumber == 1) return;

            vm.searchModel.pageNumber -= 1;
            searchClicked();
        };

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function rehireAndTransferClicked(employee) {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'rehireTransferModal.html',
                controller: 'RehireTransferModalCtrl',
                controllerAs: 'vm',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clients,
                            employee: employee
                        }
                    }
                }
            });

            modalInstance.result.then(function (result) {
                searchClicked();

                alert('Successfully transferred!');
            }, function () {
                searchClicked();
            });
        };

        function searchClicked() {
            if (vm.searchModel.client && vm.searchModel.client.id > 0) {

                if (vm.searchModel.clientId != vm.searchModel.client.id) {
                    vm.searchModel.pageNumber = 1;
                }
                    
                vm.searchModel.clientId = vm.searchModel.client.id;
            }
            else {
                vm.pageNumber = 1;
                vm.searchModel.clientId = undefined;
            }

            vm.searchInProgress = true;

            $http.get('/Employees/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.totalResultsCount = response.data.totalResultsCount;
                vm.lastPageNumber = response.data.lastPageNumber;
                
                vm.searchInProgress = false;
            });
        };
    };
}());
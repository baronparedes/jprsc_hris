(function () {
    angular
        .module('app')
        .controller('PayrollEndProcessCtrl', ['$http', '$scope', '$timeout', '$uibModal', '$window', 'globalSettings', PayrollEndProcessCtrl]);

    function PayrollEndProcessCtrl($http, $scope, $timeout, $uibModal, $window, globalSettings) {
        var vm = this;
        vm.endProcessClicked = endProcessClicked;
        vm.payrollProcessBatches = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $timeout(function () {
            vm.clients = vm.serverModel.clients;
        });

        searchClicked();

        function endProcessClicked() {
            var modalInstance = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'endProcessModal.html',
                controller: 'EndProcessModalCtrl',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    params: function () {
                        return {
                            clients: vm.clients
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
            vm.searchInProgress = true;

            $http.get('/Payroll/EndProcessQuery', { params: vm.searchModel }).then(function (response) {
                vm.payrollProcessBatches = response.data.payrollProcessBatches;
                vm.searchInProgress = false;
            });
        };
    };
}());
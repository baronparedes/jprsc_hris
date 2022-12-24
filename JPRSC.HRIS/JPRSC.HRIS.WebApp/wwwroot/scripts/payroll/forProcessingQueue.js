(function () {
    angular
        .module('app')
        .controller('ForProcessingQueueCtrl', ['$http', '$scope', '$timeout', '$uibModal', '$scope', 'globalSettings', ForProcessingQueueCtrl]);

    function ForProcessingQueueCtrl($http, $scope, $timeout, $uibModal, $scope, globalSettings) {
        var vm = this;
        vm.forProcessingBatches = [];
        vm.lastPageNumber = 1;
        vm.nextClicked = nextClicked;
        vm.previousClicked = previousClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1 };
        vm.searchInProgress = false;
        vm.totalResultsCount = 0;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

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

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Payroll/ForProcessingBatchSearch', { params: vm.searchModel }).then(function (response) {
                vm.forProcessingBatches = response.data.forProcessingBatches;
                vm.totalResultsCount = response.data.totalResultsCount;
                vm.lastPageNumber = response.data.lastPageNumber;

                vm.searchInProgress = false;
            });
        };
    };
}());
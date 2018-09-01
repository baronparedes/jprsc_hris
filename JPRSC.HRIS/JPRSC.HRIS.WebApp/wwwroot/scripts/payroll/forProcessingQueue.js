(function () {
    angular
        .module('app')
        .controller('ForProcessingQueueCtrl', ['$http', '$uibModal', '$scope', 'globalSettings', ForProcessingQueueCtrl]);

    function ForProcessingQueueCtrl($http,  $uibModal, $scope, globalSettings) {
        var vm = this;
        vm.downloadQueueClicked = downloadQueueClicked;
        vm.forProcessingBatches = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function downloadQueueClicked(forProcessingBatch) {
            console.log('downloadQueueClicked');
        };

        function searchClicked() {
            $http.get('/Payroll/ForProcessingBatchSearch', { params: vm.searchModel }).then(function (response) {
                vm.forProcessingBatches = response.data.forProcessingBatches;

                vm.searchInProgress = false;
            });
        };
    };
}());
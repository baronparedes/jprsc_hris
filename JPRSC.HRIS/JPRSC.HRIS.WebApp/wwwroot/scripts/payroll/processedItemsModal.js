(function () {
    angular
        .module('app')
        .controller('ProcessedItemsModalCtrl', ['$http', '$scope', '$timeout', '$uibModal', '$uibModalInstance', 'lookups', 'params', ProcessedItemsModalCtrl]);

    function ProcessedItemsModalCtrl($http, $scope, $timeout, $uibModal, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.processedItems = params.processedItems;
        vm.showProcessedItems = params.showProcessedItems;
        vm.showSkippedItems = params.showSkippedItems;
        vm.skippedItems = params.skippedItems;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('AuditTrailIndexCtrl', ['$http', '$scope', AuditTrailIndexCtrl]);

    function AuditTrailIndexCtrl($http, $scope) {
        var vm = this;
        vm.auditTrails = [];
        vm.nextClicked = nextClicked;
        vm.previousClicked = previousClicked;
        vm.searchClicked = searchClicked;
        vm.searchModel = { pageNumber: 1, pageSize: 100 };
        vm.searchInProgress = false;
        vm.totalResultsCount = 0;

        searchClicked();

        function nextClicked() {
            if (vm.totalResultsCount == 0 || vm.searchInProgress || vm.totalResultsCount - (vm.searchModel.pageSize * vm.searchModel.pageNumber) <= vm.searchModel.pageSize) return;

            vm.searchModel.pageNumber = vm.searchModel.pageNumber + 1;
            searchClicked();
        };

        function previousClicked() {
            if (vm.totalResultsCount == 0 || vm.searchInProgress || vm.searchModel.pageNumber == 1) return;

            vm.searchModel.pageNumber = vm.searchModel.pageNumber - 1;
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/AuditTrails/Search', { params: vm.searchModel }).then(function (response) {
                vm.auditTrails = response.data.auditTrails;
                vm.totalResultsCount = response.data.totalResultsCount;
                vm.searchInProgress = false;
            });
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('PagIbigRecordIndexCtrl', ['$http', '$scope', PagIbigRecordIndexCtrl]);

    function PagIbigRecordIndexCtrl($http, $scope) {
        var vm = this;
        vm.pagIbigRecords = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/PagIbigRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.pagIbigRecords = response.data.pagIbigRecords;
                vm.searchInProgress = false;
            });
        };
    };
}());
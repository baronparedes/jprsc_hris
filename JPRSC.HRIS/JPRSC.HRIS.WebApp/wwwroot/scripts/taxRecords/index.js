(function () {
    angular
        .module('app')
        .controller('TaxRecordIndexCtrl', ['$http', '$scope', TaxRecordIndexCtrl]);

    function TaxRecordIndexCtrl($http, $scope) {
        var vm = this;
        vm.currencySymbol = 'P';
        vm.taxRecords = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/TaxRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.taxRecords = response.data.taxRecords;
                vm.searchInProgress = false;
            });
        };
    };
}());
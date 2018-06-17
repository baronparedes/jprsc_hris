(function () {
    angular
        .module('app')
        .controller('PhicRecordIndexCtrl', ['$http', '$scope', PhicRecordIndexCtrl]);

    function PhicRecordIndexCtrl($http, $scope) {
        var vm = this;
        vm.phicRecords = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/PhicRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.phicRecords = response.data.phicRecords;
                vm.searchInProgress = false;
            });
        };
    };
}());
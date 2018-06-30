(function () {
    angular
        .module('app')
        .controller('DailyTimeRecordIndexCtrl', ['$http', '$scope', DailyTimeRecordIndexCtrl]);

    function DailyTimeRecordIndexCtrl($http, $scope) {
        var vm = this;
        vm.dailyTimeRecords = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/DailyTimeRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.dailyTimeRecords = response.data.dailyTimeRecords;
                vm.searchInProgress = false;
            });
        };
    };
}());
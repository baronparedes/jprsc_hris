(function () {
    angular
        .module('app')
        .controller('JobTitleIndexCtrl', ['$http', '$scope', JobTitleIndexCtrl]);

    function JobTitleIndexCtrl($http, $scope) {
        var vm = this;
        vm.jobTitles = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/JobTitles/Search', { params: vm.searchModel }).then(function (response) {
                vm.jobTitles = response.data.jobTitles;
                vm.searchInProgress = false;
            });
        };
    };
}());
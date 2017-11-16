(function () {
    angular
        .module('app')
        .controller('CompanyIndexCtrl', ['$http', '$scope', CompanyIndexCtrl]);

    function CompanyIndexCtrl($http, $scope) {
        var vm = this;
        vm.companies = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Companies/Search', { params: vm.searchModel }).then(function (response) {
                vm.companies = response.data.companies;
                vm.searchInProgress = false;
            });
        };
    };
}());
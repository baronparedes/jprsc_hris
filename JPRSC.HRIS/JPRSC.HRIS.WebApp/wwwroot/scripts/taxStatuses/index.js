(function () {
    angular
        .module('app')
        .controller('TaxStatusIndexCtrl', ['$http', '$scope', TaxStatusIndexCtrl]);

    function TaxStatusIndexCtrl($http, $scope) {
        var vm = this;
        vm.taxStatuses = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/TaxStatuses/Search', { params: vm.searchModel }).then(function (response) {
                vm.taxStatuses = response.data.taxStatuses;
                vm.searchInProgress = false;
            });
        };
    };
}());
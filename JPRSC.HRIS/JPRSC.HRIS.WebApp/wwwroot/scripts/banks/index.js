(function () {
    angular
        .module('app')
        .controller('BankIndexCtrl', ['$http', '$scope', BankIndexCtrl]);

    function BankIndexCtrl($http, $scope) {
        var vm = this;
        vm.banks = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Banks/Search', { params: vm.searchModel }).then(function (response) {
                vm.banks = response.data.banks;
                vm.searchInProgress = false;
            });
        };
    };
}());
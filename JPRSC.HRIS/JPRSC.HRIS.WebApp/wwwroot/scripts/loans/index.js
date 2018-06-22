(function () {
    angular
        .module('app')
        .controller('LoanIndexCtrl', ['$http', '$scope', LoanIndexCtrl]);

    function LoanIndexCtrl($http, $scope) {
        var vm = this;
        vm.loans = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Loans/Search', { params: vm.searchModel }).then(function (response) {
                vm.loans = response.data.loans;
                vm.searchInProgress = false;
            });
        };
    };
}());
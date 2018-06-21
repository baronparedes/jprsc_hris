(function () {
    angular
        .module('app')
        .controller('LoanTypeIndexCtrl', ['$http', '$scope', LoanTypeIndexCtrl]);

    function LoanTypeIndexCtrl($http, $scope) {
        var vm = this;
        vm.loanTypes = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/LoanTypes/Search', { params: vm.searchModel }).then(function (response) {
                vm.loanTypes = response.data.loanTypes;
                vm.searchInProgress = false;
            });
        };
    };
}());
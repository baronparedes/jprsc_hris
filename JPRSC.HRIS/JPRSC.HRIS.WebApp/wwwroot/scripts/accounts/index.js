(function () {
    angular
        .module('app')
        .controller('AccountIndexCtrl', ['$http', '$scope', AccountIndexCtrl]);

    function AccountIndexCtrl($http, $scope) {
        var vm = this;
        vm.accounts = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Accounts/Search', { params: vm.searchModel }).then(function (response) {
                console.log(response.data);
                vm.accounts = response.data.accounts;
                vm.searchInProgress = false;
            });
        };
    };
}());
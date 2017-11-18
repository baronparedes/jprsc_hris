(function () {
    angular
        .module('app')
        .controller('ClientIndexCtrl', ['$http', '$scope', ClientIndexCtrl]);

    function ClientIndexCtrl($http, $scope) {
        var vm = this;
        vm.clients = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Clients/Search', { params: vm.searchModel }).then(function (response) {
                vm.clients = response.data.clients;
                vm.searchInProgress = false;
            });
        };
    };
}());
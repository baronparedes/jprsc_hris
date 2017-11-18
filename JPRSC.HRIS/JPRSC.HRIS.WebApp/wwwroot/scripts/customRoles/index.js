(function () {
    angular
        .module('app')
        .controller('CustomRoleIndexCtrl', ['$http', '$scope', CustomRoleIndexCtrl]);

    function CustomRoleIndexCtrl($http, $scope) {
        var vm = this;
        vm.customRoles = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/CustomRoles/Search', { params: vm.searchModel }).then(function (response) {
                vm.customRoles = response.data.customRoles;
                vm.searchInProgress = false;
            });
        };
    };
}());
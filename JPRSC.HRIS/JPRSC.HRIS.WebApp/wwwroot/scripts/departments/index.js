(function () {
    angular
        .module('app')
        .controller('DepartmentIndexCtrl', ['$http', '$scope', DepartmentIndexCtrl]);

    function DepartmentIndexCtrl($http, $scope) {
        var vm = this;
        vm.departments = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Departments/Search', { params: vm.searchModel }).then(function (response) {
                vm.departments = response.data.departments;
                vm.searchInProgress = false;
            });
        };
    };
}());
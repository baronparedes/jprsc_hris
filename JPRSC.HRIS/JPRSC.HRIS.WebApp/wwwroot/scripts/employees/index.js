(function () {
    angular
        .module('app')
        .controller('EmployeeIndexCtrl', ['$http', '$scope', EmployeeIndexCtrl]);

    function EmployeeIndexCtrl($http, $scope) {
        var vm = this;
        vm.employees = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Employees/Search', { params: vm.searchModel }).then(function (response) {
                vm.employees = response.data.employees;
                vm.searchInProgress = false;
            });
        };
    };
}());
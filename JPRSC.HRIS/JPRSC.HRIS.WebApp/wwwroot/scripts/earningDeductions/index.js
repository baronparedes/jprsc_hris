(function () {
    angular
        .module('app')
        .controller('EarningDeductionIndexCtrl', ['$http', '$scope', EarningDeductionIndexCtrl]);

    function EarningDeductionIndexCtrl($http, $scope) {
        var vm = this;
        vm.earningDeductions = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/EarningDeductions/Search', { params: vm.searchModel }).then(function (response) {
                vm.earningDeductions = response.data.earningDeductions;
                vm.searchInProgress = false;
            });
        };
    };
}());
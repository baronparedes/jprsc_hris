(function () {
    angular
        .module('app')
        .controller('ApprovalLevelIndexCtrl', ['$http', '$scope', ApprovalLevelIndexCtrl]);

    function ApprovalLevelIndexCtrl($http, $scope) {
        var vm = this;
        vm.approvalLevels = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/ApprovalLevels/Search', { params: vm.searchModel }).then(function (response) {
                vm.approvalLevels = response.data.approvalLevels;
                vm.searchInProgress = false;
            });
        };
    };
}());
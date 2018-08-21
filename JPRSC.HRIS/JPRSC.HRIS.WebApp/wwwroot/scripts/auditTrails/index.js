(function () {
    angular
        .module('app')
        .controller('AuditTrailIndexCtrl', ['$http', '$scope', AuditTrailIndexCtrl]);

    function AuditTrailIndexCtrl($http, $scope) {
        var vm = this;
        vm.auditTrails = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/AuditTrails/Search', { params: vm.searchModel }).then(function (response) {
                vm.auditTrails = response.data.auditTrails;
                vm.searchInProgress = false;
            });
        };
    };
}());
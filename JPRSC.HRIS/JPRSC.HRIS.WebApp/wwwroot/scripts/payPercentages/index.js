(function () {
    angular
        .module('app')
        .controller('PayPercentageIndexCtrl', ['$http', '$scope', PayPercentageIndexCtrl]);

    function PayPercentageIndexCtrl($http, $scope) {
        var vm = this;
        vm.cancelEditPayPercentageClicked = cancelEditPayPercentageClicked;
        vm.editPayPercentageClicked = editPayPercentageClicked;
        vm.saveEditPayPercentageClicked = saveEditPayPercentageClicked;
        vm.payPercentages = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function cancelEditPayPercentageClicked(payPercentage) {
            payPercentage.isEditing = false;
            payPercentage.copy = {};
        };

        function editPayPercentageClicked(payPercentage) {
            payPercentage.isEditing = true;
            payPercentage.copy = angular.copy(payPercentage);
        };

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function saveEditPayPercentageClicked(payPercentage) {
            var action = '/PayPercentages/Edit';
            var data = payPercentage.copy;

            $http.post(action, data).then(function (response) {
                payPercentage.isEditing = false;
                payPercentage.name = payPercentage.copy.name;
                payPercentage.percentage = payPercentage.copy.percentage;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/PayPercentages/Search', { params: vm.searchModel }).then(function (response) {
                vm.payPercentages = response.data.payPercentages;
                vm.searchInProgress = false;
            });
        };
    };
}());
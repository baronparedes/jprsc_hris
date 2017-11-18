(function () {
    angular
        .module('app')
        .controller('ReligionIndexCtrl', ['$http', '$scope', ReligionIndexCtrl]);

    function ReligionIndexCtrl($http, $scope) {
        var vm = this;
        vm.religions = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/Religions/Search', { params: vm.searchModel }).then(function (response) {
                vm.religions = response.data.religions;
                vm.searchInProgress = false;
            });
        };
    };
}());
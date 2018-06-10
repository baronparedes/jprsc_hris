(function () {
    angular
        .module('app')
        .controller('EditPayPercentageCtrl', ['$http', '$window', EditPayPercentageCtrl]);

    function EditPayPercentageCtrl($http, $window) {
        var vm = this;
        vm.editPayPercentageSubmit = editPayPercentageSubmit;
        vm.validationErrors = {};

        function editPayPercentageSubmit(e) {
            var action = '/PayPercentages/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/PayPercentages/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
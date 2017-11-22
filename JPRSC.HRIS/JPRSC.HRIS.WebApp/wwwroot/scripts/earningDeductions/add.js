(function () {
    angular
        .module('app')
        .controller('AddEarningDeductionCtrl', ['$http', '$window', 'lookups', AddEarningDeductionCtrl]);

    function AddEarningDeductionCtrl($http, $window, lookups) {
        var vm = this;
        vm.addEarningDeductionSubmit = addEarningDeductionSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        function addEarningDeductionSubmit(e) {
            var action = '/EarningDeductions/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/EarningDeductions/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
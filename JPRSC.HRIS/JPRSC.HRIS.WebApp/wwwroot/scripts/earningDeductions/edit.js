(function () {
    angular
        .module('app')
        .controller('EditEarningDeductionCtrl', ['$http', '$timeout', '$window', 'lookups', EditEarningDeductionCtrl]);

    function EditEarningDeductionCtrl($http, $timeout, $window, lookups) {
        var vm = this;
        vm.editEarningDeductionSubmit = editEarningDeductionSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            var earningDeduction = vm.serverModel;
            vm.earningDeductionType = { value: earningDeduction.earningDeductionType };
        });

        function editEarningDeductionSubmit(e) {
            var action = '/EarningDeductions/Edit';
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
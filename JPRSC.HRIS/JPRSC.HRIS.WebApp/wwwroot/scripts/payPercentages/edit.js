(function () {
    angular
        .module('app')
        .controller('EditPayPercentageCtrl', ['$http', '$timeout', '$window', EditPayPercentageCtrl]);

    function EditPayPercentageCtrl($http, $timeout, $window) {
        var vm = this;
        vm.addTaxRangeClicked = addTaxRangeClicked;
        vm.deleteTaxRangeClicked = deleteTaxRangeClicked;
        vm.editPayPercentageSubmit = editPayPercentageSubmit;
        vm.getTaxRangeFieldName = getTaxRangeFieldName;
        vm.taxRanges = [];
        vm.validationErrors = {};

        $timeout(function () {
            var payPercentage = vm.serverModel;
            vm.taxRanges = payPercentage.taxRanges;
            console.log(vm.taxRanges);
        });

        function addTaxRangeClicked() {
            vm.taxRanges.push({});
        };

        function deleteTaxRangeClicked(index) {
            vm.taxRanges.splice(index, 1);
        };

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

        function getTaxRangeFieldName(index, fieldName) {
            return `TaxRanges[${index}].${fieldName}`;
        };
    };
}());
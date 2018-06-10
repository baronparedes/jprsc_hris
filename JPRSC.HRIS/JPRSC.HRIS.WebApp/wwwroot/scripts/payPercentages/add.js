(function () {
    angular
        .module('app')
        .controller('AddPayPercentageCtrl', ['$http', '$window', AddPayPercentageCtrl]);

    function AddPayPercentageCtrl($http, $window) {
        var vm = this;
        vm.addTaxRangeClicked = addTaxRangeClicked;
        vm.addPayPercentageSubmit = addPayPercentageSubmit;
        vm.deleteTaxRangeClicked = deleteTaxRangeClicked;
        vm.getTaxRangeFieldName = getTaxRangeFieldName;
        vm.taxRanges = [];
        vm.validationErrors = {};

        function addTaxRangeClicked() {
            vm.taxRanges.push({});
        };

        function addPayPercentageSubmit(e) {
            var action = '/PayPercentages/Add';
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

        function deleteTaxRangeClicked(index) {
            vm.taxRanges.splice(index, 1);
        };

        function getTaxRangeFieldName(index, fieldName) {
            return `TaxRanges[${index}].${fieldName}`;
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('EditTaxStatusCtrl', ['$http', '$timeout', '$window', EditTaxStatusCtrl]);

    function EditTaxStatusCtrl($http, $timeout, $window) {
        var vm = this;
        vm.addTaxRangeClicked = addTaxRangeClicked;
        vm.deleteTaxRangeClicked = deleteTaxRangeClicked;
        vm.editTaxStatusSubmit = editTaxStatusSubmit;
        vm.getTaxRangeFieldName = getTaxRangeFieldName;
        vm.taxRanges = [];
        vm.validationErrors = {};

        $timeout(function () {
            var taxStatus = vm.serverModel;
            vm.taxRanges = taxStatus.taxRanges;
        });

        function addTaxRangeClicked() {
            vm.taxRanges.push({});
        };

        function deleteTaxRangeClicked(index) {
            vm.taxRanges.splice(index, 1);
        };

        function editTaxStatusSubmit(e) {
            var action = '/TaxStatuses/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/TaxStatuses/Index';
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
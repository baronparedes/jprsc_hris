(function () {
    angular
        .module('app')
        .controller('EditTaxRecordCtrl', ['$http', '$timeout', '$window', EditTaxRecordCtrl]);

    function EditTaxRecordCtrl($http, $timeout, $window) {
        var vm = this;
        vm.addTaxRangeClicked = addTaxRangeClicked;
        vm.deleteTaxRangeClicked = deleteTaxRangeClicked;
        vm.editTaxRecordSubmit = editTaxRecordSubmit;
        vm.getTaxRangeFieldName = getTaxRangeFieldName;
        vm.taxRanges = [];
        vm.validationErrors = {};

        $timeout(function () {
            var taxRecord = vm.serverModel;
            vm.taxRanges = taxRecord.taxRanges;
        });

        function addTaxRangeClicked() {
            vm.taxRanges.push({});
        };

        function deleteTaxRangeClicked(index) {
            vm.taxRanges.splice(index, 1);
        };

        function editTaxRecordSubmit(e) {
            var action = '/TaxRecords/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/TaxRecords/Index';
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
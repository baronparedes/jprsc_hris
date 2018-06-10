(function () {
    angular
        .module('app')
        .controller('AddTaxRecordCtrl', ['$http', '$window', AddTaxRecordCtrl]);

    function AddTaxRecordCtrl($http, $window) {
        var vm = this;
        vm.addTaxRecordSubmit = addTaxRecordSubmit;
        vm.taxRanges = [];
        vm.validationErrors = {};

        function addTaxRecordSubmit(e) {
            var action = '/TaxRecords/Add';
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
    };
}());
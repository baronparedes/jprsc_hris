(function () {
    angular
        .module('app')
        .controller('EditTaxRecordCtrl', ['$http', '$window', EditTaxRecordCtrl]);

    function EditTaxRecordCtrl($http, $window) {
        var vm = this;
        vm.editTaxRecordSubmit = editTaxRecordSubmit;
        vm.validationErrors = {};

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
    };
}());
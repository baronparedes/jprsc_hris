(function () {
    angular
        .module('app')
        .controller('EditTaxStatusCtrl', ['$http', '$window', EditTaxStatusCtrl]);

    function EditTaxStatusCtrl($http, $window) {
        var vm = this;
        vm.editTaxStatusSubmit = editTaxStatusSubmit;
        vm.validationErrors = {};

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
    };
}());
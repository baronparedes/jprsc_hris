(function () {
    angular
        .module('app')
        .controller('AddTaxStatusCtrl', ['$http', '$window', AddTaxStatusCtrl]);

    function AddTaxStatusCtrl($http, $window) {
        var vm = this;
        vm.addTaxStatusSubmit = addTaxStatusSubmit;
        vm.validationErrors = {};

        function addTaxStatusSubmit(e) {
            var action = '/TaxStatuses/Add';
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
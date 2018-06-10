(function () {
    angular
        .module('app')
        .controller('EditPagIbigRecordCtrl', ['$http', '$window', EditPagIbigRecordCtrl]);

    function EditPagIbigRecordCtrl($http, $window) {
        var vm = this;
        vm.editPagIbigRecordSubmit = editPagIbigRecordSubmit;
        vm.validationErrors = {};

        function editPagIbigRecordSubmit(e) {
            var action = '/PagIbigRecords/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/PagIbigRecords/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
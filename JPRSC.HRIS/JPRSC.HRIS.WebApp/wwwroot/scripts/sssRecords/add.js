(function () {
    angular
        .module('app')
        .controller('AddSSSRecordCtrl', ['$http', '$window', AddSSSRecordCtrl]);

    function AddSSSRecordCtrl($http, $window) {
        var vm = this;
        vm.addSSSRecordSubmit = addSSSRecordSubmit;
        vm.validationErrors = {};

        function addSSSRecordSubmit(e) {
            var action = '/SSSRecords/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/SSSRecords/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
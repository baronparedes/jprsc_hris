(function () {
    angular
        .module('app')
        .controller('EditSSSRecordCtrl', ['$http', '$window', EditSSSRecordCtrl]);

    function EditSSSRecordCtrl($http, $window) {
        var vm = this;
        vm.editSSSRecordSubmit = editSSSRecordSubmit;
        vm.validationErrors = {};

        function editSSSRecordSubmit(e) {
            var action = '/SSSRecords/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                alert('SSS record saved!');
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
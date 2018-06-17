(function () {
    angular
        .module('app')
        .controller('EditPhicRecordCtrl', ['$http', '$timeout', '$window', EditPhicRecordCtrl]);

    function EditPhicRecordCtrl($http, $timeout, $window) {
        var vm = this;
        vm.editPhicRecordSubmit = editPhicRecordSubmit;
        vm.phicRecord = {};
        vm.validationErrors = {};

        $timeout(function () {
            vm.phicRecord = vm.serverModel;
        });

        function editPhicRecordSubmit(e) {
            var action = '/PhicRecords/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                alert('PHIC record saved!');
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
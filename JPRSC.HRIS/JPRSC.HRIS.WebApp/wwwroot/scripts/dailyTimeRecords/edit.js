(function () {
    angular
        .module('app')
        .controller('EditDailyTimeRecordCtrl', ['$http', '$timeout', '$window', 'lookups', EditDailyTimeRecordCtrl]);

    function EditDailyTimeRecordCtrl($http, $timeout, $window, lookups) {
        var vm = this;
        vm.editDailyTimeRecordSubmit = editDailyTimeRecordSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            var dailyTimeRecord = vm.serverModel;
            vm.dailyTimeRecordType = { value: dailyTimeRecord.dailyTimeRecordType };
        });

        function editDailyTimeRecordSubmit(e) {
            var action = '/DailyTimeRecords/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/DailyTimeRecords/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
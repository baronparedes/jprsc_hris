(function () {
    angular
        .module('app')
        .controller('AddDailyTimeRecordCtrl', ['$http', '$window', 'lookups', AddDailyTimeRecordCtrl]);

    function AddDailyTimeRecordCtrl($http, $window, lookups) {
        var vm = this;
        vm.addDailyTimeRecordSubmit = addDailyTimeRecordSubmit;
        vm.lookups = lookups;
        vm.validationErrors = {};

        function addDailyTimeRecordSubmit(e) {
            var action = '/DailyTimeRecords/Add';
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
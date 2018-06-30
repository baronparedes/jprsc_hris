(function () {
    angular
        .module('app')
        .controller('AddDailyTimeRecordModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', AddDailyTimeRecordModalCtrl]);

    function AddDailyTimeRecordModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addDailyTimeRecordSubmit = addDailyTimeRecordSubmit;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.currencySymbol = 'P';
        vm.daysWorkedValue = 0;
        vm.employees = params.employees;
        vm.hoursLateValue = 0;
        vm.hoursUndertimeValue = 0;
        vm.hoursWorkedValue = 0;
        vm.lookups = lookups;
        vm.validationErrors = {};

        $timeout(function () {
            console.log('vm.employees', vm.employees);
            vm.employee = vm.employees[0];
        });

        $scope.$watch('vm.daysWorked', function () { if (!vm.employee) return; vm.daysWorkedValue = vm.employee.dailyRate * vm.daysWorked; });
        $scope.$watch('vm.hoursWorked', function () { if (!vm.employee) return; vm.hoursWorkedValue = vm.employee.hourlyRate * vm.hoursWorked; });
        $scope.$watch('vm.hoursLate', function () { if (!vm.employee) return; vm.hoursLateValue = vm.employee.hourlyRate * vm.hoursLate; });
        $scope.$watch('vm.hoursUndertime', function () { if (!vm.employee) return; vm.hoursUndertimeValue = vm.employee.hourlyRate * vm.hoursUndertime; });

        function addDailyTimeRecordSubmit(e) {
            var action = '/DailyTimeRecords/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            console.log('addDailyTimeRecordSubmit');

            //$http.post(action, data, config).then(function (response) {
            //    $uibModalInstance.close();
            //}, function (response) {
            //    if (response.status == 400) {
            //        vm.validationErrors = response.data;
            //    }
            //});
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('AddDailyTimeRecordModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', AddDailyTimeRecordModalCtrl]);

    function AddDailyTimeRecordModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addDailyTimeRecordAndAddAnother = addDailyTimeRecordAndAddAnother;
        vm.addDailyTimeRecordAndExit = addDailyTimeRecordAndExit;
        vm.addDailyTimeRecordSubmit = addDailyTimeRecordSubmit;
        vm.addInProgress = false;
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
            vm.currentEmployeeIndex = 0;
            vm.employee = vm.employees[0];
        });

        $scope.$watch('vm.daysWorked', function () { if (!vm.employee) return; vm.daysWorkedValue = vm.employee.dailyRate * vm.daysWorked; });
        $scope.$watch('vm.hoursWorked', function () { if (!vm.employee) return; vm.hoursWorkedValue = vm.employee.hourlyRate * vm.hoursWorked; });
        $scope.$watch('vm.hoursLate', function () { if (!vm.employee) return; vm.hoursLateValue = vm.employee.hourlyRate * vm.hoursLate; });
        $scope.$watch('vm.hoursUndertime', function () { if (!vm.employee) return; vm.hoursUndertimeValue = vm.employee.hourlyRate * vm.hoursUndertime; });

        function addDailyTimeRecordAndAddAnother() {
            addDailyTimeRecordSubmit(function () {
                if (vm.currentEmployeeIndex < vm.employees.length) {
                    vm.currentEmployeeIndex += 1;
                    vm.employee = vm.employees[vm.currentEmployeeIndex];

                    vm.daysWorked = 0;
                    vm.hoursWorked = 0;
                    vm.hoursLate = 0;
                    vm.hoursUndertime = 0;
                }
            });
        };

        function addDailyTimeRecordAndExit() {
            addDailyTimeRecordSubmit(function () {
                $uibModalInstance.close();
            });
        };

        function addDailyTimeRecordSubmit(successCallback) {
            vm.addInProgress = true;
            var action = '/DailyTimeRecords/Add';
            var data = $('#addDailyTimeRecordForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                if (successCallback) {
                    successCallback();
                }

                vm.addInProgress = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.addInProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
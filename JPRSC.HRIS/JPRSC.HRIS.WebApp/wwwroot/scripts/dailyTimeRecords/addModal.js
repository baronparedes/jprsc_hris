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
        vm.addOvertimeClicked = addOvertimeClicked;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.currencySymbol = 'P';
        vm.daysWorkedValue = 0;
        vm.deleteOvertimeClicked = deleteOvertimeClicked;
        vm.employees = params.employees;
        vm.getOvertimeFieldName = getOvertimeFieldName;
        vm.hoursLateValue = 0;
        vm.hoursUndertimeValue = 0;
        vm.hoursWorkedValue = 0;
        vm.lookups = lookups;
        vm.numberOfHoursChanged = numberOfHoursChanged;
        vm.otRateChanged = otRateChanged;
        vm.overtimes = [];
        vm.payRates = params.payRates;
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
                    vm.overtimes = [];
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

        function addOvertimeClicked() {
            vm.overtimes.push({ numberOfHours: 0 });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function deleteOvertimeClicked(index) {
            vm.overtimes.splice(index, 1);
        };

        function getOvertimeFieldName(index, fieldName) {
            return `Overtimes[${index}].${fieldName}`;
        };

        function numberOfHoursChanged(overtime) {
            recalculateNumberOfHoursValue(overtime);
        };

        function otRateChanged(overtime) {
            recalculateNumberOfHoursValue(overtime);
        };

        function recalculateNumberOfHoursValue(overtime) {
            if (!vm.employee || !overtime.payRate || !overtime.payRate.percentage || !overtime.numberOfHours) return;
            if (parseFloat(overtime.numberOfHours) <= 0) return 0;

            overtime.numberOfHoursValue = vm.employee.hourlyRate * (overtime.payRate.percentage / 100) * parseFloat(overtime.numberOfHours);
        };
    };
}());
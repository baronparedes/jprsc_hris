(function () {
    angular
        .module('app')
        .controller('AddDailyTimeRecordModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', AddDailyTimeRecordModalCtrl]);

    function AddDailyTimeRecordModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addDailyTimeRecordAndAddAnother = addDailyTimeRecordAndAddAnother;
        vm.addDailyTimeRecordAndExit = addDailyTimeRecordAndExit;
        vm.addDailyTimeRecordSubmit = addDailyTimeRecordSubmit;
        vm.addEarningDeductionRecordClicked = addEarningDeductionRecordClicked;
        vm.addInProgress = false;
        vm.addOvertimeClicked = addOvertimeClicked;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.currencySymbol = 'P';
        vm.daysWorkedValue = 0;
        vm.deleteOvertimeClicked = deleteOvertimeClicked;
        vm.earningDeductionRecords = [];
        vm.earningDeductions = params.earningDeductions;
        vm.employees = params.employees;
        vm.getEarningDeductionRecordFieldName = getEarningDeductionRecordFieldName;
        vm.getOvertimeFieldName = getOvertimeFieldName;
        vm.hoursLateValue = 0;
        vm.hoursUndertimeValue = 0;
        vm.hoursWorkedValue = 0;
        vm.lookups = lookups;
        vm.numberOfMinutesChanged = numberOfMinutesChanged;
        vm.otRateChanged = otRateChanged;
        vm.overtimes = [];
        vm.payRates = params.payRates;
        vm.payrollPeriods = [];
        vm.payrollPeriodSelectionDisabled = true;
        vm.validationErrors = {};

        $timeout(function () {
            vm.currentEmployeeIndex = 0;
            vm.employee = vm.employees[0];

            populatePayrollPeriodsSelection();
        });

        $scope.$watch('vm.daysWorked', function () { if (!vm.employee) return; vm.daysWorkedValue = vm.employee.dailyRate * vm.daysWorked; });
        $scope.$watch('vm.minutesWorked', function () { if (!vm.employee) return; vm.hoursWorkedValue = vm.employee.hourlyRate * vm.minutesWorked / 60; });
        $scope.$watch('vm.minutesLate', function () { if (!vm.employee) return; vm.hoursLateValue = vm.employee.hourlyRate * vm.minutesLate / 60; });
        $scope.$watch('vm.minutesUndertime', function () { if (!vm.employee) return; vm.hoursUndertimeValue = vm.employee.hourlyRate * vm.minutesUndertime / 60; });

        function addDailyTimeRecordAndAddAnother() {
            addDailyTimeRecordSubmit(function () {
                if (vm.currentEmployeeIndex < vm.employees.length) {
                    vm.currentEmployeeIndex += 1;
                    vm.employee = vm.employees[vm.currentEmployeeIndex];

                    vm.daysWorked = 0;
                    vm.minutesWorked = 0;
                    vm.minutesLate = 0;
                    vm.minutesUndertime = 0;
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

        function addEarningDeductionRecordClicked() {
            vm.earningDeductionRecords.push({});
        };

        function addOvertimeClicked() {
            vm.overtimes.push({ numberOfMinutes: 0 });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function deleteOvertimeClicked(index) {
            vm.overtimes.splice(index, 1);
        };

        function getEarningDeductionRecordFieldName(index, fieldName) {
            return `EarningDeductionRecords[${index}].${fieldName}`;
        };

        function getOvertimeFieldName(index, fieldName) {
            return `Overtimes[${index}].${fieldName}`;
        };

        function numberOfMinutesChanged(overtime) {
            recalculateNumberOfHoursValue(overtime);
        };

        function otRateChanged(overtime) {
            recalculateNumberOfHoursValue(overtime);
        };

        function populatePayrollPeriodsSelection() {
            vm.payrollPeriodSelectionDisabled = true;

            $http.get('/DailyTimeRecords/PayrollPeriodSelection', { params: { clientId: vm.client.id } }).then(function (response) {
                vm.payrollPeriods = response.data.payrollPeriods;

                if (!vm.payrollPeriods.length) {
                    vm.payrollPeriods.push({ value: '', text: 'No previous unprocessed payroll records found' });
                }
                else {
                    vm.payrollPeriods.splice(0, 0, { value: '', text: '' });
                    vm.payrollPeriodSelectionDisabled = false;
                }

                vm.dailyTimeRecordPayrollPeriodBasis = vm.payrollPeriods[0];
            });
        };

        function recalculateNumberOfHoursValue(overtime) {
            if (!vm.employee || !overtime.payRate || !overtime.payRate.percentage || !overtime.numberOfMinutes) return;
            if (parseFloat(overtime.numberOfMinutes) <= 0) return 0;

            overtime.numberOfHoursValue = vm.employee.hourlyRate * (overtime.payRate.percentage / 100) * parseFloat(overtime.numberOfMinutes) / 60;
        };
    };
}());
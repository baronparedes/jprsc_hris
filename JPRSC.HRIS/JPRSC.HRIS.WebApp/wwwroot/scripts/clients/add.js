(function () {
    angular
        .module('app')
        .controller('AddClientCtrl', ['$http', '$window', 'globalSettings', 'lookups', AddClientCtrl]);

    function AddClientCtrl($http, $window, globalSettings, lookups) {
        var vm = this;
        vm.addClientSubmit = addClientSubmit;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.getPayrollPeriodInput = getPayrollPeriodInput;
        vm.lookups = lookups;
        vm.numberOfPayrollPeriodsAMonth = 0;
        vm.numberOfPayrollPeriodsAMonthChanged = numberOfPayrollPeriodsAMonthChanged;
        vm.pagIbigPayrollPeriods = [];
        vm.phicPayrollPeriods = [];
        vm.sssPayrollPeriods = [];
        vm.taxPayrollPeriods = [];
        vm.validationErrors = {};

        function addClientSubmit(e) {
            var action = '/Clients/Add';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Clients/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function getPayrollPeriodInput(payrollPeriods) {
            if (!payrollPeriods || !payrollPeriods.length) return;

            var selectedPayrollPeriods = [];

            for (var i = 0; i < payrollPeriods.length; i++) {
                var item = payrollPeriods[i];
                if (item.selected === true) {
                    selectedPayrollPeriods.push(item.payrollPeriod.toString());
                }
            }

            return selectedPayrollPeriods.join(',');
        };

        function numberOfPayrollPeriodsAMonthChanged() {
            var payrollPeriods = [];

            for (var i = 0; i < vm.numberOfPayrollPeriodsAMonth; i++) {
                payrollPeriods.push({ payrollPeriod: i + 1, selected: false});
            }

            vm.pagIbigPayrollPeriods = angular.copy(payrollPeriods);
            vm.phicPayrollPeriods = angular.copy(payrollPeriods);
            vm.sssPayrollPeriods = angular.copy(payrollPeriods);
            vm.taxPayrollPeriods = angular.copy(payrollPeriods);
        };
    };
}());
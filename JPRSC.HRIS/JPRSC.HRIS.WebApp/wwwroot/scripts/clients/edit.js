(function () {
    angular
        .module('app')
        .controller('EditClientCtrl', ['$http', '$timeout', '$window', 'globalSettings', 'lookups', EditClientCtrl]);

    function EditClientCtrl($http, $timeout, $window, globalSettings, lookups) {
        var vm = this;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.editClientSubmit = editClientSubmit;
        vm.getPayrollPeriodInput = getPayrollPeriodInput;
        vm.lookups = lookups;
        vm.numberOfPayrollPeriodsAMonth = 0;
        vm.pagIbigPayrollPeriods = [];
        vm.phicPayrollPeriods = [];
        vm.sssPayrollPeriods = [];
        vm.taxPayrollPeriods = [];
        vm.validationErrors = {};

        $timeout(function () {
            var client = vm.serverModel;
            vm.cutOffPeriod = { value: client.cutOffPeriod };
            vm.taxTable = { value: client.taxTable };
            vm.payrollCode = { value: client.payrollCode };
            vm.payrollPeriodMonth = { value: client.payrollPeriodMonth };
            vm.numberOfPayrollPeriodsAMonth = client.numberOfPayrollPeriodsAMonth;
            console.log('client', client);

            //setupPayrollPeriodsSelection();

            vm.pagIbigPayrollPeriods = getPayrollPeriodsSelection(client.pagIbigPayrollPeriod);
            vm.phicPayrollPeriods = getPayrollPeriodsSelection(client.phicPayrollPeriod);
            vm.sssPayrollPeriods = getPayrollPeriodsSelection(client.sssPayrollPeriod);
            vm.taxPayrollPeriods = getPayrollPeriodsSelection(client.taxPayrollPeriod);
        });

        function editClientSubmit(e) {
            var action = '/Clients/Edit';
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

        function getPayrollPeriodsSelection(selectedPayrollPeriod) {
            var payrollPeriods = [];
            var selectedPayrollPeriods = selectedPayrollPeriod ? selectedPayrollPeriod.split(',') : [];

            for (var i = 0; i < vm.numberOfPayrollPeriodsAMonth; i++) {

                var payrollPeriodValue = i + 1;

                payrollPeriods.push({ payrollPeriod: payrollPeriodValue, selected: selectedPayrollPeriods.indexOf(payrollPeriodValue.toString()) != -1 });
            }

            return payrollPeriods;
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('GenerateReportModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', GenerateReportModalCtrl]);

    function GenerateReportModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.alphalistType = '7.1';
        vm.cancel = cancel;
        vm.clientChanged = clientChanged;
        vm.clients = params.clients;
        vm.employees = [];
        vm.fromPayrollPeriod = '1';
        vm.fromPayrollPeriodMonth = '10';
        vm.loadEmployeesInProgress = false;
        vm.payrollPeriodMonth = '-1';
        vm.payrollPeriodYear = new Date().getFullYear().toString();
        vm.payrollPeriodFromYear = new Date().getFullYear().toString();
        vm.payrollPeriodToYear = new Date().getFullYear().toString();
        vm.reportType = params.reportType;
        vm.showEmployeeSelection = false;
        vm.toPayrollPeriod = '1';
        vm.toPayrollPeriodMonth = '10';
        vm.thirteenthMonthFromPayrollPeriod = '1';
        vm.thirteenthMonthFromPayrollPeriodMonth = '10';
        vm.thirteenthMonthPayrollPeriodFromYear = new Date().getFullYear().toString();
        vm.thirteenthMonthPayrollPeriodToYear = new Date().getFullYear().toString();
        vm.thirteenthMonthToPayrollPeriod = '1';
        vm.thirteenthMonthToPayrollPeriodMonth = '10';
        vm.validationErrors = {};

        $timeout(function () {
            if (vm.clients.length && vm.clients[0].id !== -1) {
                vm.clients.splice(0, 0, { id: -1, code: 'All Clients', name: 'All Clients' });
            }

            vm.client = vm.clients[0];
            clientChanged();
        });

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function clientChanged() {
            console.log('clientChanged');
            vm.employee = { id: -1, code: 'All Employees', name: 'All Employees' };

            if (vm.client.id === -1) {
                vm.employees = [];
                vm.showEmployeeSelection = false;
            }
            else {
                vm.loadEmployeesInProgress = true;

                $http.get('/Employees/GetByClientId', { params: { clientId: vm.client.id } }).then(function (response) {
                    vm.employees = response.data.employees;

                    console.log('vm.employees', vm.employees);

                    if (vm.employees.length) {
                        vm.employees.splice(0, 0, { id: -1, code: 'All Employees', name: 'All Employees' });
                        vm.employee = vm.employees[0];
                    }

                    vm.loadEmployeesInProgress = false;
                    vm.showEmployeeSelection = true;
                });
            }
        }
    };
}());
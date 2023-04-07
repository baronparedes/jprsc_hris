(function () {
    angular
        .module('app')
        .controller('AddPayrollModalCtrl', ['$http', '$scope', '$timeout', '$uibModal', '$uibModalInstance', 'lookups', 'params', AddPayrollModalCtrl]);

    function AddPayrollModalCtrl($http, $scope, $timeout, $uibModal, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.addInProgress = false;
        vm.cancel = cancel;
        vm.clientChanged = clientChanged;
        vm.clients = params.clients;
        vm.datepickerOptions = globalSettings.datepickerOptions;
        vm.payrollPeriodMonth = '10';
        vm.processSubmit = processSubmit;
        vm.validationErrors = {};

        $timeout(function () {
            if (!vm.clients[0].id) {
                vm.clients.splice(0, 1);
            }

            if (params.client) {
                vm.client = params.client;
            }
            else {
                vm.client = vm.clients[0];
            }

            clientChanged();
        });

        function processSubmit() {
            vm.processInProgress = true;
            var action = '/Payroll/Process';
            var data = $('#processForm').serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                if (!response.data.errorMessage) {
                    var showProcessedItems = true; // hardcoded for now
                    var showSkippedItems = false; // hardcoded for now

                    if (showProcessedItems || showSkippedItems) {
                        var modalInstance = $uibModal.open({
                            animation: true,
                            ariaLabelledBy: 'modal-title',
                            ariaDescribedBy: 'modal-body',
                            templateUrl: 'processedItemsModal.html',
                            controller: 'ProcessedItemsModalCtrl',
                            controllerAs: 'vm',
                            resolve: {
                                params: function () {
                                    return {
                                        processedItems: response.data.processedItems,
                                        showProcessedItems: showProcessedItems,
                                        showSkippedItems: showSkippedItems,
                                        skippedItems: response.data.skippedItems
                                    }
                                }
                            }
                        });

                        modalInstance.result.then(function (result) {
                            $uibModalInstance.close();
                        }, function () {
                            $uibModalInstance.close();
                        });
                    }
                }
                else {
                    var errorMessage = 'Unable to process. ' + response.data.errorMessage;
                    alert(errorMessage);
                }

                vm.processInProgress = false;
            }, function (response) {
                console.log(response);

                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }

                vm.processInProgress = false;
            });
        };

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        function clientChanged() {
            if (vm.client) {
                vm.payrollPeriod = vm.client.currentPayrollPeriod;
            }
        };
    };
}());
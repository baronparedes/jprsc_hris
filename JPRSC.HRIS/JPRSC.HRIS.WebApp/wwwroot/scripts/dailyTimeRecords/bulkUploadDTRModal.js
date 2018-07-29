(function () {
    angular
        .module('app')
        .controller('BulkUploadDTRModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', BulkUploadDTRModalCtrl]);

    function BulkUploadDTRModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.lookups = lookups;
        vm.uploadInProgress = false;
        vm.validationErrors = {};

        $timeout(function () {
            // Selecting the same file twice, even if the file was modified in between,
            // does not reset the file input, resulting in the original file being
            // uploaded the second time around. To remedy this, we reset the file input
            // every time it is clicked.
            $('input[type=file]').click(function () {
                $(this).attr('value', '');
            });

            $("form#bulkUploadForm").submit(function (e) {
                e.preventDefault();
                var formData = new FormData($('#bulkUploadForm')[0]);

                $.ajax({
                    url: '/DailyTimeRecords/BulkUploadDTR',
                    type: 'POST',
                    data: formData,
                    success: function (data) {
                        $scope.$apply(function () {
                            vm.validationErrors = {};
                            vm.unprocessedItems = data.unprocessedItems;

                            if (!vm.unprocessedItems.length) {
                                var successMessage = `Successfully processed ${data.processedItemsCount} employee record/s.`
                                alert(successMessage);

                                $uibModalInstance.close();
                            }
                        });
                    },
                    error: function (data) {
                        $scope.$apply(function () {
                            vm.validationErrors = {};

                            if (data.status === 400) {
                                vm.validationErrors = data.responseJSON;
                            }
                        });
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
            });

            populatePayrollPeriodsSelection();
        });

        function cancel() {
            $uibModalInstance.dismiss('cancel');
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

                vm.payrollProcessBatchPayrollPeriodBasis = vm.payrollPeriods[0];
            });
        };
    };
}());
(function () {
    angular
        .module('app')
        .controller('BulkUploadEDRModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', BulkUploadEDRModalCtrl]);

    function BulkUploadEDRModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
        var vm = this;
        vm.cancel = cancel;
        vm.client = params.client;
        vm.lookups = lookups;
        vm.payrollPeriodMonth = params.payrollPeriodMonth;
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

                $scope.$apply(function () {
                    vm.uploadInProgress = true;
                });

                var formData = new FormData($('#bulkUploadForm')[0]);

                $.ajax({
                    url: '/DailyTimeRecords/BulkUploadEDR',
                    type: 'POST',
                    data: formData,
                    success: function (data) {
                        $scope.$apply(function () {
                            vm.validationErrors = {};

                            if (data.isInvalidFile) {
                                var message = 'Invalid file. To check if your file is valid, right click on your file and choose Open With and then Notepad. You should not see any strange characters.';
                                alert(message);

                                $uibModalInstance.close();
                                vm.uploadInProgress = false;
                                return;
                            }

                            if (data.duplicateEmployeeCodes.length) {
                                var message = `Upload failed. There are duplicate employee codes in the csv file: `;
                                for (var i = 0; i < data.duplicateEmployeeCodes.length; i++) {
                                    message += data.duplicateEmployeeCodes[i];
                                    if (i < data.duplicateEmployeeCodes.length - 1) message += ', ';
                                }
                                alert(message);

                                $uibModalInstance.close();
                                vm.uploadInProgress = false;
                                return;
                            }

                            vm.unprocessedItems = data.unprocessedItems;

                            if (!vm.unprocessedItems.length) {
                                var successMessage = `Successfully processed ${data.processedItemsCount} employee record/s.`

                                if (data.skippedItems && data.skippedItems.length) {
                                    var skipMessage = ' Skipping employees with no employee code: ';
                                    for (var i = 0; i < data.skippedItems.length; i++) {
                                        skipMessage += data.skippedItems[i].lastName + ', ' + data.skippedItems[i].firstName;

                                        if (i < data.skippedItems.length - 1) {
                                            skipMessage += '; ';
                                        }
                                    }

                                    successMessage += ' ' + skipMessage;
                                }

                                alert(successMessage);

                                $uibModalInstance.close();
                            }
                            else {
                                vm.uploadInProgress = false;
                            }
                        });
                    },
                    error: function (data) {
                        $scope.$apply(function () {
                            vm.validationErrors = {};

                            if (data.status === 400) {
                                vm.validationErrors = data.responseJSON;
                            }
                            else {
                                alert('An error has occured. Please try again later. If the problem persists, please contact the system administrator.');
                            }

                            vm.uploadInProgress = false;
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
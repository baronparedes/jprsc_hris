﻿(function () {
    angular
        .module('app')
        .controller('BulkUploadModalCtrl', ['$http', '$scope', '$timeout', '$uibModalInstance', 'lookups', 'params', BulkUploadModalCtrl]);

    function BulkUploadModalCtrl($http, $scope, $timeout, $uibModalInstance, lookups, params, $window) {
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
                    url: '/DailyTimeRecords/BulkUpload',
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
        });

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    };
}());
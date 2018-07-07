(function () {
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
            $("form#bulkUploadForm").submit(function (e) {
                e.preventDefault();
                var formData = new FormData($('#bulkUploadForm')[0]);

                $.ajax({
                    url: 'DailyTimeRecords/BulkUpload',
                    type: 'POST',
                    data: formData,
                    success: function (data) {
                        $scope.$apply(function () {
                            console.log('success');
                            console.log(data);

                            vm.validationErrors = {};
                        });
                    },
                    error: function (data) {
                        $scope.$apply(function () {
                            vm.validationErrors = data.responseJSON;

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
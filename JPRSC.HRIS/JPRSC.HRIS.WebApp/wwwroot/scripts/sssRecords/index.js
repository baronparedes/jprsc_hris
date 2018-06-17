(function () {
    angular
        .module('app')
        .controller('SSSRecordIndexCtrl', ['$http', '$scope', SSSRecordIndexCtrl]);

    function SSSRecordIndexCtrl($http, $scope) {
        var vm = this;
        vm.cancelEditSSSRecordClicked = cancelEditSSSRecordClicked;
        vm.editSSSRecordClicked = editSSSRecordClicked;
        vm.saveEditSSSRecordClicked = saveEditSSSRecordClicked;
        vm.sssRecords = [];
        vm.searchClicked = searchClicked;
        vm.searchModel = {};
        vm.searchInProgress = false;

        $scope.$watch('vm.searchModel', onSearchModelChange, true);

        function cancelEditSSSRecordClicked(sssRecord) {
            sssRecord.isEditing = false;
            sssRecord.copy = {};
        };

        function editSSSRecordClicked(sssRecord) {
            sssRecord.isEditing = true;
            sssRecord.copy = angular.copy(sssRecord);
        };

        function onSearchModelChange(newValue, oldValue) {
            searchClicked();
        };

        function saveEditSSSRecordClicked(sssRecord) {
            var action = '/SSSRecords/Edit';
            var data = sssRecord.copy;

            $http.post(action, data).then(function (response) {
                sssRecord.isEditing = false;
                sssRecord.number = sssRecord.copy.number;
                sssRecord.range1 = sssRecord.copy.range1;
                sssRecord.employer = sssRecord.copy.employer;
                sssRecord.employee = sssRecord.copy.employee;
                sssRecord.philHealthEmployer = sssRecord.copy.philHealthEmployer;
                sssRecord.philHealthEmployee = sssRecord.copy.philHealthEmployee;
                sssRecord.ecc = sssRecord.copy.ecc;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function searchClicked() {
            vm.searchInProgress = true;

            $http.get('/SSSRecords/Search', { params: vm.searchModel }).then(function (response) {
                vm.sssRecords = response.data.sssRecords;
                for (var i = 0; i < vm.sssRecords.length; i++) {
                    Object.defineProperty(vm.sssRecords[i], 'total', { get: function () { return this.employee + this.employer; }})
                }
                vm.searchInProgress = false;
            });
        };
    };
}());
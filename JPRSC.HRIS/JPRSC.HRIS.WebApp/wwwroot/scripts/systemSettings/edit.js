(function () {
    angular
        .module('app')
        .controller('EditSystemSettingsCtrl', ['$http', '$timeout', '$window', EditSystemSettingsCtrl]);

    function EditSystemSettingsCtrl($http, $timeout, $window) {
        var vm = this;
        vm.editSMTPSettingsSubmit = editSMTPSettingsSubmit;
        vm.editSystemSettingsSubmit = editSystemSettingsSubmit;
        vm.isTesting = false;
        vm.phicRecord = {};
        vm.testClicked = testClicked;
        vm.validationErrors = {};

        $timeout(function () {
            vm.phicRecord = vm.serverModel;
        });

        function editSystemSettingsSubmit(e) {
            var action = '/SystemSettings/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                alert('System Settings saved!');
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function editSMTPSettingsSubmit(e) {
            var action = '/SystemSettings/EditSMTP';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                if (vm.isTesting == true) {
                    alert('Test email sent!');
                }
                else {
                    alert('SMTP Settings saved!');
                }

                vm.isTesting = false;
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };

        function testClicked() {
            vm.isTesting = true;
        };
    };
}());
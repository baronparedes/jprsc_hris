(function () {
    angular
        .module('app')
        .controller('EditSystemSettingsCtrl', ['$http', '$timeout', '$window', EditSystemSettingsCtrl]);

    function EditSystemSettingsCtrl($http, $timeout, $window) {
        var vm = this;
        vm.editSystemSettingsSubmit = editSystemSettingsSubmit;
        vm.phicRecord = {};
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
    };
}());
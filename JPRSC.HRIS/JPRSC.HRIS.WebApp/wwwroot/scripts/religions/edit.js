(function () {
    angular
        .module('app')
        .controller('EditReligionCtrl', ['$http', '$window', EditReligionCtrl]);

    function EditReligionCtrl($http, $window) {
        var vm = this;
        vm.editReligionSubmit = editReligionSubmit;
        vm.validationErrors = {};

        function editReligionSubmit(e) {
            var action = '/Religions/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/Religions/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
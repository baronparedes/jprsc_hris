(function () {
    angular
        .module('app')
        .controller('EditJobTitleCtrl', ['$http', '$window', EditJobTitleCtrl]);

    function EditJobTitleCtrl($http, $window) {
        var vm = this;
        vm.editJobTitleSubmit = editJobTitleSubmit;
        vm.validationErrors = {};

        function editJobTitleSubmit(e) {
            var action = '/JobTitles/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/JobTitles/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
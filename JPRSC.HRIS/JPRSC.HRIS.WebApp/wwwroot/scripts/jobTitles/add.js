(function () {
    angular
        .module('app')
        .controller('AddJobTitleCtrl', ['$http', '$window', AddJobTitleCtrl]);

    function AddJobTitleCtrl($http, $window) {
        var vm = this;
        vm.addJobTitleSubmit = addJobTitleSubmit;
        vm.validationErrors = {};

        function addJobTitleSubmit(e) {
            var action = '/JobTitles/Add';
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
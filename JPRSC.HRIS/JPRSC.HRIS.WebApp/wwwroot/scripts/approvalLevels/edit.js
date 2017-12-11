(function () {
    angular
        .module('app')
        .controller('EditApprovalLevelCtrl', ['$http', '$window', EditApprovalLevelCtrl]);

    function EditApprovalLevelCtrl($http, $window) {
        var vm = this;
        vm.editApprovalLevelSubmit = editApprovalLevelSubmit;
        vm.validationErrors = {};

        function editApprovalLevelSubmit(e) {
            var action = '/ApprovalLevels/Edit';
            var data = $(angular.element(e.target)[0]).serialize();
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

            $http.post(action, data, config).then(function (response) {
                $window.location = '/ApprovalLevels/Index';
            }, function (response) {
                if (response.status == 400) {
                    vm.validationErrors = response.data;
                }
            });
        };
    };
}());
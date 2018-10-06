//(function () {
//    angular
//        .module('app')
//        .controller('EditEmployeeRateCtrl', ['$http', '$window', EditEmployeeRateCtrl]);

//    function EditEmployeeRateCtrl($http, $window) {
//        var vm = this;
//        vm.editEmployeeRateSubmit = editEmployeeRateSubmit;
//        vm.validationErrors = {};

//        function editEmployeeRateSubmit(e) {
//            var action = '/EmployeeRates/Edit';
//            var data = $(angular.element(e.target)[0]).serialize();
//            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

//            $http.post(action, data, config).then(function (response) {
//                $window.location = '/EmployeeRates/Index';
//            }, function (response) {
//                if (response.status == 400) {
//                    vm.validationErrors = response.data;
//                }
//            });
//        };
//    };
//}());
//(function () {
//    angular
//        .module('app')
//        .controller('AddEmployeeRateCtrl', ['$http', '$window', AddEmployeeRateCtrl]);

//    function AddEmployeeRateCtrl($http, $window) {
//        var vm = this;
//        vm.addEmployeeRateSubmit = addEmployeeRateSubmit;
//        vm.validationErrors = {};

//        function addEmployeeRateSubmit(e) {
//            var action = '/EmployeeRates/Add';
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
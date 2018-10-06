//(function () {
//    angular
//        .module('app')
//        .controller('AddPhicRecordCtrl', ['$http', '$window', AddPhicRecordCtrl]);

//    function AddPhicRecordCtrl($http, $window) {
//        var vm = this;
//        vm.addPhicRecordSubmit = addPhicRecordSubmit;
//        vm.validationErrors = {};

//        function addPhicRecordSubmit(e) {
//            var action = '/PhicRecords/Add';
//            var data = $(angular.element(e.target)[0]).serialize();
//            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' } };

//            $http.post(action, data, config).then(function (response) {
//                $window.location = '/PhicRecords/Index';
//            }, function (response) {
//                if (response.status == 400) {
//                    vm.validationErrors = response.data;
//                }
//            });
//        };
//    };
//}());
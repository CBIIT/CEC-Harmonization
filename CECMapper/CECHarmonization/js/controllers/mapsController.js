//// The controller uses the Factory to make calls to the ASP.NET Web API service.    This controller relies on the 
////  dataFactory for data retrieval and manipulation. All of the dataFactory functions return a promise which 
////  is resolved by the controller using the success() and error() functions. Once data is returned from the 
////  factory (assuming success) the $scope is updated which will drive the user interface.


////angular.module('CECApp')

//  app.controller('mapsController', ['$scope', 'dataFactory', 
//        function ($scope, dataFactory) {

//    $scope.status = "starting...";

//    $scope.customers;
//    $scope.targetDatasets;
//    $scope.orders;

//    getTargetDatasets();

//    function getTargetDatasets() {
//        dataFactory.getDatasets('harmonization')
//            .success(function (result) {
//                $scope.targetDatasets = result;
//            })
//            .error(function (error) {
//                $scope.status = 'Unable to load Target Datasets data: ' + error.message;
//            });
//    }


    

////    $scope.updateCustomer = function (id) {
////        var cust;
////        for (var i = 0; i < $scope.customers.length; i++) {
////            var currCust = $scope.customers[i];
////            if (currCust.ID === id) {
////                cust = currCust;
////                break;
////}
////}

////        dataFactory.updateCustomer(cust)
////          .success(function () {
////              $scope.status = 'Updated Customer! Refreshing customer list.';
////})
////          .error(function (error) {
////              $scope.status = 'Unable to update customer: ' + error.message;
////});
////};

////    $scope.insertCustomer = function () {
////    //Fake customer data
////        var cust = {
////    ID: 10,
////    FirstName: 'JoJo',
////    LastName: 'Pikidily'
////};
////        dataFactory.insertCustomer(cust)
////            .success(function () {
////                $scope.status = 'Inserted Customer! Refreshing customer list.';
////                $scope.customers.push(cust);
////}).
////            error(function(error) {
////                $scope.status = 'Unable to insert customer: ' + error.message;
////});
////};

////    $scope.deleteCustomer = function (id) {
////        dataFactory.deleteCustomer(id)
////        .success(function () {
////            $scope.status = 'Deleted Customer! Refreshing customer list.';
////            for (var i = 0; i < $scope.customers.length; i++) {
////                var cust = $scope.customers[i];
////                if (cust.ID === id) {
////                    $scope.customers.splice(i, 1);
////                    break;
////}
////}
////            $scope.orders = null;
////})
////        .error(function (error) {
////            $scope.status = 'Unable to delete customer: ' + error.message;
////});
////};

////    $scope.getCustomerOrders = function (id) {
////        dataFactory.getOrders(id)
////        .success(function (orders) {
////            $scope.status = 'Retrieved orders!';
////            $scope.orders = orders;
////})
////        .error(function (error) {
////            $scope.status = 'Error retrieving customers! ' + error.message;
////});
//            //};


//}]);
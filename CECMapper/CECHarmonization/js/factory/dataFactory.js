///  This factory handles GET, PUT, POST, and DELETE calls to the ASP.NET Web API service. 
//   The factory creates an object that handles making calls to the server.

// The $http object is injected into the factory at runtime by AngularJS and used to make Ajax calls to the 
//  server. Looking through the code you can see that it exposes get(), post(), put(), and delete() functions 
//  that make it a piece of cake to work with RESTful services. Because the functions defined in the factory 
//  don’t know what to do with the data they handle, each one returns a promise that can be wired up to 
//  callback functions by the caller.

//angular.module('CECApp')
module.factory('dataFactory', ['$http', function ($http) {

    var urlBase = '/api';
    var dataFactory = {};


    //////////////////////////////////////////////////////////

    dataFactory.getMaps = function () {
        return $http.get(urlBase + '/MapsAPI');
    };

    dataFactory.getMapperByVariables = function (targetId, svaId, studyId) {
        mapper = $http.get(urlBase + '/MapsAPI?targetId=' + targetId + '&svaId=' + svaId + '&studyId=' + studyId);
        return mapper;
    };

    
    dataFactory.micaSave = function (m) {
        var data = JSON.stringify(m);
        var request = $http({
            method: 'post',
            url: '/api/MicaAPI?svaId=' + m.entity_id,
            data: data,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return request;
    };


    // save (should always be an update)
    dataFactory.mapperSave = function (m) {
        var data = JSON.stringify(m);
        var request = $http({
            method: 'put',
            url: '/api/MapsAPI?id=' + m.Id,
            data: data,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return request;
    };



    //////////////////////////////////////////////////////////
    // step 1
    dataFactory.getDatasets = function (type) {
        return $http.get(urlBase + '/datasetAPI?type=' + type);
    };
    // step 2
    dataFactory.getVariablesByDatasetId = function (id) {
        return $http.get(urlBase + '/VariableAPI?id=' + id);
    };
    // step 3
    dataFactory.getStudiesByTargetId = function (targetId) {
        return $http.get(urlBase + '/StudyAPI?id=' + targetId);
    };
    // step 4
    dataFactory.getDatasetsByStudyId = function (studyId, cohortId) {
        return $http.get(urlBase + '/datasetAPI?action=1&id=' + studyId);
    };

    //////////////////////////////////////////////////////////
    dataFactory.getVariableValuesByVariableId = function (id, studyid) {
        return $http.get(urlBase + '/VariableAPI?action=1&id=' + id + '&studyid=' + studyid);
    };

    dataFactory.getVariableValuesByDatasetId = function (id, studyid) {
        return $http.get(urlBase + '/VariableAPI?action=2&id=' + id + '&studyid=' + studyid);
    }



    ///////////////////////////////////////////////////////////////
    /// mica items
    ///////////////////////////////////////////////////////////////
    //dataFactory.getMicaData = function (svaid) {

    //    str = urlBase + '/MicaApi?id=' + svaid;

    //    $http({ method: 'GET', url: str }).
    //   success(function (data, status) {
    //       return data;
    //   }).
    //   error(function (data, status) {
    //       return null;
    //   });
    //};

    dataFactory.getMicaData = function (svaId) {
        return $http.get(urlBase + '/MicaApi?id=' + svaId);
    };






    dataFactory.updateCustomer = function (cust) {
        return $http.put(urlBase + '/' + cust.ID, cust)
    };

    dataFactory.deleteCustomer = function (id) {
        return $http.delete(urlBase + '/' + id);
    };

    dataFactory.getOrders = function (id) {
        return $http.get(urlBase + '/' + id + '/orders');
    };

    return dataFactory;
}]);


var module = angular.module('CECApp', ['ngRoute', 'ui.bootstrap', 'ngSanitize', 'queryBuilder', 'CECFilters']);

module.controller('mapsController', mapsController);

function mapsController($rootScope, $scope, $window, $log, dataFactory, mapperService, cvvService, micaService) {


    /// dropdowns at the top --------------------------------------
    $scope.targetDatasets;
    $scope.targetVariables;

    $scope.cohortStudies;
    $scope.cohortStudiesforhARM;
    $scope.cohortDatasets;
    $scope.cohortVariables;
    $scope.maprecs = [];
    $scope.Action = [];




    $scope.Status = [
        { name: 'impossible' },
        { name: 'complete' },
        { name: 'undetermined' }
    ];

    // called after selection in #4
    $scope.setSVAStatus = function (action) {
        $scope.SVAStatus = action.name;
    };


    $scope.tabs = [
    //{ title: 'Dynamic Title 1', content: 'Dynamic content 1' },
    //{ title: 'Dynamic Title 2', content: 'Dynamic content 2', disabled: true }
    ];

    // get source for 'Select Harmonized Dataset' drop down (#1)
    getTargetDatasets();

    // get cohort studies for option 2 (always Harmonized Dataset)
    getCohortStudiesforHarm();


    // called after selection in #1
    $scope.setTargetDataset = function (action) {
        $scope.selectedTargetDataset = action;
        //getTargetVariables($scope.selectedTargetDataset.nid);
        getCohortStudies($scope.selectedTargetDataset.nid)
    };

    // called after selection in #2
    // #2 removed - 1/16/2018
    //$scope.setTargetVariable = function (action) {
    //    $scope.selectedTargetVariable = action;
    //    getCohortStudies($scope.selectedTargetVariable.nid)
    //};


    // called after selection in #3
    $scope.setCohortStudy = function (action) {
        $scope.selectedCohortStudy = action;
        getCohortDatasets($scope.selectedCohortStudy.study_id);
    };



    // called after selection in #4
    $scope.setCohortDataset = function (action) {
        $scope.selectedCohortDataset = action;
    };




    ///////////////////////////////////////////////////////////////////////////////////
    // various sets
    $scope.setTargetVariableValue = function (action) {
        $scope.selectedTargetVariableValue = action;
    };

    $scope.setCohortVariableValue = function (action) {
        $scope.selectedCohortVariableValue = action;
    };

    $scope.setVariableValueAction = function (action, i) {
        $scope.Action[i] = action;


    };

    //$scope.setCohortVariable = function (action) {
    //    $scope.selectedCohortVariable = action;
    //};

    //$scope.status = {
    //    isopen: false
    //};

    //$scope.toggled = function (open) {
    //    $log.log('Dropdown is now: ', open);
    //};

    //$scope.toggleDropdown = function ($event) {
    //    $event.preventDefault();
    //    $event.stopPropagation();
    //    $scope.status.isopen = !$scope.status.isopen;
    //};

    ///////////////////////////////////////////////////////////////////////////////////
    // step 1
    // get Target Datasets (all Harmonized datasets)
    function getTargetDatasets() {
        dataFactory.getDatasets('harmonization')
        .success(function (result) {
            $scope.targetDatasets = result;
        })
        .error(function (error) {
            $scope.status = 'Unable to load Target Datasets data: ' + error.message;
        });
    };


    ///////////////////////////////////////////////////////////////////////////////////
    // step 2
    // get target variables contained within the selected Harmonized Dataset 
    function getTargetVariables(id) {
        dataFactory.getVariablesByDatasetId(id)
    .success(function (result) {
        $scope.targetVariables = result;
    })
            .error(function (error) {
                $scope.status = 'Unable to load Target Variables data: ' + error.message;
            });
    };

    ///////////////////////////////////////////////////////////////////////////////////
    // step 3
    // get All Cohort Studies that are associated with teh Selected Target
    function getCohortStudies(id) {
        dataFactory.getStudiesByTargetId(id)
            .success(function (result) {
                $scope.cohortStudies = result;
            })
            .error(function (error) {
                $scope.status = 'Unable to load Cohort Studies data: ' + error.message;
            });
    };


    ///////////////////////////////////////////////////////////////////////////////////
    // pre-step for Option #2
    // get All Cohort Studies that are associated with the Harmonized Dataset
    function getCohortStudiesforHarm() {
        dataFactory.getStudiesByTargetId(673)
            .success(function (result) {
                $scope.cohortStudiesforHARM = result;
            })
            .error(function (error) {
                $scope.status = 'Unable to load Cohort Studies data: ' + error.message;
            });
    };

    ///////////////////////////////////////////////////////////////////////////////////
    // step 4
    // get Cohort datasets within the selected Cohort Study
    function getCohortDatasets(id) {
        dataFactory.getDatasetsByStudyId(id)
            .success(function (result) {
                $scope.cohortDatasets = result;
            })
                    .error(function (error) {
                        $scope.status = 'Unable to load Cohort Datasets data: ' + error.message;
                    });
    };




    // get the list of Values for the given Target Varialbe
    function getVariableValues(id, studyid) {
        dataFactory.getVariableValuesByVariableId(id, studyid)
        .success(function (result) {
            $scope.targetVariableValues = result;
        })
        .error(function (error) {
            $scope.status = 'Unable to load Variable Values data: ' + error.message;
        });
    };

    // used when inserting a new map record
    function getCohortVariables(id) {
        dataFactory.getVariablesByDatasetId(id)
        .success(function (result) {
            $scope.cohortVariables = result;
        })
            .error(function (error) {
                $scope.status = 'Unable to load Cohort Variables data: ' + error.message;
            });
    };

    // used when inserting a new map record
    function getCohortVariableValues(datasetid, studyid) {
        dataFactory.getVariableValuesByDatasetId(datasetid, studyid)
            .success(function (result) {
                $scope.cohortVariableValues = result;
                cvvService.addcvv(result);
            })
            .error(function (error) {
                $scope.status = 'Unable to load Cohort Variable Values data: ' + error.message;
            });
    };


    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////

    function getMapperByVariables(targetId, svaId, studyId) {
        dataFactory.getMapperByVariables(targetId, svaId, studyId)
                .success(function (result) {
                    for (i = 0; i < result.MapRecs.length; i++) {
                        if (result.MapRecs[i].json == null) {
                            result.MapRecs[i].filter = JSON.parse('{"group": {"operator": "AND","i": ' + i + ', "rules": []}}');
                        }
                        else
                            result.MapRecs[i].filter = JSON.parse(result.MapRecs[i].json);
                    }
                    $scope.Mapper = result;
                    mapperService.addmapper(result);
                })
                        .error(function (error) {
                            $scope.status = 'Unable to load Mapper data: ' + error.message;
                        });
    };


    //function insertMapper(mapper) {
    //    dataFactory.insertMapper(mapper)
    //        .success(function (result) {
    //            $scope.Mapper = result;
    //        })
    //        .error(function (error) {
    //            $scope.status = 'Unable to save Mapper data: ' + error.message;
    //        });
    //};


    $scope.insertMapRecs = function (variable) {
        $scope.maprecs.push({
            "MapType": "if-then",
            "TargetVariable": $scope.selectedTargetVariableValue.title,
            "TargetLabel": $scope.selectedTargetVariableValue.field_variable_categories_label,
            "TargetValue": $scope.selectedTargetVariableValue.field_variable_categories_name,
            "CohortDataset": $scope.selectedCohortVariableValue.dataset_name,
            "CohortVariable": $scope.selectedCohortVariableValue.title,
            "CohortLabel": $scope.selectedCohortVariableValue.field_variable_categories_label,
            "CohortValue": $scope.selectedCohortVariableValue.field_variable_categories_name

        });

    }

    ///////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////
    ///  called when tabs are selected -----------------------------------------------
    $scope.getContent = function (tabInfo) {

        // target values
        //getVariableValues(tabInfo.entity_id, tabinfo.study_id);

        //getVariableValues($scope.selectedTargetVariable.nid, $scope.selectedTargetVariable.study_id)

        // get cohort values and save in cohortVariableValues factory
        getCohortVariableValues(tabInfo.entity_id, tabInfo.study_id);

        // 
        $scope.cvv = cvvService.getcvv();

        getMicaData($scope.selectedCohortStudy.sva_id)

        getMapperByVariables($scope.selectedTargetVariable.nid, $scope.selectedCohortStudy.sva_id, tabInfo.study_id);


        //$scope.Mapper = new Mapper();
        //$scope.Mapper.load($scope.selectedTargetVariable.nid, $scope.selectedCohortStudy.sva_id, tabInfo.study_id);

        //   $scope.Mapper = mapService.load($scope.selectedTargetVariable.nid, $scope.selectedCohortStudy.sva_id, tabInfo.study_id);



        return
    };


    // get the list of Values for the given Target Varialbe
    function getMicaData(id) {
        dataFactory.getMicaData(id)
        .success(function (result) {
            micaService.addmica(result);
            $scope.micadata = result;
        })
        .error(function (error) {
            $scope.status = 'Unable to load mica data: ' + error.message;
        });
    };




    // called when a new tab is created
    $scope.insertTab = function (variable) {
        $scope.tabs.push({
            "title": $scope.selectedCohortDataset.title,
            "content": $scope.selectedCohortDataset,
            active: true
        });

    }


 

    $scope.deleteTab = function (i) {
        $scope.tabs.splice(i, 1);
    }



    //// save the Mica data (at the bottom of the page)
    //$scope.micaSave = function (svaId, comment, MapRecs, status) {

    //    micaScript = "";
    //    for (i = 0; i <= MapRecs.length - 1; i++) {
    //        if (!angular.isUndefined(MapRecs[i].scriptSection))
    //            if (MapRecs[i].scriptSection != null)
    //                if (MapRecs[i].TargetMissing == "1")
    //                    micaScript = MapRecs[i].scriptSection + ";" + micaScript;
    //                else
    //                    micaScript += MapRecs[i].scriptSection + "; ";
    //    }

    //    m = micaService.getmica(0);
    //    m[0].Id = 1;
    //    m[0].entity_id = svaId;
    //    m[0].field_sva_comment_value = comment;
    //    m[0].field_sva_script_value = micaScript;
    //    m[0].field_sva_status_value = status;

    //    //micaService.addmica(m);

    //    dataFactory.micaSave(m[0])
    //          .success(function (result) {
    //              $scope.status = 'Successfully saved Mica Data' + result;
    //          })
    //          .error(function (error) {
    //              $scope.status = 'Unable to save Mica data: ' + error.message;
    //          });


    //}

    //// save the Mapper data (does not go to Mica db)
    //$scope.mapperSave = function (m) {


    //    m.CohortDatasetId = $scope.selectedCohortDataset.entity_id;
    //    m.CohortDatasetName = $scope.selectedCohortDataset.title;
    //    m.TargetDatasetId = $scope.selectedTargetDataset.nid;
    //    m.TargetDatasetName = $scope.selectedTargetDataset.title;
    //    m.TargetFieldId = $scope.selectedTargetVariable.nid;
    //    m.TargetFieldName = $scope.selectedTargetVariable.title;

    //    // update the service
    //    mapperService.addmapper(m);
    //    dataFactory.mapperSave(m)
    //          .success(function (result) {
    //              $scope.status = 'Successfully saved Mapper Data' + result;
    //          })
    //          .error(function (error) {
    //              $scope.status = 'Unable to save Mapper data: ' + error.message;
    //          });


    //}


    $scope.alertMe = function () {
        setTimeout(function () {
            $window.alert('You\'ve selected the alert tab!');
        });
    };

    $scope.allSave = function (svaId, comment, MapRecs, status, mapper) {
        
        // save Mica Data
        /////////////////////////////////////////////////////////////
        micaScript = "";
        for (i = 0; i <= MapRecs.length - 1; i++) {
            if (!angular.isUndefined(MapRecs[i].scriptSection))
                if (MapRecs[i].scriptSection != null)
                    if (MapRecs[i].TargetMissing == "1")
                        micaScript = MapRecs[i].scriptSection + ";" + micaScript;
                    else
                        micaScript += MapRecs[i].scriptSection + "; ";
        }

        m = micaService.getmica(0);
        m[0].Id = 1;
        m[0].entity_id = svaId;
        m[0].field_sva_comment_value = comment;
        m[0].field_sva_script_value = micaScript;
        m[0].field_sva_status_value = status;

        //micaService.addmica(m);

        dataFactory.micaSave(m[0])
              .success(function (result) {
                  $scope.status = 'Successfully saved Mica Data' + result;
              })
              .error(function (error) {
                  $scope.status = 'Unable to save Mica data: ' + error.message;
              });

        // save Mapper Data
        /////////////////////////////////////////////////////////////
        mapper.CohortDatasetId = $scope.selectedCohortDataset.entity_id;
        mapper.CohortDatasetName = $scope.selectedCohortDataset.title;
        mapper.TargetDatasetId = $scope.selectedTargetDataset.nid;
        mapper.TargetDatasetName = $scope.selectedTargetDataset.title;
        mapper.TargetFieldId = $scope.selectedTargetVariable.nid;
        mapper.TargetFieldName = $scope.selectedTargetVariable.title;

        // update the service
        mapperService.addmapper(mapper);
        dataFactory.mapperSave(mapper)
              .success(function (result) {
                  $scope.status = 'Successfully saved Mapper Data' + result;
              })
              .error(function (error) {
                  $scope.status = 'Unable to save Mapper data: ' + error.message;
              });

    }


}


//var app = angular.module('app', ['ngSanitize', 'queryBuilder']);
module.controller('QueryBuilderController', ['$rootScope', '$scope', 'mapperService', function ($rootScope, $scope, mapperService) {
    var data = '{"group": {"operator": "AND","rules": []}}';


    $scope.actions = [
     { type: 'ASSIGN', msg: 'just set the target to the selected cohort' },
     { type: 'SET', msg: 'set the target to a value (no cohort variable)' },
     { type: 'IF', msg: 'if cohort is x then set target' },
     { type: 'ELSE-IF', msg: 'else if cohort is x then set target' },
     { type: 'ELSE', msg: 'if cohort condition not included above then set target to an expression' },
     { type: 'ELSE-SET', msg: 'if cohort condition not included above then set target directly' },
     { type: 'ELSE-ASSIGN', msg: 'if cohort condition not included above then ASSIGN target directly' },
     { type: 'ELSE-CONVERT', msg: 'if cohort condition not included above then CONVERT to the target directly' },
     { type: 'CONVERT', msg: 'if cohort is x then set target' }
    ];

    function htmlEntities(str) {
        return String(str).replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    function computed(group) {
        if (!group) return "";

        if (group.selectedAction != null) {
            if (group.selectedAction == "ASSIGN" || group.selectedAction == "ELSE-ASSIGN") {
                group.disableFields = true;
                group.disableConditions = false;
                group.disableConditionData = false;
                group.disableTextInput = false;
                group.disableOperations = false;
            }
            if (group.selectedAction == "SET" || group.selectedAction == "ELSE-SET") {
                group.disableFields = false;
                group.disableConditions = false;
                group.disableConditionData = false;
                group.disableTextInput = false;
                group.disableOperations = false;
            }
            else if (group.selectedAction == "CONVERT" || group.selectedAction == "ELSE-CONVERT") {
                group.disableFields = true;
                group.disableConditions = false;
                group.disableConditionData = false;
                group.disableTextInput = true;
                group.disableOperations = true;
            }
            else if (group.selectedAction == "IF" || group.selectedAction == "ELSE-IF" || group.selectedAction == "ELSE") {
                group.disableFields = true;
                group.disableConditions = true;
                group.disableConditionData = true;
                group.disableTextInput = true;
                group.disableOperations = false;

            }
        }


        fullstr = null;

        // "SET"  or "ELSE-SET"
        if (group.selectedAction == "SET" || group.selectedAction == "ELSE-SET")
            if (group.disableConditions == false & group.disableConditionData == false & group.disableTextInput == false & group.disableOperations == false) {
                for (var str = "(", i = 0; i < group.rules.length; i++) {
                    i > 0 && (str += " " + group.operator + " ");
                    str += group.rules[i].group ?
                        computed(group.rules[i].group) :
                         " = " + group.rules[i].field;
                }
            }

        // "ASSIGN"
        if (group.selectedAction == "ASSIGN" || group.selectedAction == "ELSE-ASSIGN")
            if (group.disableConditions == false & group.disableConditionData == false & group.disableTextInput == false & group.disableOperations == false) {
                for (var str = "", i = 0; i < group.rules.length; i++) {
                    i > 0 && (str += " " + group.operator + " ");
                    str += group.rules[i].group ?
                        computed(group.rules[i].group) :
                         " = " + group.rules[i].field;
                }

            }

        // "CONVERT"
        if (group.selectedAction == "CONVERT" || group.selectedAction == "ELSE-CONVERT") {
            for (var str = "(", i = 0; i < group.rules.length; i++) {
                i > 0 && (str += "");
                str += group.rules[i].group ?
                    computed(group.rules[i].group) :
                    group.rules[i].field + " " + group.rules[i].operation + " " + group.rules[i].data;
                //group.rules[i].field + " " + htmlEntities(group.rules[i].condition) + " " + group.rules[i].data;
            }
        }

        // "IF"  "ELSE-IF" or "ELSE"
        if (group.selectedAction == "IF" || group.selectedAction == "ELSE-IF" || group.selectedAction == "ELSE") {
            for (var str = "(", i = 0; i < group.rules.length; i++) {
                i > 0 && (str += " " + group.operator + " ");
                str += group.rules[i].group ?
                    computed(group.rules[i].group) :
                    group.rules[i].field + " " + htmlEntities(group.rules[i].condition) + " " + group.rules[i].data;
            }

        }


        return str + ")";
        //return str;
    }

    $scope.json = null;

    $scope.filter = JSON.parse(data);

    $scope.$watch('filter', function (newValue) {
        $scope.json = JSON.stringify(newValue, null, 2);

        var str = computed(newValue.group);
        if (newValue.group.selectedAction == "IF" || newValue.group.selectedAction == "ELSE-IF" || newValue.group.selectedAction == "ELSE") {
            fullstr = String(newValue.group.selectedAction).replace("-", " ").replace("SET", "") + " " + str + " THEN " + newValue.group.TargetFieldName + " = " + newValue.group.TargetValue;

        }
        if (newValue.group.selectedAction == "CONVERT" || newValue.group.selectedAction == "ELSE-CONVERT") {
            fullstr = String(newValue.group.selectedAction).replace("-", " ").replace("CONVERT", " ") + newValue.group.TargetFieldName + " = " + str + "";
        }
        if (newValue.group.selectedAction == "ASSIGN" || newValue.group.selectedAction == "ELSE-ASSIGN") {
            if (newValue.group.rules.length > 0)
                fullstr = String(newValue.group.selectedAction).replace("-", " ").replace("ASSIGN", "") + " " + newValue.group.TargetFieldName + " = " + newValue.group.rules[0].field;
            else
                fullstr = String(newValue.group.selectedAction).replace("-", " ").replace("ASSIGN", "") + " " + newValue.group.TargetFieldName + " = ";

        }
        if (newValue.group.selectedAction == "SET" || newValue.group.selectedAction == "ELSE-SET") {
            fullstr = String(newValue.group.selectedAction).replace("-", " ").replace("SET", "") + " " + newValue.group.TargetFieldName + " = " + newValue.group.TargetValue;

        }


        $scope.output = fullstr;
        $scope.newValue = newValue;
        //if (newValue.group.rules.length > 0) {
        mapperService.updatemaprec(newValue, $scope.output, $scope.json);
        this.mapper = mapperService.getmapper();
        //}
    }, true);


    //$scope.$watch("rec.Action", function (newValue, oldValue) {
    //    if (newValue != null) {
    //        if (newValue.type == "SET" || newValue.type == "ASSIGN" || newValue.type == "ELSE-SET") {
    //            $scope.disableConditions = false;
    //            $scope.disableConditionData = false;
    //            $scope.disableTextInput = false;
    //            $scope.disableOperations = false;
    //        }
    //        else if (newValue.type == "CONVERT") {
    //            $scope.disableConditions = false;
    //            $scope.disableConditionData = false;
    //            $scope.disableTextInput = true;
    //            $scope.disableOperations = true;
    //        }
    //        else if (newValue.type == "IF" || newValue.type == "ELSE-IF" || newValue.type == "ELSE") {
    //            $scope.disableConditions = true;
    //            $scope.disableConditionData = true;
    //            $scope.disableTextInput = true;
    //            $scope.disableOperations = false;

    //        }
    //    }
    //});





}]);

var queryBuilder = angular.module('queryBuilder', []);
queryBuilder.directive('queryBuilder', ['$compile', 'dataFactory', 'mapperService', 'cvvService', function ($compile, dataFactory, mapperService, cvvService) {
    return {
        restrict: 'E',  //only can be instatiated by an Element
        scope: {
            group: '=',
            id: '='
        },
        templateUrl: '/queryBuilderDirective.html',
        controller: function ($scope) {

        },
        link: function postLink(scope, iElement, iAttrs) {
        },
        compile: function (element, attrs) {
            var content, directive;
            content = element.contents().remove();
            return {
                pre: function (scope, element, attributes, controller, transcludeFn) {

                },
                post: function (scope, element, attrs, $rootScope) {




                    scope.operators = [
                        { name: 'AND' },
                        { name: 'OR' }
                    ];


                    //scope.fields = scope.$parent.cohortVariableValues;
                    scope.fields = cvvService.getcvv(0); // (scope.cohort === undefined) ? scope.group.fields : scope.cohort;
                    //    { name: 'Firstname' },
                    //    { name: 'Lastname' },
                    //    { name: 'Birthdate' },
                    //    { name: 'City' },
                    //    { name: 'Country' }
                    //];

                    scope.conditions = [
                        { name: '' },
                        { name: '=' },
                        { name: '<>' },
                        { name: '<' },
                        { name: '<=' },
                        { name: '>' },
                        { name: '>=' }
                    ];

                    scope.operations = [
                        { name: '' },
                        { name: '*' },
                        { name: '/' },
                        { name: '+' },
                        { name: '-' },
                    ];

                    scope.addCondition = function (i) {
                        scope.group.rules.push({
                            condition: '=',
                            field: '',
                            data: '',
                            value: '',
                            operation: ''
                        });
                    };

                    scope.removeCondition = function (index) {
                        scope.group.rules.splice(index, 1);
                    };

                    scope.addGroup = function (i) {
                        scope.group.rules.push({
                            group: {
                                operator: 'AND',
                                rules: [],
                                TargetFieldId: scope.group.TargetFieldId,
                                TargetFieldName: scope.group.TargetFieldName,
                                TargetLabel: scope.group.TargetLabel,
                                TargetValue: scope.group.TargetValue,
                                disableConditionData: scope.group.disableConditionData,
                                disableConditions: scope.group.disableConditions,
                                disableOperations: scope.group.disableOperations,
                                disableTextInput: scope.group.disableTextInput,
                                selectedAction: scope.group.selectedAction,
                                i: scope.group.i
                            }
                        });
                    };


                    scope.updateConditionData = function (value, index) {
                        var a = (value) ? value : "";
                        scope.group.rules[index].data = a;
                    };



                    scope.removeGroup = function () {
                        "group" in scope.$parent && scope.$parent.group.rules.splice(scope.$parent.$index, 1);
                    };

                    directive || (directive = $compile(content));

                    element.append(directive(scope, function ($compile) {
                        return $compile;
                    }));
                }
            }
        }
    }
}]);


angular.module('CECApp').controller('AlertDemoCtrl', function ($scope) {
    $scope.alerts = [
        {
            type: 'danger', msg: 'Oh snap! Change a few things up and try submitting again.'
        },
                    {
                        type: 'success', msg: 'Well done! You successfully read this important alert message.'
                    }
    ];

    $scope.addAlert = function () {
        $scope.alerts.push({
            msg: 'Another alert!'
        });
    };

    $scope.closeAlert = function (index) {
        $scope.alerts.splice(index, 1);
    };
});


/**
 * Filters out all duplicate items from an array by checking the specified key
 * @param [key] {string} the name of the attribute of each object to compare for uniqueness
 if the key is empty, the entire object will be compared
 if the key === false then no filtering will be performed
 * @return {array}
 */
angular.module('CECFilters', []).filter('unique', ['$parse', function ($parse) {
    'use strict';

    return function (items, filterOn) {

        if (filterOn === false) {
            return items;
        }

        if ((filterOn || angular.isUndefined(filterOn)) && angular.isArray(items)) {
            var newItems = [],
              get = angular.isString(filterOn) ? $parse(filterOn) : function (item) { return item; };

            var extractValueToCompare = function (item) {
                return angular.isObject(item) ? get(item) : item;
            };

            angular.forEach(items, function (item) {
                var isDuplicate = false;

                for (var i = 0; i < newItems.length; i++) {
                    if (angular.equals(extractValueToCompare(newItems[i]), extractValueToCompare(item))) {
                        isDuplicate = true;
                        break;
                    }
                }
                if (!isDuplicate) {
                    newItems.push(item);
                }

            });
            items = newItems;
        }
        return items;
    };
}]);




// service used to share the Mapper object between controllers
module.service('mapperService', function () {
    var mapperList = [];

    var addmapper = function (newObj) {
        mapperList.push(newObj);
    };

    var getmapper = function () {
        return mapperList;
    };


    var updatemaprec = function (newValue, output, json) {
        index = newValue.group.i;
        if (angular.isUndefined(mapperList[0].MapRecs[index].filter))
            mapperList[0].MapRecs[index].filter = null;

        if (angular.isUndefined(mapperList[0].MapRecs[index].output))
            mapperList[0].MapRecs[index].output = null;

        if (angular.isUndefined(mapperList[0].MapRecs[index].json))
            mapperList[0].MapRecs[index].json = null;


        mapperList[0].MapRecs[index].filter = newValue;
        mapperList[0].MapRecs[index].scriptSection = output;
        mapperList[0].MapRecs[index].json = json;
        mapperList[0].MapRecs[index].selectedAction = newValue.group.selectedAction;
        mapperList[0].MapRecs[index].ModifiedData = new Date();
    }

    return {
        addmapper: addmapper,
        getmapper: getmapper,
        updatemaprec: updatemaprec
    };

});


// service used to share cohort variable values between controllers
module.service('cvvService', function () {
    var cvvList = [];

    var addcvv = function (newObj) {
        cvvList.push(newObj);
    };

    var getcvv = function (i) {
        return cvvList[i];
    };

    return {
        addcvv: addcvv,
        getcvv: getcvv
    };

});

// service used to share mica data between controllers
module.service('micaService', function () {
    var micaList = [];

    var addmica = function (newObj) {
        micaList.push(newObj);
    };

    var getmica = function (i) {
        return micaList[i];
    };

    return {
        addmica: addmica,
        getmica: getmica
    };

});


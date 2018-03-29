var express = require('express');
var router = express.Router();
var mysql = require('../components/mysql');
var cache = require('../components/cache');
var config = require('../config');
var logger = require('../components/logger');

router.post('/list', function(req, res) {
	let body = req.body;
	let searchText = body.searchText || "";
	let orderBy = body.orderBy || {};
	let paging = body.paging || {};
	let func = "cohort_published";
	let params = [searchText];
	if(orderBy){
		params.push(orderBy.column);
		params.push(orderBy.order);
	}
	else{
		params.push("");
		params.push("");
	}
	if(paging && paging.page != 0){
		params.push((paging.page-1) * paging.pageSize);
		params.push(paging.pageSize);
	}
	else{
		params.push(-1);
		params.push(-1);
	}
	mysql.callProcedure(func,params,function(results){
		if(results && results[0] && results[0].length > 0){
			let dt = {};
			dt.list = results[0];
			dt.total = results[1][0].total;
			res.json({status:200, data:dt});
		}
		else{
			res.json({status:200, data:{list:[],total:0}});
		}
	});
});

router.post('/select', function(req, res) {
	let body = req.body;
	let filter = body.filter || {};
	let orderBy = body.orderBy || {};
	let paging = body.paging || {};
	let func = "cohort_select";
	let params = [];
	//form filter into Strings
	let gender;
	let race;
	let ethnicity;
	
	gender = filter.participant.gender;
	//-1:[], 2:["Male"], 1:["Female"], 0: ["Male","Female"] 
	if(gender.indexOf("Female") > -1 && gender.indexOf("Male") > -1){
		params.push(0);
	}
	else if(gender.indexOf("Female") == -1 && gender.indexOf("Male") > -1){
		params.push(2);
	}
	else if(gender.indexOf("Female") > -1 && gender.indexOf("Male") == -1){
		params.push(1);
	}
	else{
		params.push(-1)
	}
	race = filter.participant.race;
	ethnicity = filter.participant.ethnicity;
	let column_info = [];
	let ethnicity_len = ethnicity.length;
	let race_len = race.length;
	let gender_len = gender.length;
	if(ethnicity_len !== 0 || race_len !== 0 || gender_len !== 0){
		if(ethnicity_len === config.ethnicity.length && race_len === config.race.length && gender_len === config.gender.length){
			column_info.push("race_total_total");
		}
		else{
			if(race_len === config.race.length || race_len === 0){
				let prefix = "race_total_";
				if(ethnicity_len === config.ethnicity.length || ethnicity_len === 0){
					ethnicity = Object.keys(config.ethnicity);
				}
				else if(gender_len === config.gender.length || gender_len === 0){
					gender = Object.keys(config.gender);
				}
				else{
					
				}
				//go through the rest of the two arrays
				ethnicity.forEach(function(eth){
					gender.forEach(function(g){
						column_info.push(prefix + config.ethnicity[eth] + "_" + config.gender[g]);
					});
				});
			}
			else{
				if((ethnicity_len === config.ethnicity.length || ethnicity_len === 0) && 
					(gender_len === config.gender.length || gender_len === 0)){
					let surfix = "_total";
					race.forEach(function(r){
						column_info.push("race_"+config.race[r]+surfix);
					});
				}
				else if((ethnicity_len === config.ethnicity.length || ethnicity_len === 0) && 
					!(gender_len === config.gender.length || gender_len === 0)){
					ethnicity = Object.keys(config.ethnicity);
					race.forEach(function(r){
						ethnicity.forEach(function(eth){
							gender.forEach(function(g){
								column_info.push("race_" + config.race[r] + "_" + config.ethnicity[eth] + "_" + config.gender[g]);
							});
						});
					});
				}
				else if(!(ethnicity_len === config.ethnicity.length || ethnicity_len === 0) && 
					(gender_len === config.gender.length || gender_len === 0)){
					gender = Object.keys(config.gender);
					race.forEach(function(r){
						ethnicity.forEach(function(eth){
							gender.forEach(function(g){
								column_info.push("race_" + config.race[r] + "_" + config.ethnicity[eth] + "_" + config.gender[g]);
							});
						});
					});
				}
				else{
					//go through all the columns and filter out the applied ones
					race.forEach(function(r){
						ethnicity.forEach(function(eth){
							gender.forEach(function(g){
								column_info.push("race_" + config.race[r] + "_" + config.ethnicity[eth] + "_" + config.gender[g]);
							});
						});
					});
				}
			}
		}
		params.push(column_info.toString());
	}
	else{
		params.push("");
	}


	if(filter.participant.age.length > 0){
		params.push(filter.participant.age.toString());
	}
	else{
		params.push("");
	}
	
	if(filter.collect.cancer.length > 0){
		let cancer_column = [];
		let category = params[0];
		filter.collect.cancer.forEach(function(cc){
			if(category == -1 || category == 2){
				cancer_column.push("ci_"+config.cancer[cc]+"_male");
				cancer_column.push("ci_"+config.cancer[cc]+"_female");
			}
			else if(category == 0){
				cancer_column.push("ci_"+config.cancer[cc]+"_male");
			}
			else{
				cancer_column.push("ci_"+config.cancer[cc]+"_female");
			}
		});
		params.push(cancer_column.toString());
	}
	else{
		params.push("");
	}
	if(filter.collect.data.length > 0){
		let data_columns = [];
		filter.collect.data.forEach(function(cd){
			data_columns.push(config.collected_data[cd]);
		});
		params.push(data_columns.toString());
	}
	else{
		params.push("");
	}
	if(filter.collect.specimen.length > 0){
		let specimen_columns = [];
		filter.collect.specimen.forEach(function(cs){
			specimen_columns.push(config.collected_specimen[cs]);
		});
		params.push(specimen_columns.toString());
	}
	else{
		params.push("");
	}

	if(filter.study.state.length > 0){
		let state_columns = [];
		filter.study.state.forEach(function(ss){
			state_columns.push(config.eligible_disease_state[ss]);
		});
		params.push(state_columns.toString());
	}
	else{
		params.push("");
	}
	if(orderBy){
		params.push(orderBy.column);
		params.push(orderBy.order);
	}
	else{
		params.push("");
		params.push("");
	}
	if(paging && paging.page != 0){
		params.push((paging.page-1) * paging.pageSize);
		params.push(paging.pageSize);
	}
	else{
		params.push(-1);
		params.push(-1);
	}
	mysql.callProcedure(func,params,function(results){
		if(results && results[0] && results[0].length > 0){
			let dt = {};
			dt.list = results[0];
			dt.total = results[1][0].total;
			res.json({status:200, data:dt});
		}
		else{
			res.json({status:200, data:{list:[],total:0}});
		}
	});
});

router.get('/:id', function(req, res){
	let id = req.params.id;
	let info = cache.getValue("cohort:"+id);
	if(info == undefined){
		let func = "cohort_info";
		let params = [id];
		mysql.callProcedure(func,params,function(results){
			if(results && results[0] && results[0].length > 0){
				let basic = results[0][0];
				info = {};
				info.cohort_id = id;
				info.cohort_name = basic.cohort_name;
				info.cohort_acronym = basic.cohort_acronym;
				info.update_time = basic.update_time;
				info.completed_by_name = basic.completed_by_name;
				info.completed_by_position = basic.completed_by_position;
				info.completed_by_phone = basic.completed_by_phone;
				info.completed_by_email = basic.completed_by_email;
				info.pi_name_1 = basic.pi_name_1;
				info.pi_name_2 = basic.pi_name_2;
				info.pi_name_3 = basic.pi_name_3;
				info.pi_name_4 = basic.pi_name_4;
				info.pi_name_5 = basic.pi_name_5;
				info.pi_name_6 = basic.pi_name_6;
				info.pi_institution_1 = basic.pi_institution_1;
				info.pi_institution_2 = basic.pi_institution_2;
				info.pi_institution_3 = basic.pi_institution_3;
				info.pi_institution_4 = basic.pi_institution_4;
				info.pi_institution_5 = basic.pi_institution_5;
				info.pi_institution_6 = basic.pi_institution_6;
				info.cohort_web_site = basic.cohort_web_site;
				info.cohort_description = basic.cohort_description;
				info.attachments = {};
				let attachs = results[1];
				attachs.forEach(function(attach){
					if(attach.category == 0){
						//cohort questionnaires
						if(info.attachments.questionnaires == undefined){
							info.attachments.questionnaires = [];
						}
						info.attachments.questionnaires.push({
							type:attach.attachment_type,
							url:attach.attachment_type == 1 ? '/download/'+attach.filename : attach.website,
							name:attach.filename
						});
					}
					else if(attach.category == 1){
						//study protocol
						if(info.attachments.protocols == undefined){
							info.attachments.protocols = [];
						}
						info.attachments.protocols.push({
							type:attach.attachment_type,
							url:attach.attachment_type == 1 ? '/download/'+attach.filename : attach.website,
							name:attach.filename
						});
					}
					else{
						//policies
						if(info.attachments.policies == undefined){
							info.attachments.policies = [];
						}
						info.attachments.policies.push({
							type:attach.attachment_type,
							url:attach.attachment_type == 1 ? '/download/'+attach.filename : attach.website,
							name:attach.filename
						});
					}
				});
				cache.setValue("cohort:"+id, info, config.cohort_ttl);
			}
			res.json({status:200, data:info});
		});
	}
	else{
		logger.debug("get from cache <cohort:"+id+">");
		res.json({
			status:200,
			data:info
		});
	}
	
});

module.exports = router;
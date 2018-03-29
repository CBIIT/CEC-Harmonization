/**
 * Main application routes
 */

'use strict';

var m_cohort = require('./service/cohort');
var m_common = require('./service/common');
var m_user = require('./service/user');
var config = require('./config');

module.exports = function(app){

	//allows CrossDomainAccess to API
	app.use(function(req, res, next){
		res.header('Access-Control-Allow-Origin', '*');
		res.header('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE');
		res.header('Access-Control-Allow-Headers', 'Content-Type, Authorization');

		if(next){
			next();
		}
	});

	app.use('/', m_common);
	app.use('/cohort', m_cohort);
	app.use('/user', m_user);
	
	// All other routes should redirect to error page
    app.route('/*')
        .get(function(req, res) {
            res.json({status:404,data:"Error request."});
    });
};
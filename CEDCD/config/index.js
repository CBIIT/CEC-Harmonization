/**
 * general configuration
 */

'use strict';

var path = require('path');
var _ = require('lodash');

var all = {
	
	// Root path of server
    root: path.normalize(__dirname + '/..'),

	// Server port
	port: process.env.PORT || 3001,

	// Server port
    logDir: process.env.LOGDIR || '/var/log/cedcd',

	// Node environment (dev, test, stage, prod), must select one.
	env: process.env.NODE_ENV || 'prod',

	//cookie max age in millseconds
	maxAge: 3600000,

	//mysql uri
	mysql: {
			connectionLimit: 100, 
			host: process.env.MYSQL_HOST || 'localhost',
			user : process.env.MYSQL_USER || 'root', 
			password : process.env.MYSQL_PASSWORD || '123456', 
			db : 'cedcd'
	},

	//time to live for cohort information
	cohort_ttl: 24 * 60 * 60,

	//file upload path
	file_path: process.env.FILE_PATH || "/data/docs/"

};

module.exports = _.merge(all, require('./' + all.env + '.js'));
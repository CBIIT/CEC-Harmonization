/**
 * production environment
 */

'use strict';
var argv = require('minimist')(process.argv.slice(2));

module.exports = {
	// Server port
	port: argv.p || 9221,

	// Server port
    logDir: argv.o || '/local/content/analysistools-sandbox/cedcd_log/',

	//file upload path
	file_path: argv.f || "/local/content/cedcd_data/"
};

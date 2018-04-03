/**
 * local environment
 */

'use strict';

module.exports = {
	// Server port
	port: process.env.PORT || 9221,

	// Server port
    logDir: process.env.LOGDIR || '/var/log/cedcd',

	//file upload path
	file_path: process.env.FILE_PATH || "/data/docs/"
};
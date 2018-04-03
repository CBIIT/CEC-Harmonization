import React, { Component } from 'react';

class Linkage extends Component {

  render() {
    return (
    <div>
      	<div id="table-intro" className="col-md-12">
		  <h2 className="table-title">
		  	<span id="tabLabel" className="subtitle">Linkages and Technology</span>
		  </h2>
		  <div className="table-description">
		    <p>The Cohort Overview compares the cohort design and the types of data and specimens collected across the cohorts you selected. To view more information about a specific cohort, select the acronym of the cohort at the top of the table.</p>
		  </div>
		</div>
		<div id="cedcd-cohorts-inner" className="col-md-12 activeArea">
			<div className="table-inner col-md-12">
				<div className="table-legend col-sm-9"> <span className="">N/A: Not Applicable; N/P: Not Provided</span> </div>
				<div className="table-export col-sm-3">
					<a id="exportTblBtn" href="javascript:void(0);">Export Table <span className="glyphicon glyphicon-export"></span></a>
				</div>
				<div className="clearFix"></div>
				<div className="cedcd-table">
					<p style={{fontWeight:"bold"}}>Todo...</p>
	            </div> 
	        </div>
	    </div>
	</div>);
  }
}

export default Linkage;
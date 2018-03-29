import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import './TableHeader.css';

class TableHeader extends Component {

  render() {
    let name;
    if(this.props.value === "cohort_name"){
      name="Cohort Name";
    }
    else if(this.props.value === "cohort_acronym"){
      name="Cohort Acronym";
    }
    else if(this.props.value === "update_time"){
      name="Last Updated";
    }
    else{
      name="Cohort Name";
    }
    let cls;
    if(this.props.value === this.props.orderBy.column){
      if(this.props.orderBy.order ==='asc'){
          cls = "glyphicon glyphicon-arrow-up glyphicon-active";
      }
      else{
          cls = "glyphicon glyphicon-arrow-down glyphicon-active";
      }
    }
    else{
        cls = "glyphicon glyphicon-arrow-down";
    }
    return (
      <th className="sortable" width={this.props.width} scope="col">
        <a href="javascript:void(0);" onClick={this.props.onClick}>{name}<span className={cls}></span></a>
      </th>
    );
  }
}

export default TableHeader;
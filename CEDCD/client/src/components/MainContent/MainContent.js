import React, { Component } from 'react';
import { Switch, Route } from 'react-router-dom';
import './MainContent.css';
import Home from '../Home/Home';
import Details from '../Details/Details';
import Enrollment from '../Enrollment/Enrollment';
import Cancer from '../Cancer/Cancer';
import Biospecimen from '../Biospecimen/Biospecimen';
import About from '../About/About';
import Information from '../Information/Information';

class MainContent extends Component {

  render() {
    return (
      <Switch>
        <Route exact path='/' component={Home}/>
        <Route path='/home' component={(props) => (
          <Home timestamp={new Date().toString()} {...props} />
        )}/>
        <Route path='/select' component={(props) => (
          <Details timestamp={new Date().toString()} {...props} />
        )}/>
        <Route path='/enrollment' component={Enrollment}/>
        <Route path='/cancer' component={Cancer}/>
        <Route path='/biospecimen' component={Biospecimen}/>
        <Route path='/about' component={About}/>
        <Route path='/cohort/:number' component={Information}/>
      </Switch>
    );
  }
}

export default MainContent;
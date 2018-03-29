import React, { Component } from 'react';
import './App.css';
import NavBar from '../NavBar/NavBar';
import MainContent from '../MainContent/MainContent';

class App extends Component {
  constructor(props){
    super(props);
    this.state = {
      currTab:0
    };
  }

  handleClick(i){
    localStorage.clear();
    this.setState({currTab: i});
  }

  componentDidMount(){
    let path = window.location.pathname;
    if(path == "/home" || path.startsWith("/home?")){
      setTimeout(() => {
        this.setState({ currTab: 0 });
      }, 100);
    }
    else if(path == "/select" || path.startsWith("/select?")){
      setTimeout(() => {
        this.setState({ currTab: 1 });
      }, 100);
    }
    else if(path == "/enrollment" || path.startsWith("/enrollment?")){
      setTimeout(() => {
        this.setState({ currTab: 2 });
      }, 100);
    }
    else if(path == "/cancer" || path.startsWith("/cancer?")){
      setTimeout(() => {
        this.setState({ currTab: 3 });
      }, 100);
    }
    else if(path == "/biospecimen" || path.startsWith("/biospecimen?")){
      setTimeout(() => {
        this.setState({ currTab: 4 });
      }, 100);
    }
    else if(path == "/about" || path.startsWith("/about?")){
      setTimeout(() => {
        this.setState({ currTab: 5 });
      }, 100);
    }
    else{
      setTimeout(() => {
        this.setState({ currTab: 0 });
      }, 100);
    }
  }

  render() {
    let content = (
      <MainContent/>
    );
    return (
      <div>
        <div id="mainNavBar">
          <div id="mainNavBar-inner">
            <NavBar currTab={this.state.currTab} onClick={(i) => this.handleClick(i)}/>
          </div>
        </div>
        <div id="cedcd-main-content" className="row">
          {content}
          <div className="clearFix"></div>
        </div>
      </div>
    );
  }
}

export default App;

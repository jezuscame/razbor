import React from 'react';
import './App.css';
import PingPong from './Functions/PingPong';

import Home from './Pages/Home'
import Dashboard from './Pages/Dashboard'
import Registration from './Pages/Registration'
import Profile from './Pages/Profile'
import {BrowserRouter, Routes, Route} from 'react-router-dom'
import Login from './Pages/Login';

// gaidynas

const App = () => {

  return (
    <BrowserRouter>
    <Routes>
      <Route path="/" element={<Home/>}/>
      <Route path="/dashboard" element={<Dashboard/>}/>
      <Route path="/registration" element={<Registration/>}/>
      <Route path="/login" element={<Login/>}/>
      <Route path="/profile" element={<Profile/>}/>
    </Routes>
    </BrowserRouter>



    //<div>
    //  <PingPong />
    //</div>
  );
}

export default App;
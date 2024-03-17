import { useEffect, useState } from "react";
import './assets/styles/app.scss';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import Product from './pages/Product';
import Login from './pages/Login';
import Register from './pages/Register';
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/prisijungti" element={<Login />} />
        <Route path="/registruotis" element={<Register />} />
        <Route path="/skelbimas/:productId" element={<Product />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

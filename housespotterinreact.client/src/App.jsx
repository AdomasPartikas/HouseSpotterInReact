import "./assets/styles/app.scss";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./contexts/AuthContext";
import Home from "./pages/Home";
import Product from "./pages/Product";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Favorite from "./pages/Favorite";
function App() {
  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/prisijungti" element={<Login />} />
          <Route path="/registruotis" element={<Register />} />
          <Route path="/megstamiausi" element={<Favorite />} />
          <Route path="/skelbimas/:productId" element={<Product />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
}

export default App;

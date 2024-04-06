import { useState } from "react";
import Header from "../components/header";
import { Link, useNavigate } from "react-router-dom";
import TextInput from "../components/form/TextInput";
import { useAuth } from "../contexts/AuthContext";
function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    if (name === "email") setEmail(value);
    else if (name === "password") setPassword(value);
  };

  async function loginUser() {
    const payload = {
      username: email,
      password: password,
    };

    try {
      const response = await fetch("housespotter/db/user/login", {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        const data = await response.json();
        login(data);
        navigate("/megstamiausi");
      } else {
        console.error("HTTP error:", response.status, response.statusText);
      }
    } catch (error) {
      console.error("Network error:", error);
    }
  }

  return (
    <div className="login">
      <Header />
      <div className="hero">
        <div className="layout">
          <form
            onSubmit={(event) => {
              event.preventDefault();
              loginUser();
            }}
          >
            <h1>Prisijungti</h1>
            <TextInput
              label="El. paštas"
              name="email"
              placeholder="Įveskite savo el. paštą"
              inputType="text"
              value={email}
              onChange={handleInputChange}
            />
            <TextInput
              label="Slaptažodis"
              name="password"
              placeholder="Įveskite savo slaptažodį"
              inputType="password"
              value={password}
              onChange={handleInputChange}
            />
            <button type="submit" className="primary__btn">
              Prisijungti
            </button>
            <p>Neturite paskyros?</p>
            <Link to="/registruotis" className="secondary__btn">
              Registruotis
            </Link>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Login;

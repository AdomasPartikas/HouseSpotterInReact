import { useState } from "react";
import Header from "../components/header";
import { Link } from "react-router-dom";
import TextInput from "../components/form/TextInput";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    if (name === "email") setEmail(value);
    else if (name === "password") setPassword(value);
  };

  return (
    <div className="login">
      <Header isAdmin={false} isLoggedIn={false} />
      <div className="hero">
        <div className="layout">
          <form action="">
            <h1>Prisijungti</h1>
            <TextInput
              label="El. paštas"
              name="email"
              placeholder="Įveskite savo el. paštą"
              inputType="email"
              onChange={handleInputChange}
            />
            <TextInput
              label="Slaptažodis"
              name="password"
              placeholder="Įveskite savo slaptažodį"
              inputType="password"
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

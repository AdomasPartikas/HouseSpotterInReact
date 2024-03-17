import { useState } from "react";
import Header from "../components/header";
import { Link } from "react-router-dom";
import TextInput from "../components/form/TextInput";

function Register() {
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
            <h1>Registruotis</h1>
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
              Registruotis
            </button>
            <p>Jau turite paskyrą?</p>
            <Link to="/prisijungti" className="secondary__btn">
              Prisijungti
            </Link>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Register;

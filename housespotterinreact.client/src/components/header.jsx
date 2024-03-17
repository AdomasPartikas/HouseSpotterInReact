import React from "react";
import { Link } from 'react-router-dom';

const Header = ({ isLoggedIn, isAdmin }) => {
  return (
    <header>
      <div className="layout">
        <div className="header__col-1">
          <a href="/" className="header__logo">
            HouseSpotter
          </a>
        </div>
        <div className="header__col-2">
          <nav className="header__nav">
            {isAdmin && <Link to="/scraper">Scraper</Link>}
            {isLoggedIn && <Link to="/favorite">Mėgstamiausi</Link>}
            {isLoggedIn && <Link to="/account">Paskyra</Link>}
            {!isLoggedIn && <Link to="/prisijungti" className="header__btn">Prisijungti</Link>}
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Header;

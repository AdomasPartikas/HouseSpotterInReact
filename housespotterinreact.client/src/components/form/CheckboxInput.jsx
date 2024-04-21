import React from "react";
import PropTypes from "prop-types";

function CheckboxInput({ label, name, onChange }) {
  return (
    <label htmlFor={name} className="checkbox">
      <input type="checkbox" id={name} name={name} onChange={onChange} />
      <span className="c-box"></span>
      <span className="c-text">{label}</span>
    </label>
  );
}

CheckboxInput.propTypes = {
  label: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired,
};

export default CheckboxInput;

import React from "react";
import PropTypes from "prop-types";

function SelectInput({ label, name, options = [], onChange, multiselect = false }) {
  return (
    <div className="input">
      <label htmlFor={name}>{label}</label>
      <select name={name} id={name} onChange={onChange} multiple={multiselect}>
        {options.length > 0 ? options.map((option, index) => (
          <option value={index} key={index}>
            {option}
          </option>
        )) : <option value="">Loading...</option>}
      </select>
    </div>
  );
}

SelectInput.propTypes = {
  label: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  options: PropTypes.array.isRequired,
  onChange: PropTypes.func.isRequired,
  multiselect: PropTypes.bool,
};

export default SelectInput;

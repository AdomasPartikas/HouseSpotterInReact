import React from "react";
import PropTypes from "prop-types";

function TextInput({
  label,
  name,
  nameFrom,
  nameTo,
  placeholder,
  placeholderFrom,
  placeholderTo,
  inputType = "text",
  onChange,
}) {
  if (nameFrom && nameTo) {
    return (
      <div className="input double">
        <label>{label}</label>
        <div className="input__double">
          <div className="input-from">
            <input
              type={inputType}
              name={nameFrom}
              id={nameFrom}
              placeholder={placeholderFrom}
              onChange={onChange}
            />
          </div>
          <div className="input-to">
            <input
              type={inputType}
              name={nameTo}
              id={nameTo}
              placeholder={placeholderTo}
              onChange={onChange}
            />
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="input">
      <label htmlFor={name}>{label}</label>
      <input
        type={inputType}
        name={name}
        id={name}
        placeholder={placeholder}
        onChange={onChange}
      />
    </div>
  );
}

TextInput.propTypes = {
  label: PropTypes.string.isRequired,
  name: PropTypes.string,
  nameFrom: PropTypes.string,
  nameTo: PropTypes.string,
  placeholder: PropTypes.string,
  placeholderFrom: PropTypes.string,
  placeholderTo: PropTypes.string,
  inputType: PropTypes.string,
  onChange: PropTypes.func.isRequired,
};

export default TextInput;

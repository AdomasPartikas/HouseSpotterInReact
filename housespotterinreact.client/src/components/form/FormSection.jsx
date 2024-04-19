import React from "react";
import TextInput from "./textInput";
import SelectInput from "./selectInput";
import CheckboxInput from "./checkboxInput";

import PropTypes from "prop-types";

function FormSection({ sectionOptions, handleInputChange, className }) {
  const content = (
    <>
      {sectionOptions.map(
        (
          {
            type,
            label,
            name,
            options,
            multiselect,
            nameFrom,
            nameTo,
            placeholderFrom,
            placeholderTo,
          },
          index
        ) => {
          switch (type) {
            case "select":
              return (
                <SelectInput
                  key={index}
                  label={label}
                  name={name}
                  options={options}
                  multiselect={multiselect}
                  onChange={handleInputChange}
                />
              );
            case "text":
              return (
                <TextInput
                  key={index}
                  label={label}
                  nameFrom={nameFrom}
                  nameTo={nameTo}
                  placeholderFrom={placeholderFrom}
                  placeholderTo={placeholderTo}
                  onChange={handleInputChange}
                />
              );
            case "checkbox":
              return (
                <CheckboxInput
                  key={index}
                  label={label}
                  name={name}
                  onChange={handleInputChange}
                />
              );
            default:
              return null;
          }
        }
      )}
    </>
  );

  if (className) {
    return <div className={className}>{content}</div>;
  } else {
    return <>{content}</>;
  }
}

FormSection.propTypes = {
  sectionOptions: PropTypes.array.isRequired,
  handleInputChange: PropTypes.func.isRequired,
  className: PropTypes.string,
};

export default FormSection;

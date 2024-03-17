import TextInput from "./TextInput";
import SelectInput from "./SelectInput";
import CheckboxInput from "./CheckboxInput";

export default function FormSection({ sectionOptions, handleInputChange }) {
  return (
    <>
      {sectionOptions.map(({ type, label, name, options, multiselect, nameFrom, nameTo, placeholderFrom, placeholderTo}, index) => {
        switch (type) {
          case 'select':
            return <SelectInput key={index} label={label} name={name} options={options} multiselect={multiselect} onChange={handleInputChange} />;
          case 'text':
            return <TextInput key={index} label={label} nameFrom={nameFrom} nameTo={nameTo} placeholderFrom={placeholderFrom} placeholderTo={placeholderTo} onChange={handleInputChange} />;
          case 'checkbox':
            return <CheckboxInput key={index} label={label} name={name} onChange={handleInputChange} />;
          default:
            return null;
        }
      })}
    </>
  );
}

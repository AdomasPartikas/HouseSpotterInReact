export default function SelectInput({ label, name, options, onChange, multiselect = false }) {
  return (
    <div className="input">
      <label htmlFor={name}>{label}</label>
      <select name={name} id={name} onChange={onChange} multiple={multiselect}>
        {options.map((option, index) => (
          <option value={index} key={index}>{option}</option>
        ))}
      </select>
    </div>
  );
}

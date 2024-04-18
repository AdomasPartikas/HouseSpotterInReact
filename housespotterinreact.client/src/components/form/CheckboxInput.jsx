import React from 'react';
export default function CheckboxInput({ label, name, onChange }) {
  return (
    <label htmlFor={name} className="checkbox">
      <input type="checkbox" id={name} name={name} onChange={onChange} />
      <span className="c-box"></span>
      <span className="c-text">{label}</span>
    </label>
  );
}
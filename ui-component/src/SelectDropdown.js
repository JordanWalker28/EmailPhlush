import React from 'react';
import { SelectContainer, SelectLabel, SelectDropdownElement, SelectDropdownMenu } from './SelectDropdownStyles';

function SelectDropdown({ label, id, value, onChange, options }) {
  return (
    <SelectContainer>
      <SelectLabel htmlFor={id}>{label}</SelectLabel>
      <SelectDropdownElement id={id} value={value} onChange={onChange}>
        {options.map((option) => (
          <option key={option} value={option}>
            {option}
          </option>
        ))}
      </SelectDropdownElement>
    </SelectContainer>
  );
}

export default SelectDropdown;

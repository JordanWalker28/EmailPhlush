import React from 'react';
import SelectDropdown from './SelectDropdown';

const DropdownList = ({ numItems, graphType, setNumItems, setGraphType }) => {
  const handleNumItemsChange = (event) => {
    const value = event.target.value;
    setNumItems(value === '*' ? Infinity : parseInt(value, 10));
  };

  const handleGraphTypeChange = (event) => {
    setGraphType(event.target.value);
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: '1rem' }}>
      <SelectDropdown
        label="Show Top:"
        id="numItems"
        value={numItems === Infinity ? '*' : numItems.toString()}
        onChange={handleNumItemsChange}
        options={['10', '20', '30', '*']} // Add more options as needed
      />
      <SelectDropdown
        label="Graph Type:"
        id="graphType"
        value={graphType}
        onChange={handleGraphTypeChange}
        options={['bar', 'pie', 'doughnut', 'polar']} // Add more options as needed
      />
    </div>
  );
};

export default DropdownList;

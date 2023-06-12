import React from 'react';
import SelectDropdown from './SelectDropdown';

const DropdownList = ({ numItems, graphType, setNumItems, setGraphType }) => {
  const handleNumItemsChange = (event) => {
    setNumItems(parseInt(event.target.value, 10));
  };

  const handleGraphTypeChange = (event) => {
    setGraphType(event.target.value);
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: '1rem' }}>
    <SelectDropdown
        label="Show Top:"
        id="numItems"
        value={numItems}
        onChange={handleNumItemsChange}
        options={['10', '20', '30']} // Add more options as needed
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

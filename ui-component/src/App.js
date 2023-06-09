import React, { useState } from 'react';
import DataSortingComponent from './DataSortingComponent';
import GraphComponent from './GraphComponent';
import TableComponent from './TableComponent';

function App() {
  const [numItems, setNumItems] = useState(10); // Number of items to display
  const [graphType, setGraphType] = useState("bar"); // Number of items to display
  const dataJson = require('./email_counts.json');

  const handleNumItemsChange = (event) => {
    setNumItems(parseInt(event.target.value, 10));
  };

  const handlegraphTypeChange = (event) => {
    setGraphType(event.target.value);
  };

  return (
    <div className="App" style={{ width: '100%', height: '100vh' }}>
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: '1rem' }}>
        <label htmlFor="numItems">Show Top:</label>
        <select id="numItems" value={numItems} onChange={handleNumItemsChange}>
          <option value="10">10</option>
          <option value="20">20</option>
          <option value="30">30</option>
          {/* Add more options as needed */}
        </select>
        <select id="numItems" value={graphType} onChange={handlegraphTypeChange}>
          <option value="bar">Bar</option>
          <option value="pie">Pie</option>
          {/* Add more options as needed */}
        </select>
      </div>
      <DataSortingComponent dataJson={dataJson} numItems={numItems}>
        {(sortedData) => (
          <>
          <div style={{ width: '100%', height: 'calc(50% - 2rem)' }}>
            <GraphComponent data={sortedData} graphType={graphType}/>
            </div>
            <div style={{ width: '100%', height: 'calc(50% - 2rem)' }}>
              <TableComponent data={sortedData} />
            </div>
          </>
        )}
      </DataSortingComponent>
    </div>
  );
}

export default App;

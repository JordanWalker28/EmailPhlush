import React, { useState } from 'react';
import DataSortingComponent from './DataSortingComponent';
import GraphComponent from './GraphComponent';
import TableComponent from './TableComponent';
import DropdownList from './DropdownList'; // New component

function App() {
  const [numItems, setNumItems] = useState(10); // Number of items to display
  const [graphType, setGraphType] = useState('bar'); // Number of items to display
  const dataJson = require('./email_counts.json');

  return (
    <div className="App" style={{ width: '100%', height: '100vh' }}>
        <DropdownList
          numItems={numItems}
          graphType={graphType}
          setNumItems={setNumItems}
          setGraphType={setGraphType}
        />
      <DataSortingComponent dataJson={dataJson} numItems={numItems}>
        {(sortedData) => (
          <>
            <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: '1rem', height: 'calc(90% - 2rem)' }}>
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

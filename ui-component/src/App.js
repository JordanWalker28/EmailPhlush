import React, { useState } from 'react';
import DataSortingComponent from './DataSortingComponent';
import GraphComponent from './GraphComponent';
import TableComponent from './TableComponent';
import DropdownList from './DropdownList';
import reportWebVitals from './reportWebVitals';

function App() {
  const [numItems, setNumItems] = useState(10); // Number of items to display
  const [graphType, setGraphType] = useState('bar'); // Graph type
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
            <GraphComponent data={sortedData} graphType={graphType} />
            <TableComponent data={sortedData}/>
          </>
        )}
      </DataSortingComponent>
    </div>
  );
}

reportWebVitals(console.log);


export default App;

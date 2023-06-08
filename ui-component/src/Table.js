import React, { useState, useEffect } from 'react';
import Chart from './Chart';
import Table from './Table';
import dataJson from './email_counts.json';

function App() {
  const [numItems, setNumItems] = useState(10);
  const [labels, setLabels] = useState([]);
  const [counts, setCounts] = useState([]);

  useEffect(() => {
    const dataB = Object.entries(dataJson);
    const sortedDataB = [...dataB].sort((a, b) => b[1] - a[1]);
    const selectedDataB = sortedDataB.slice(0, numItems);

    const selectedLabels = selectedDataB.map(([label, count]) => label);
    const selectedCounts = selectedDataB.map(([label, count]) => count);

    setLabels(selectedLabels);
    setCounts(selectedCounts);
  }, [dataJson, numItems]);

  const handleNumItemsChange = (event) => {
    setNumItems(parseInt(event.target.value, 10));
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
      </div>
      <div style={{ width: '100%', height: 'calc(100% - 2rem)' }}>
        <Chart labels={labels} counts={counts} />
      </div>
      <div style={{ width: '100%', marginTop: '2rem' }}>
        <Table labels={labels} counts={counts} />
      </div>
    </div>
  );
}

export default App;

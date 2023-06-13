import { useState, useEffect } from 'react';

function DataSortingComponent({ dataJson, numItems, children }) {
  const [sortedData, setSortedData] = useState([]);

  useEffect(() => {
    const dataB = Object.entries(dataJson);
    const sortedDataB = [...dataB].sort((a, b) => b[1] - a[1]); // Sort the data by count in descending order
    const selectedDataB = sortedDataB.slice(0, numItems); // Get the top N items based on numItems value

    setSortedData(selectedDataB);
  }, [dataJson, numItems]);

  return children(sortedData);
}

export default DataSortingComponent;

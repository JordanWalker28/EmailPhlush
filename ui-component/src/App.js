import {
  Chart as ChartJS,
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend
} from 'chart.js';
import React, { useState, useEffect } from 'react';
import { Bar } from 'react-chartjs-2';

ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip, Legend);

function getRandomColor() {
  const letters = '0123456789ABCDEF';
  let color = '#';
  for (let i = 0; i < 6; i++) {
    color += letters[Math.floor(Math.random() * 16)];
  }
  return color;
}

function App() {
  var dataJson = require('./email_counts.json');
  var dataB = Object.entries(dataJson);

  const [numItems, setNumItems] = useState(10); // Number of items to display
  const [labels, setLabels] = useState([]);
  const [counts, setCounts] = useState([]);
  const [colors, setColors] = useState([]);

  useEffect(() => {
    const sortedDataB = [...dataB].sort((a, b) => b[1] - a[1]); // Sort the data by count in descending order
    const selectedDataB = sortedDataB.slice(0, numItems); // Get the top N items based on numItems value

    const selectedLabels = selectedDataB.map(([label, count]) => label);
    const selectedCounts = selectedDataB.map(([label, count]) => count);
    const selectedColors = selectedDataB.map(() => getRandomColor()); // Generate random colors for each bar

    setLabels(selectedLabels);
    setCounts(selectedCounts);
    setColors(selectedColors);
  }, [numItems]);

  const data = {
    labels: labels,
    datasets: [
      {
        label: 'Count',
        data: counts,
        backgroundColor: colors,
        borderColor: 'black',
        borderWidth: 1,
      },
    ],
  };

  const options = {};

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
        <Bar data={data} options={options}></Bar>
      </div>
      <div style={{ width: '100%', marginTop: '2rem' }}>
        <table style={{ width: '80%', marginTop: '2rem', textAlign: 'center' }}>
          <thead>
            <tr>
              <th style={{ textAlign: 'center' }}>Label</th>
              <th style={{ textAlign: 'center' }}>Count</th>
            </tr>
          </thead>
          <tbody>
            {labels.map((label, index) => (
              <tr key={label}>
                <td style={{ textAlign: 'center' }}>{label}</td>
                <td style={{ textAlign: 'center' }}>{counts[index]}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default App;

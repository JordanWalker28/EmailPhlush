import React, { useEffect, useState } from 'react';
import { Bar } from 'react-chartjs-2';
import { Chart as ChartJS, BarElement, CategoryScale, LinearScale, Tooltip, Legend } from 'chart.js';

ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip, Legend);

function Chart({ dataJson, numItems }) {
  const [labels, setLabels] = useState([]);
  const [counts, setCounts] = useState([]);
  const [colors, setColors] = useState([]);

  useEffect(() => {
    const dataB = Object.entries(dataJson);
    const sortedDataB = [...dataB].sort((a, b) => b[1] - a[1]);
    const selectedDataB = sortedDataB.slice(0, numItems);

    const selectedLabels = selectedDataB.map(([label, count]) => label);
    const selectedCounts = selectedDataB.map(([label, count]) => count);
    const selectedColors = selectedDataB.map(() => getRandomColor());

    setLabels(selectedLabels);
    setCounts(selectedCounts);
    setColors(selectedColors);
  }, [dataJson, numItems]);

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

  return <Bar data={data} options={options} />;
}

function getRandomColor() {
  const letters = '0123456789ABCDEF';
  let color = '#';
  for (let i = 0; i < 6; i++) {
    color += letters[Math.floor(Math.random() * 16)];
  }
  return color;
}

export default Chart;

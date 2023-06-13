import React from 'react';
import { Bar, Doughnut, Pie, PolarArea } from 'react-chartjs-2';
import ChartJS from 'chart.js/auto';

import {
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend,
  ArcElement,
  RadialLinearScale,
} from 'chart.js';

ChartJS.register(
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend,
  ArcElement,
  RadialLinearScale
);

const GraphComponent = ({ data, graphType }) => {
  const ChartComponent = getChartComponent(graphType);

  const options = {
    elements: {
      bar: {
        borderWidth: 0.1,
      },
    },
    scales: {
      y: {
        beginAtZero: true,// Set the maximum value to 40
      },
      x: {
          max: 30,
      }
    },
  };

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: '1rem',
        height: 'calc(90% - 2rem)',
      }}
    >
      <ChartComponent
        data={{
          labels: data.map(([label]) => label),
          datasets: [
            {
              label: 'Count',
              data: data.map(([label, count]) => count),
              backgroundColor: data.map(() => getRandomColor()),
              borderColor: 'black',
              borderWidth: 1,
            },
          ],
        }}
        options={options} // Remove the extra curly braces around options
      />
    </div>
  );
};

const getChartComponent = (graphType) => {
  const chartComponents = {
    bar: Bar,
    pie: Pie,
    doughnut: Doughnut,
    polar: PolarArea,
  };

  return chartComponents[graphType] || Bar;
};

const getRandomColor = () => {
  const letters = '0123456789ABCDEF';
  let color = '#';
  for (let i = 0; i < 6; i++) {
    color += letters[Math.floor(Math.random() * 16)];
  }
  return color + '4D'; // Append '4D' to the color to set opacity to 0.3 (hex to decimal conversion: 0.3 * 255 = 76 => 4D in hexadecimal)
};

export default GraphComponent;

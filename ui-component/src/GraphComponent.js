import {
    Chart as ChartJS,
    BarElement,
    CategoryScale,
    LinearScale,
    Tooltip,
    Legend,
    ArcElement,
    RadialLinearScale,
  } from 'chart.js';
  import React from 'react';
  import { Bar, Doughnut, Pie, PolarArea } from 'react-chartjs-2';
  
  ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip, Legend, ArcElement,RadialLinearScale);
  
  function GraphComponent({ data, graphType }) {
    let ChartComponent;
    if (graphType === 'bar') {
      ChartComponent = Bar;
    } else if (graphType === 'pie') {
      ChartComponent = Pie;
    } else if (graphType === 'doughnut') {
      ChartComponent = Doughnut;
    } else if (graphType === 'polar') {
      ChartComponent = PolarArea;
    }  
    return (
      <ChartComponent
        style={{ height: '100px', width: '50%', marginTop: '2rem', position: 'center' }}
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
        options={{}}
      />
    );
  }
  
  function getRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
  
  export default GraphComponent;
  
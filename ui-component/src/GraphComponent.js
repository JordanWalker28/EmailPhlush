import {
    Chart as ChartJS,
    BarElement,
    CategoryScale,
    LinearScale,
    Tooltip,
    Legend,
    ArcElement
  } from 'chart.js';
  import React, { useState, useEffect } from 'react';
  import { Bar, Doughnut, Pie } from 'react-chartjs-2';
  
  ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip, Legend, ArcElement);
  
  function GraphComponent({ data, graphType }) {
    const ChartComponent = graphType === 'bar' ? Bar : Pie;
  
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
  
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

function GraphComponent({ data }) {
  return (
    <Bar
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

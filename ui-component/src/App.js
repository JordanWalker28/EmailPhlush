import {
  Chart as ChartJS,
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend
} from 'chart.js';

import { Bar} from 'react-chartjs-2';

ChartJS.register(
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend
)
function App() {

  const data = {
    labels: ['Mon','Tue', 'Wed'],
    datasets: [
      {
        label: '369',
        data: [3,6,9],
        backgroundColor:'aqua',
        borderColor: 'black',
        borderWidth: 1,
      }
    ]
  }

  const options = {

  }

  return (
    <div className="App">
      <div>
        <Bar
         data = {data}
         options = {options}
        ></Bar>
      </div>
    </div>
  );
}

export default App;

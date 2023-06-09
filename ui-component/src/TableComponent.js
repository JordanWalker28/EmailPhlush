import React from 'react';

function TableComponent({ data }) {
  return (
    <table style={{ width: '80%', marginTop: '2rem', textAlign: 'center' }}>
      <thead>
        <tr>
          <th style={{ textAlign: 'center' }}>Label</th>
          <th style={{ textAlign: 'center' }}>Count</th>
        </tr>
      </thead>
      <tbody>
        {data.map(([label, count]) => (
          <tr key={label}>
            <td style={{ textAlign: 'center' }}>{label}</td>
            <td style={{ textAlign: 'center' }}>{count}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default TableComponent;

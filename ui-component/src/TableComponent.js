import React from 'react';

const TableComponent = ({ data }) => {
  return (
    <table style={{ width: '80%', marginTop: '2rem', textAlign: 'center' }}>
      <thead>
        <tr>
          <th>Label</th>
          <th>Count</th>
        </tr>
      </thead>
      <tbody>
        {data.map(([label, count]) => (
          <tr key={label}>
            <td>{label}</td>
            <td>{count}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default TableComponent;

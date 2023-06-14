import React, { useState } from 'react';

const TableComponent = ({ data }) => {
  const [searchQuery, setSearchQuery] = useState('');

  // Filter the data based on the search query
  const filteredData = data.filter(([label]) =>
    label.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: '5rem',
      }}
    >      <input
        type="text"
        placeholder="Search..."
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        style={{ marginBottom: '1rem' }}
      />

      <table style={{ width: '80%', marginTop: '2rem', textAlign: 'center' }}>
        <thead>
          <tr>
            <th>Label</th>
            <th>Count</th>
          </tr>
        </thead>
        <tbody>
          {filteredData.map(([label, count]) => (
            <tr key={label}>
              <td>{label}</td>
              <td>{count}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default TableComponent;

import React, { useEffect, useState } from 'react';
import { getAllTemperatures } from './temperatures';

const intervalMs = 5000;

const App = ({data}) => {
  const [items, setItems] = useState({});
  useEffect(() => {
    const getTemperatures = () => getAllTemperatures(data)
      .then(
        (temperatures) => setItems(prevItems => ({...prevItems, ...temperatures })),
        () => console.error(`Getting temperatures failed`)
      );

    if (!items) {
      getTemperatures();
    } else {
      const interval = setTimeout(getTemperatures, intervalMs);
      return () => clearTimeout(interval);
    }
  });

  return (
    <div className="App">
      <h2>Beers</h2>
      <table>
        <thead>
          <tr>
            <th align="left">Product</th>
            <th align="left">Temperature</th>
            <th align="left">Status</th>
          </tr>
        </thead>
        <tbody>
          {Object.keys(items).map((itemKey) => (
            <tr id={items[itemKey].id} key={items[itemKey].id}>
              <td data-tid="product-name" width={150}>{items[itemKey].name}</td>
              <td data-tid="temperature-current" width={150}>{items[itemKey].temperature}</td>
              <td data-tid="temperature-status" width={150}>
                {items[itemKey].temperature <
                  items[itemKey].minimumTemperature && <span>too low</span>}
                {items[itemKey].temperature >
                  items[itemKey].maximumTemperature && <span>too high</span>}
                {items[itemKey].temperature <=
                  items[itemKey].maximumTemperature &&
                  items[itemKey].temperature >=
                    items[itemKey].minimumTemperature && <span>all good</span>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default App;

import { render, waitFor } from '@testing-library/react';
import App from './App';

const data = [
    {
      id: '1',
      name: 'Pilsner',
      minimumTemperature: 4,
      maximumTemperature: 6,
    },
    {
      id: '2',
      name: 'IPA',
      minimumTemperature: 5,
      maximumTemperature: 6,
    }];

const mockGetAllTemperatures = jest.fn();
jest.mock('./temperatures', () => ({
    getAllTemperatures: () => mockGetAllTemperatures()
}));

describe('<App />', () => {
    beforeEach(() => {
        jest.useFakeTimers();
        jest.clearAllMocks();

        mockGetAllTemperatures.mockResolvedValue([
        { ...data[0], temperature: 5 }, 
        { ...data[1], temperature: 1 } 
    ])
    });

    it('fetches data once and renders a table with valid content', async () => {
        const { container } = render(<App data={data}/>);
        await waitFor(() => {
            expect(container.querySelectorAll("tr").length).toEqual(3);
            expect(container.querySelector("tr[id='1'] td[data-tid='temperature-status'] span").textContent).toEqual("all good");
            expect(container.querySelector("tr[id='2'] td[data-tid='temperature-status'] span").textContent).toEqual("too low");
        });
        expect(mockGetAllTemperatures).toHaveBeenCalledTimes(1);
    });

    it('re-renders upon timeout', async () => {
        const { container } = render(<App data={data}/>);
        await waitFor(() => {
            expect(mockGetAllTemperatures).toHaveBeenCalledTimes(1);
        });
        mockGetAllTemperatures.mockResolvedValue([
            { ...data[0], temperature: -5 }, 
            { ...data[1], temperature: 30 } 
        ])
        jest.runAllTimers();
        await waitFor(() => {
            expect(container.querySelectorAll("tr").length).toEqual(3);
            expect(container.querySelector("tr[id='1'] td[data-tid='temperature-status'] span").textContent).toEqual("too low");
            expect(container.querySelector("tr[id='2'] td[data-tid='temperature-status'] span").textContent).toEqual("too high");
        });
        expect(mockGetAllTemperatures).toHaveBeenCalledTimes(2);
    })
});

import { useState, useEffect } from 'react';
import { fetchHousingData, processFilters } from '../api';

export const useHousingData = () => {
  const [filtersData, setFiltersData] = useState({
    settlement: [],
    street: [],
    equipment: [],
    heating: [],
    buildingType: [],
  });
  const [housingData, setHousingData] = useState([]);

  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await fetchHousingData();
        setFiltersData(processFilters(data));
        setHousingData(data);
      } catch (error) {
        console.error("Fetching error:", error);
      }
    };

    loadData();
  }, []);

  return { filtersData, housingData };
};

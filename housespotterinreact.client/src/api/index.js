export const fetchHousingData = async () => {
  const response = await fetch("housespotter/db/getallhousing");
  if (!response.ok) {
    throw new Error("Data could not be fetched");
  }
  return response.json();
};

export const processFilters = (data) => {
  const filters = {
    settlement: ["Pasirinkti"],
    street: ["Pasirinkti"],
    equipment: ["Pasirinkti"],
    heating: ["Pasirinkti"],
    buildingType: ["Pasirinkti"],
  };

  data.forEach(item => {
    const mapping = {
      gyvenviete: "settlement",
      gatve: "street",
      irengimas: "equipment",
      sildymas: "heating",
      pastatoTipas: "buildingType",
    };

    Object.entries(mapping).forEach(([key, filterKey]) => {
      const value = item[key] || `Pasirinkti`;
      if (value !== "Kita" && !filters[filterKey].includes(value)) {
        filters[filterKey].push(value);
      }
    });
  });

  const defaultOptions = ["Kita"];
  Object.keys(filters).forEach(filterKey => {
    if (filterKey !== "settlement" && filterKey !== "street" && filterKey !== "equipment") {
      filters[filterKey].push(...defaultOptions.filter(option => !filters[filterKey].includes(option)));
    }
  });

  return filters;
};

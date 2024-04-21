import React from "react";
import FormSection from "./form/FormSection";
import { objectType, sortingOptions } from "./FilterOptions";

import PropTypes from "prop-types";

const FilterForm = ({ filtersData, handleInputChange, handleSubmit }) => {
  if (filtersData.settlement.length === 0 || filtersData.street.length === 0) {
    return <div>Loading filters...</div>;
  }

  const column1Options = [
    {
      type: "select",
      label: "Objekto tipas",
      name: "objectType",
      options: objectType,
    },
    {
      type: "select",
      label: "Gyvenvietė",
      name: "settlement",
      options: filtersData.settlement,
    },
    {
      type: "select",
      label: "Gatvė",
      name: "street",
      options: filtersData.street,
    },
  ];

  const column2Options = [
    {
      type: "text",
      label: "Plotas, m²",
      nameFrom: "areaFrom",
      nameTo: "areaTo",
      placeholderFrom: "Nuo",
      placeholderTo: "Iki",
    },
    {
      type: "text",
      label: "Kambariai",
      nameFrom: "roomsFrom",
      nameTo: "roomsTo",
      placeholderFrom: "Nuo",
      placeholderTo: "Iki",
    },
    {
      type: "select",
      label: "Įrengimas",
      name: "equipment",
      options: filtersData.equipment,
    },
    {
      type: "text",
      label: "Kaina, €",
      nameFrom: "priceFrom",
      nameTo: "priceTo",
      placeholderFrom: "Nuo",
      placeholderTo: "Iki",
    },
  ];

  const column3Options = [
    {
      type: "text",
      label: "Aukštas",
      nameFrom: "floorFrom",
      nameTo: "floorTo",
      placeholderFrom: "Nuo",
      placeholderTo: "Iki",
    },
    {
      type: "text",
      label: "Metai",
      nameFrom: "yearFrom",
      nameTo: "yearTo",
      placeholderFrom: "Nuo",
      placeholderTo: "Iki",
    },
    {
      type: "select",
      label: "Šildymas",
      name: "heating",
      options: filtersData.heating,
    },
    {
      type: "select",
      label: "Pastato tipas",
      name: "buildingType",
      options: filtersData.buildingType,
    },
    {
      type: "text",
      label: "Kaina už m², €",
      nameFrom: "priceForAreaFrom",
      nameTo: "priceForAreaTo",
      placeholderFrom: "Nuo",
      placeholderTo: "Iki",
    },
  ];

  return (
    <form onSubmit={handleSubmit} className="filters">
      <div className="filters__row">
        <FormSection
          sectionOptions={column1Options}
          handleInputChange={handleInputChange}
          className="col"
        />
        <FormSection
          sectionOptions={column2Options}
          handleInputChange={handleInputChange}
          className="col"
        />
        <FormSection
          sectionOptions={column3Options}
          handleInputChange={handleInputChange}
          className="col"
        />
        <div className="col">
          <FormSection
            sectionOptions={sortingOptions}
            handleInputChange={handleInputChange}
          />
          <button type="submit" className="btn">Ieškoti</button>
        </div>
      </div>
    </form>
  );
};

FilterForm.propTypes = {
  filtersData: PropTypes.object.isRequired,
  handleInputChange: PropTypes.func.isRequired,
  handleSubmit: PropTypes.func.isRequired,
};

export default FilterForm;

import React, { useState } from "react";
import Header from "../components/header";
import FilterForm from "../components/filterForm";
import ProductList from "../components/productList";
import { useHousingData } from "../utils/useHousingData";

function Home() {
  // const [filter, setFilter] = useState(false);
  const [filtered, setFiltered] = useState(false);
  const { filtersData, housingData } = useHousingData();

  const [filteredData, setFilteredData] = useState([]);

  const [formData, setFormData] = useState({
    objectType: "0",
    settlement: "0",
    street: "0",
    areaFrom: "0",
    areaTo: "0",
    roomsFrom: "0",
    roomsTo: "0",
    equipment: "0",
    priceFrom: "0",
    priceTo: "0",
    floorFrom: "0",
    floorTo: "0",
    yearFrom: "0",
    yearTo: "0",
    heating: "0",
    buildingType: "0",
    priceForAreaFrom: "0",
    priceForAreaTo: "0",
    sorting: "0",
  });

  const handleInputChange = (event) => {
    const target = event.target;
    const value = target.type === "checkbox" ? target.checked : target.value;
    const name = target.name;

    setFormData((prevState) => {
      const updatedFormData = { ...prevState, [name]: value };
      return updatedFormData;
    });
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    setFiltered(false);
    let tempData = housingData;
    
    if (formData.objectType === "0") {
      tempData = tempData.filter((item) => item.bustoTipas === "butai");
      setFiltered(true);
    } else if (formData.objectType === "1") {
      tempData = tempData.filter((item) => item.bustoTipas === "namai");
      setFiltered(true);
    } else if (formData.objectType === "2") {
      tempData = tempData.filter((item) => item.bustoTipas === "butu-nuoma");
      setFiltered(true);
    } else if (formData.objectType === "3") {
      tempData = tempData.filter((item) => item.bustoTipas === "namu-nuoma");
      setFiltered(true);
    } 

    if (formData.settlement !== "0") {
      tempData = tempData.filter((item) => item.gyvenviete === filtersData.settlement[formData.settlement]);
      setFiltered(true);
    }

    if (formData.street !== "0") {
      tempData = tempData.filter((item) => item.gatve === filtersData.street[formData.street]);
      setFiltered(true);
    }

    if (formData.areaFrom !== "0") {
      tempData = tempData.filter((item) => item.plotas >= formData.areaFrom);
      setFiltered(true);
    }

    if (formData.areaTo !== "0") {
      tempData = tempData.filter((item) => item.plotas <= formData.areaTo);
      setFiltered(true);
    }

    if (formData.roomsFrom !== "0") {
      tempData = tempData.filter((item) => item.kambariuSk >= formData.roomsFrom);
      setFiltered(true);
    }

    if (formData.roomsTo !== "0") {
      tempData = tempData.filter((item) => item.kambariuSk <= formData.roomsTo);
      setFiltered(true);
    }

    if (formData.equipment !== "0") {
      tempData = tempData.filter((item) => item.apdaila === filtersData.equipment[formData.equipment]);
      setFiltered(true);
    }

    if (formData.priceFrom !== "0") {
      tempData = tempData.filter((item) => item.kaina >= formData.priceFrom);
      setFiltered(true);
    }

    if (formData.priceTo !== "0") {
      tempData = tempData.filter((item) => item.kaina <= formData.priceTo);
      setFiltered(true);
    }

    if (formData.floorFrom !== "0") {
      tempData = tempData.filter((item) => item.aukstas >= formData.floorFrom);
      setFiltered(true);
    }

    if (formData.floorTo !== "0") {
      tempData = tempData.filter((item) => item.aukstas <= formData.floorTo);
      setFiltered(true);
    }

    if (formData.yearFrom !== "0") {
      tempData = tempData.filter((item) => item.metai >= formData.yearFrom);
      setFiltered(true);
    }

    if (formData.yearTo !== "0") {
      tempData = tempData.filter((item) => item.metai <= formData.yearTo);
      setFiltered(true);
    }
    
    if (formData.heating !== "0") {
      tempData = tempData.filter((item) => item.sildymas === filtersData.heating[formData.heating]);
      setFiltered(true);
    }

    if (formData.buildingType !== "0") {
      tempData = tempData.filter((item) => item.pastatoTipas === filtersData.buildingType[formData.buildingType]);
      setFiltered(true);
    }

    if (formData.priceForAreaFrom !== "0") {
      tempData = tempData.filter((item) => (item.kaina / item.plotas).toFixed(2) >= formData.priceForAreaFrom);
      setFiltered(true);
    }

    if (formData.priceForAreaTo !== "0") {
      tempData = tempData.filter((item) => (item.kaina / item.plotas).toFixed(2) <= formData.priceForAreaTo);
      setFiltered(true);
    }

    if (formData.sorting !== "0") {
      if (formData.sorting === "1") {
        tempData.sort((a, b) => (a.kaina > b.kaina) ? 1 : -1);
      }
      if (formData.sorting === "2") {
        tempData.sort((a, b) => (a.kaina < b.kaina) ? 1 : -1);
      }
      if (formData.sorting === "3") {
        tempData.sort((a, b) => (a.plotas > b.plotas) ? 1 : -1);
      }
    }

    setFilteredData(tempData);
  };

  return (
    <div className="home">
      <Header />
      <div className="hero">
        <div className="layout">
          <FilterForm
            filtersData={filtersData}
            handleInputChange={handleInputChange}
            handleSubmit={handleSubmit}
          />
        </div>
      </div>
      <ProductList title={"Skelbimai"} housingData={filtered ? filteredData : housingData} />
    </div>
  );
}

export default Home;

import { useEffect, useState } from "react";
import Header from "../components/header";
import FormSection from "../components/form/FormSection";
import ProductCard from "../components/productCard";

function Home() {
  const objectTypeOptions = [
    "Butai pardavimui",
    "Namai pardavimui",
    "Patalpos pardavimui",
    "Butai nuomai",
    "Namai nuomai",
    "Nauji projektai",
    "Patalpos nuomai",
    "Sklypai",
    "Garažai/vietos",
    "Trumpalaikė nuoma",
  ];
  const municipalityOptions = [
    "Vilnius",
    "Kaunas",
    "Klaipėda",
    "Šiauliai",
    "Panevėžys",
    "Alytus",
    "Palanga",
  ];
  const settlementOptions = [
    "Vilniaus m.",
    "Ašmenos Kelio k.",
    "Aukštųjų Karklėnų k.",
    "Ąžuolijų vs.",
    "Bajorų k.",
    "Bališkių k.",
  ];
  const microdistrictOptions = [
    "Antakalnis",
    "Antavilis",
    "Aukštieji Paneriai",
    "Bajorai",
    "Balsiai",
    "Baltupiai",
  ];
  const streetOptions = [
    "A. Domaševičiaus g.",
    "A. Goštauto g.",
    "A. Jakšto g.",
    "A. Juozapavičiaus g.",
    "A. Kojelavičiaus g.",
    "A. Mickevičiaus g.",
  ];
  const equipmentOptions = [
    "Įrengimas",
    "Įrengtas",
    "Dalinė apdaila",
    "Neįrengtas",
    "Nebaigtas statyti",
    "Pamatai",
    "Kita",
  ];
  const heatingOptions = [
    "Šildymas",
    "Centrinis",
    "Centrinis kolektorinis",
    "Dujinis",
    "Elektra",
    "Aeroterminis",
    "Geoterminis",
    "Skystu kuru",
    "Kietu kuru",
    "Saulės energija",
    "Kita",
  ];
  const buildingTypeOptions = [
    "Pastato tipas",
    "Mūrinis",
    "Blokinis",
    "Monolitinis",
    "Medinis",
    "Karkasinis",
    "Rąstinis",
    "Skydinis",
    "Kita",
  ];
  const sortingOptions = [
    "Svarbesni viršuje",
    "Naujesni viršuje",
    "Pigesni viršuje",
    "Brangesni viršuje",
    "Mažesni viršuje",
  ];
  const dateOptions = [
    "Skelbimų data",
    "1 dienos",
    "3 dienų",
    "Savaitės",
    "Dviejų savaičių",
    "Mėnesio",
  ];

  const column1Options = [
    {
      type: "select",
      label: "Objekto tipas",
      name: "objectType",
      options: objectTypeOptions,
    },
    {
      type: "select",
      label: "Savivaldybė",
      name: "municipality",
      options: municipalityOptions,
    },
    {
      type: "select",
      label: "Gyvenvietė",
      name: "settlement",
      options: settlementOptions,
    },
    {
      type: "select",
      label: "Mikrorajonas",
      name: "microdistrict",
      options: microdistrictOptions,
    },
    {
      type: "select",
      label: "Gatvė",
      name: "street",
      options: streetOptions,
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
      options: equipmentOptions,
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
      options: heatingOptions,
    },
    {
      type: "select",
      label: "Pastato tipas",
      name: "buildingType",
      options: buildingTypeOptions,
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
  const column4Options = [
    { type: "checkbox", label: "Vyksta atvirų durų diena", name: "openDoor" },
    { type: "checkbox", label: "Iš privačių asmenų", name: "privateSeller" },
    { type: "checkbox", label: "Iš agentūrų", name: "agencySeller" },
    { type: "checkbox", label: "Nauji projektai", name: "newProjects" },
  ];
  const column5Options = [
    {
      type: "select",
      label: "Rūšiavimas",
      name: "sorting",
      options: sortingOptions,
    },
    {
      type: "select",
      label: "Skelbimų data",
      name: "date",
      options: dateOptions,
    },
  ];

  const [formData, setFormData] = useState({
    objectType: "0",
    municipality: "0",
    settlement: "",
    microdistrict: [],
    street: [],
    areaFrom: "",
    areaTo: "",
    roomsFrom: "",
    roomsTo: "",
    equipment: "",
    priceFrom: "",
    priceTo: "",
    floorFrom: "",
    floorTo: "",
    yearFrom: "",
    yearTo: "",
    heating: "",
    buildingType: "",
    priceForAreaFrom: "",
    priceForAreaTo: "",
    openDoor: false,
    privateSeller: false,
    agencySeller: false,
    newProjects: false,
    sorting: "0",
    date: "",
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

  const [housingData, setHousingData] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch("housespotter/db/getallhousing");
        if (!response.ok) throw new Error("Data could not be fetched");
        const data = await response.json();
        setHousingData(data);
      } catch (error) {
        console.error("Fetching error:", error);
      }
    };

    fetchData();
  }, []);

  return (
    <div className="home">
      <Header />
      <div className="hero">
        <div className="layout">
          <form action="" className="filters">
            <div className="filters__row">
              <div className="filters__col-1">
                <FormSection
                  sectionOptions={column1Options}
                  handleInputChange={handleInputChange}
                />
              </div>
              <div className="filter__col-2">
                <FormSection
                  sectionOptions={column2Options}
                  handleInputChange={handleInputChange}
                />
              </div>
              <div className="filters__col-3">
                <FormSection
                  sectionOptions={column3Options}
                  handleInputChange={handleInputChange}
                />
              </div>
              <div className="filters__col-4">
                <FormSection
                  sectionOptions={column4Options}
                  handleInputChange={handleInputChange}
                />
              </div>
              <div className="filters__col-5">
                <FormSection
                  sectionOptions={column5Options}
                  handleInputChange={handleInputChange}
                />
                <button className="btn">Ieškoti</button>
              </div>
            </div>
          </form>
        </div>
      </div>

      <div className="products">
        <div className="layout">
          <h2>
            Skelbimai <span>({housingData.length})</span>
          </h2>
          {housingData.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </div>
    </div>
  );
}

export default Home;
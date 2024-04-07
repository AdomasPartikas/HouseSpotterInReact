import { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import Header from "../components/header";
import ProductCard from "../components/productCard";

function Favorite() {
  const { user, refreshFavoriteHousing } = useAuth();
  const [housingData, setHousingData] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(
          `housespotter/db/user/${user.id}/savedSearches`
        );
        if (!response.ok) throw new Error("Data could not be fetched");
        const data = await response.json();
        setHousingData(data);
      } catch (error) {
        console.error("Fetching error:", error);
      }
    };

    fetchData();
  }, [refreshFavoriteHousing]);

  return (
    <div className="favorite">
      <Header />
      <div className="products">
        <div className="layout">
          <h2>
            Mėgstamiausi <span>({housingData.length})</span>
          </h2>
          {housingData.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </div>
    </div>
  );
}

export default Favorite;
import React, { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import Header from "../components/header";
import ProductList from "../components/productList";

function Favorite() {
  const { user, refreshFavoriteHousing } = useAuth();
  const [favoritesData, setFavoritesData] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(
          `/housespotter/db/user/${user.id}/savedSearches`
        );
        if (!response.ok) throw new Error("Data could not be fetched");
        const data = await response.json();
        setFavoritesData(data);
      } catch (error) {
        console.error("Fetching error:", error);
      }
    };

    fetchData();
  }, [refreshFavoriteHousing]);

  return (
    <div className="favorite">
      <Header />
      <ProductList title={"Mėgstamiausi"} housingData={favoritesData} />
    </div>
  );
}

export default Favorite;

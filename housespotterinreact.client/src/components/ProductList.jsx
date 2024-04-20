import React from "react";
import PropTypes from "prop-types";

import ProductCard from "./ProductCard";

const ProductList = ({ title, housingData }) => {
  return (
    <div className="products">
      <div className="layout">
        <h2>
          {title} <span>({housingData.length})</span>
        </h2>
        {housingData.length === 0 && <p className="not-found">Nerasta jokių skelbimų</p>}
        {housingData.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
};

ProductList.propTypes = {
  housingData: PropTypes.array.isRequired,
};

export default ProductList;

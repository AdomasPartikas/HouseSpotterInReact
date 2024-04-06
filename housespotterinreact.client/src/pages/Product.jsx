import { useParams } from "react-router-dom";
import Header from "../components/header";

import products from "../api/products.json";

function Product() {
  let { productId } = useParams();

  const product = products.find(
    (product) => product.id === parseInt(productId)
  );

  if (!product) {
    return <div>Product not found</div>;
  }

  return (
    <div className="product">
      <Header />
      <div className="hero">
        <div className="layout">
          <div className="product__photo"></div>
          <div className="product__content">
            <h1>{product.title}</h1>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Product;

const getTemperature = async (product) => {
    const response = await fetch(`http://localhost:25539/temperature/${product.id}`);
    const json = await response.json();
    return {
        ...product,
        ...json,
     }
 };
 
export const getAllTemperatures = async (data) => {
    var products = await Promise.all(data.map(getTemperature));
    return products.reduce((result, product) => ({ ...result, [product.id]: { ...product } }), {});
}

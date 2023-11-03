using SurfsUpClassLibrary.Models;

namespace SurfsUpClassLibrary.Product
{
    /// <summary>
    /// Used for product methods.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Gets a product.
        /// </summary>
        /// <param name="sku">The unique sku reference.</param>
        /// <returns>A <see cref="ProductModel"/> type.</returns>
        Task<Board?> Get(int id);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>A <see cref="IList<ProductModel>"/> type.</returns>
        Task<IList<Board>> GetAll();
    }
}

using SurfsUpClassLibrary.Models;
using SurfsUpClassLibrary.ShoppingCart.Models;

namespace SurfsUpClassLibrary.Storage
{
    /// <summary>
    /// Stores the data used for the application.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Stores a list of products.
        /// </summary>
        IList<Board> Products { get; }

        /// <summary>
        /// Stores the shopping cart.
        /// </summary>
        ShoppingCartModel ShoppingCart { get; }

    }
}

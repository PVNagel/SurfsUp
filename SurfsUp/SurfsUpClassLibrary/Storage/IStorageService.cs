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
        /// Stores the shopping cart.
        /// </summary>
        ShoppingCartModel ShoppingCart { get; }

    }
}

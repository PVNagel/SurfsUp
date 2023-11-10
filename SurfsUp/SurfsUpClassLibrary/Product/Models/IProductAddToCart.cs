using SurfsUpClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfsUpClassLibrary.Product.Models
{
    /// <summary>
    /// The information needed in a product listing Razor component
    /// </summary>
    public interface IProductAddToCart
    {
        // An instance of the product
        Board? Product { get; set; }

        // The quantity wishing to be added to the cart
        int? Quantity { get; set; }

        /// <summary>
        /// The method to add a product to cart
        /// </summary>
        Task AddToCart();
    }
}

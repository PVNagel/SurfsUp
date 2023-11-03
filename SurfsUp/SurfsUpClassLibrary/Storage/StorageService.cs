using SurfsUpClassLibrary.Models;
using SurfsUpClassLibrary.ShoppingCart.Models;
using System.Net.Http.Json;

namespace SurfsUpClassLibrary.Storage
{
    /// <summary>
    /// Stores the data used for the application.
    /// </summary>
    public class StorageService : IStorageService
    {
        /// <summary>
        /// Stores a list of products.
        /// </summary>

        private readonly HttpClient httpClient ;


        /// <summary>
        /// Stores the shopping cart.        
        /// </summary>
        public ShoppingCartModel ShoppingCart { get; private set; }

        /// <summary>
        ///  Constructs a storage service.
        /// </summary>
        public StorageService()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7022") };
            ShoppingCart = new ShoppingCartModel();

          
        }

        public async Task<Board?> Get(int id)
        {
            var board = await httpClient.GetFromJsonAsync<Board>($"/v2/BoardsAPI/GetById/{id}");
            return board;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>A <see cref="IList<ProductModel>"/> type.</returns>
        public async Task<IList<Board>> GetAll()
        {
            var boards = await httpClient.GetFromJsonAsync<List<Board>>("/v2/BoardsAPI/GetAllBoards");
            return boards;
        }

    }
}

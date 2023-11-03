using SurfsUpClassLibrary.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace SurfsUpClassLibrary.Product
{

    /// <summary>
    /// Used for product methods.
    /// </summary>
    public class ProductService : IProductService
    {
        /// <summary>
        /// A private reference to the storage service from the DI container.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Constructs a product service.
        /// </summary>
        /// <param name="storageService">A reference to the storage service from the DI container.</param>
        public ProductService()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7022") };
        }

        /// <summary>
        /// Gets a product.
        /// </summary>
        /// <param name="sku">The unique sku reference.</param>
        /// <returns>A <see cref="Board"/> type.</returns>
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

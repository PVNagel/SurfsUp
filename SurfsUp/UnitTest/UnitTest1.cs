using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace MyApi.Tests
{
    [TestClass]
    public class ApiConnectionTests
    {
        private const string apiUrl = "https://localhost:7111"; // Replace with your API's URL

        [TestMethod]
        public void TestApiConnection()
        {
            // Arrange
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                // Act
                HttpResponseMessage response = client.GetAsync("").Result; // Send a GET request to the API's root endpoint

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Succeded to connect to the API.");
            }
        }
    }
}

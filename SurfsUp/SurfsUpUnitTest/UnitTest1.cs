using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace SurfsUpUnitTest
{
    [TestClass]
    public class ApiConnectionTests
    {
        private const string apiUrl = "https://localhost:7022/WeatherForecast"; // Replace with your API's URL

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
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to connect to the API.");
            }
        }
    }
}
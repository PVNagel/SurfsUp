using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SurfsUp.Controllers;
using SurfsUp.Data;
using SurfsUpClassLibrary.Models;
using Microsoft.AspNetCore.Identity;

namespace SurfsUpUnitTest
{
    [TestClass]
    public class ApiConnectionTests
    {
        private const string apiUrl = "https://localhost:7022"; // Replace with your API's URL

        [TestMethod]
        public void TestApiConnection()
        {
            // Arrange
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                // Act
                HttpResponseMessage response = client.GetAsync($"/RentingsAPI/Get/60173bf4-c37c-4c47-8f23-c68591bace90").Result; // Send a GET request to the API's root endpoint

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to connect to the API.");
            }
        }

        // Mocked DbContext
        private Mock<SurfsUpContext> _contextMock;

        // Mocked UserManager
        private Mock<UserManager<SurfsUpUser>> _userManagerMock;

        //[TestMethod]
        //public async Task CheckIfBoardIsRentedCorrectly()
        //{
        //    // Arrange
        //    var controller = new RentingsAPIController(_contextMock.Object, _userManagerMock.Object);
        //    var model = new Renting
        //    {
        //        BoardId = 1,
        //        SurfsUpUserId = "test",
        //        EndDate = new DateTime(2023, 9, 30, 0, 0, 0)
        //    };

        //    // Act
        //    var result = await controller.Create(model) as RedirectToActionResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Index", result.ActionName);

        //}

    }

}
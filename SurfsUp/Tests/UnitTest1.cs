using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SurfsUp.Controllers;
using SurfsUp.Data;
using SurfsUp.Models;
using SurfsUp.Services;

namespace Tests
{
    [TestClass]
    public class BoardsControllerTests
    {
        [TestMethod]
        public async Task Create_WithValidData_RedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SurfsUpContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name for the in-memory database
                .Options;

            using (var context = new SurfsUpContext(options))
            {
                context.Database.EnsureCreated(); // Ensure the database is created

                var controller = new BoardsController(context, Mock.Of<IHostEnvironment>(), Mock.Of<ImageService>());

                var board = new Board
                {
                    Name = "TestBoard",
                    Length = 6.0,
                    Width = 20.0,
                    Thickness = 2.75,
                    Volume = 35.0,
                    Type = "TestType",
                    Price = 100.0,
                    Equipment = "TestEquipment"
                };

                var mockFormFile = new Mock<IFormFile>();
                var attachments = new List<IFormFile> { mockFormFile.Object };

                // Act
                var result = await controller.Create(board, attachments) as RedirectToActionResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.ActionName);
            }
        }
    }
}

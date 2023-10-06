namespace Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CanGetRentings()
        {
            // Arrange
            var client = _factory.CreateClient();
            var response = await client.GetAsync("https://localhost:7022/RentingsAPI/TestApi");

                response.EnsureSuccessStatusCode();


        }



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
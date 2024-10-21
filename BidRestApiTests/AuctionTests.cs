namespace BidRestApiTests
{
  
    using global::BidRestAPI.Controllers;
    using global::BidRestAPI.Interfaces;
    using global::BidRestAPI.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    namespace BidRestAPI.Tests
    {
        [TestFixture]
        public class AuctionControllerTests
        {
            private Mock<IAuctionService> _auctionServiceMock;
            private Mock<IAuthService> _authServiceMock;
            private Mock<ILogger<AuctionController>> _loggerMock;
            private AuctionController _controller;

            [SetUp]
            public void Setup()
            {
                _auctionServiceMock = new Mock<IAuctionService>();
                _authServiceMock = new Mock<IAuthService>();
                _loggerMock = new Mock<ILogger<AuctionController>>();
                _controller = new AuctionController(_auctionServiceMock.Object, _authServiceMock.Object, _loggerMock.Object);
            }

            [Test]
            public async Task GetAuctions_ShouldReturnOkResultWithAuctions()
            {
                // Arrange
                var mockAuctions = new List<AuctionItem> { new AuctionItem { Id = Guid.NewGuid(), Title = "Test Auction" } };
                _auctionServiceMock.Setup(service => service.GetAuctionItemsAsync()).ReturnsAsync(mockAuctions);

                // Act
                var result = await _controller.GetAuctions();

                // Assert
                var okResult = result as OkObjectResult;
                Assert.IsNotNull(okResult);
                Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
                Assert.AreEqual(mockAuctions, okResult.Value);
            }

            [Test]
            public async Task GetAuctions_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                _auctionServiceMock.Setup(service => service.GetAuctionItemsAsync()).ThrowsAsync(new Exception("Some error"));

                // Act
                var result = await _controller.GetAuctions();

                // Assert
                var statusCodeResult = result as ObjectResult;
                Assert.IsNotNull(statusCodeResult);
                Assert.AreEqual((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            }

            [Test]
            public async Task CreateAuction_WithValidData_ShouldReturnCreatedResult()
            {
                // Arrange
                var dto = new CreateAuctionDto { UserId = Guid.NewGuid(), Title = "New Auction" };
                var auction = new AuctionItem { Id = Guid.NewGuid(), Title = dto.Title };
                _authServiceMock.Setup(auth => auth.UserExistsAsync(dto.UserId)).ReturnsAsync(true);
                _auctionServiceMock.Setup(service => service.CreateAuctionItemAsync(dto, dto.UserId)).ReturnsAsync(auction);

                // Act
                var result = await _controller.CreateAuction(dto);

                // Assert
                var createdResult = result as CreatedAtActionResult;
                Assert.IsNotNull(createdResult);
                Assert.AreEqual((int)HttpStatusCode.Created, createdResult.StatusCode);
                Assert.AreEqual(auction, createdResult.Value);
            }

            [Test]
            public async Task CreateAuction_WithInvalidModel_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new CreateAuctionDto(); // Invalid DTO
                _controller.ModelState.AddModelError("Title", "Title is required.");

                // Act
                var result = await _controller.CreateAuction(dto);

                // Assert
                var badRequestResult = result as BadRequestObjectResult;
                Assert.IsNotNull(badRequestResult);
                Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            }

            [Test]
            public async Task DeleteAuction_WithValidId_ShouldReturnOk()
            {
                // Arrange
                var auctionId = Guid.NewGuid();
                var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }));

                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = mockUser }
                };

                _auctionServiceMock.Setup(service => service.DeleteAuctionItemAsync(auctionId)).ReturnsAsync(true);

                // Act
                var result = await _controller.DeleteAuction(auctionId);

                // Assert
                var okResult = result as OkObjectResult;
                Assert.IsNotNull(okResult);
                Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            }

          

            [Test]
            public async Task DeleteAuction_WhenUserNotAuthenticated_ShouldReturnUnauthorized()
            {
                // Arrange
                var auctionId = Guid.NewGuid();

                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
                };

                // Act
                var result = await _controller.DeleteAuction(auctionId);

                // Assert
                var unauthorizedResult = result as UnauthorizedObjectResult;
                Assert.IsNotNull(unauthorizedResult);
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, unauthorizedResult.StatusCode);
            }
        }
    }
}
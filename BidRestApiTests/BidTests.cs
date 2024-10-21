using BidRestAPI.Controllers;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;

namespace BidRestAPI.Tests
{
    [TestFixture]
    public class BidControllerTests
    {
        private Mock<IBidService> _bidServiceMock;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<ILogger<BidController>> _loggerMock;
        private BidController _controller;

        [SetUp]
        public void Setup()
        {
            _bidServiceMock = new Mock<IBidService>();
            _loggerMock = new Mock<ILogger<BidController>>();

            // Mock UserManager using custom setup
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _controller = new BidController(_bidServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task PlaceBid_WithValidData_ShouldReturnOkResult()
        {
            // Arrange
            var auctionItemId = Guid.NewGuid();
            var dto = new PlaceBidDto { Amount = 100.00M };
            var userId = "test-user-id";

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _bidServiceMock.Setup(bidService => bidService.PlaceBidAsync(auctionItemId, dto.Amount, userId))
                           .ReturnsAsync(new Bid { AuctionItemId = auctionItemId, UserId = userId, Amount = dto.Amount });

            // Act
            var result = await _controller.PlaceBid(auctionItemId, dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.IsInstanceOf<Bid>(okResult.Value);
        }

        [Test]
        public async Task PlaceBid_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var auctionItemId = Guid.NewGuid();
            var dto = new PlaceBidDto(); // Invalid DTO
            _controller.ModelState.AddModelError("Amount", "Amount is required.");

            // Act
            var result = await _controller.PlaceBid(auctionItemId, dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task PlaceBid_WhenUserNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            var auctionItemId = Guid.NewGuid();
            var dto = new PlaceBidDto { Amount = 100.00M };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null);

            // Act
            var result = await _controller.PlaceBid(auctionItemId, dto);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, unauthorizedResult.StatusCode);
        }

        [Test]
        public async Task PlaceBid_WhenExceptionThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var auctionItemId = Guid.NewGuid();
            var dto = new PlaceBidDto { Amount = 100.00M };
            var userId = "test-user-id";

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _bidServiceMock.Setup(bidService => bidService.PlaceBidAsync(auctionItemId, dto.Amount, userId))
                           .ThrowsAsync(new Exception("Error during bidding"));

            // Act
            var result = await _controller.PlaceBid(auctionItemId, dto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }
    }
}

using System;
using System.Threading.Tasks;
using BidRestAPI.Controllers;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BidRestAPI.Tests
{
    [TestFixture]
    public class BidControllerTests
    {
        private Mock<IBidService> _mockBidService;
        private Mock<IAuthService> _mockAuthService;
        private Mock<ILogger<BidController>> _mockLogger;
        private BidController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBidService = new Mock<IBidService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<BidController>>();
            _controller = new BidController(_mockBidService.Object, _mockLogger.Object, _mockAuthService.Object);
        }

        [Test]
        public async Task PlaceBid_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Amount", "Required");
            var dto = new PlaceBidDto { UserId = Guid.NewGuid(), Amount = 100 };

            // Act
            var result = await _controller.PlaceBid(Guid.NewGuid(), dto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        [Test]
        public async Task PlaceBid_ReturnsBadRequest_WhenUserIdIsEmpty()
        {
            // Arrange
            var dto = new PlaceBidDto { UserId = Guid.Empty, Amount = 100 };

            // Act
            var result = await _controller.PlaceBid(Guid.NewGuid(), dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("User ID must be provided.", badRequestResult.Value);
        }

        [Test]
        public async Task PlaceBid_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var dto = new PlaceBidDto { UserId = Guid.NewGuid(), Amount = 100 };
            _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await _controller.PlaceBid(Guid.NewGuid(), dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("User ID does not exist.", badRequestResult.Value);
        }

        [Test]
        public async Task PlaceBid_ReturnsOk_WhenBidIsPlacedSuccessfully()
        {
            // Arrange
            var dto = new PlaceBidDto { UserId = Guid.NewGuid(), Amount = 100 };
            var auctionItemId = Guid.NewGuid();
            var bid = new Bid { Id = Guid.NewGuid(), AuctionItemId = auctionItemId, Amount = 100, UserId = dto.UserId.ToString() };

            _mockAuthService.Setup(x => x.UserExistsAsync(dto.UserId)).ReturnsAsync(true);
            _mockBidService.Setup(x => x.PlaceBidAsync(auctionItemId, dto.Amount, dto.UserId)).ReturnsAsync(bid);

            // Act
            var result = await _controller.PlaceBid(auctionItemId, dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(bid, okResult.Value);
        }
    }
}

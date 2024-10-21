using BidRestAPI.Controllers;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BidRestAPI.Tests
{
    [TestFixture]
    public class AuthTests
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

       


        [Test]
        public async Task Login_WithNullDto_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Login(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task Login_WhenExceptionThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var dto = new UserLoginDto { Email = "test@example.com", Password = "Password123" };
            _authServiceMock.Setup(auth => auth.LoginAsync(dto)).ThrowsAsync(new Exception("Error during login"));

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }
    }
}

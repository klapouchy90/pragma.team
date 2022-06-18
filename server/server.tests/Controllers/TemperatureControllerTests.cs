using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using server.Controllers;
using server.Models;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace server.tests.Controllers
{
    [TestClass]
    public class TemperatureControllerTests
    {
        private Mock<HttpMessageHandler> mockMessageHandler;

        [TestInitialize]
        public void Initialize()
        {
            mockMessageHandler = new Mock<HttpMessageHandler>();
            MockSendResponse(ItExpr.IsAny<HttpRequestMessage>(), new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        private void MockSendResponse(Expression requestExpr, HttpResponseMessage response)
        {
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", requestExpr, ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private void MockSendResponse(Sensor sensor)
        {
            MockSendResponse(
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsoluteUri == string.Format(TemperatureController.SensorServiceUrl, Uri.EscapeDataString(sensor.Id))),
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(sensor, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }))
                });
        }

        public TemperatureController GetController()
        {
            var mockedFactory = new Mock<IHttpClientFactory>();
            mockedFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockMessageHandler.Object));
            return new TemperatureController(mockedFactory.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsValidResult()
        {
            var sensor = new Sensor { Id = "1/2", Temperature = 30 };
            MockSendResponse(sensor);
            var controller = GetController();
            var response = await controller.Get(sensor.Id);
            mockMessageHandler
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            Assert.IsTrue(response is OkObjectResult);
            var result = response as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(sensor, result.Value);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFoundWhenSensorDoesntExist()
        {
            var controller = GetController();
            var response = await controller.Get("5");
            mockMessageHandler
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            Assert.IsTrue(response is StatusCodeResult);
            var result = response as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequestWhenModelStateInvalid()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("id", "Id cannot be null");
            var response = await controller.Get(null);
            Assert.IsTrue(response is BadRequestObjectResult);
            mockMessageHandler
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }
    }
}

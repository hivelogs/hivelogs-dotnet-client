using FakeItEasy;
using Hivelogs.Client.Abstractions;
using Hivelogs.Client.Dtos;
using Hivelogs.Client.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Hivelogs.Client.Tests
{
    public class HivelogsMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ExecutesNextDelegateSuccessfully()
        {
            // Arrange
            var fakeHivelogsClient = A.Fake<IHivelogsClient>();
            var fakeLogger = A.Fake<ILogger<HivelogsMiddleware>>();

            RequestDelegate next = async context =>
            {
                await context.Response.WriteAsync("Test response");
            };

            var middleware = new HivelogsMiddleware(next, fakeHivelogsClient, fakeLogger);

            var context = new DefaultHttpContext();
            var requestBodyText = "Test request body";
            var requestBytes = Encoding.UTF8.GetBytes(requestBodyText);
            context.Request.Body = new MemoryStream(requestBytes);
            context.Request.ContentLength = requestBytes.Length;
            context.Request.Method = "GET";
            context.Request.Path = "/test";

            // Act
            await middleware.InvokeAsync(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            Assert.Equal("Test response", responseBody);

            A.CallTo(() => fakeHivelogsClient.SendLogAsync(A<CreateLogDto>.That.Matches(dto =>
                dto.Message == "Request started" &&
                dto.HttpMethod == "GET" &&
                dto.RequestPath == "/test" &&
                dto.RequestBody == requestBodyText)))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHivelogsClient.SendLogAsync(A<CreateLogDto>.That.Matches(dto =>
                dto.Message == "Request finished" &&
                dto.HttpMethod == "GET" &&
                dto.RequestPath == "/test" &&
                dto.ResponseBody == "Test response" &&
                dto.ResponseStatus == context.Response.StatusCode.ToString())))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHivelogsClient.SendLogAsync(A<CreateLogDto>.That.Matches(dto =>
                dto.Message == "Unhandled exception in middleware")))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_LogsErrorOnException()
        {
            // Arrange
            var fakeHivelogsClient = A.Fake<IHivelogsClient>();
            var fakeLogger = A.Fake<ILogger<HivelogsMiddleware>>();

            RequestDelegate next = context => throw new Exception("Test exception");

            var middleware = new HivelogsMiddleware(next, fakeHivelogsClient, fakeLogger);

            var context = new DefaultHttpContext();
            var requestBodyText = "Test request body";
            var requestBytes = Encoding.UTF8.GetBytes(requestBodyText);
            context.Request.Body = new MemoryStream(requestBytes);
            context.Request.ContentLength = requestBytes.Length;
            context.Request.Method = "POST";
            context.Request.Path = "/error";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => middleware.InvokeAsync(context));

            A.CallTo(() => fakeHivelogsClient.SendLogAsync(A<CreateLogDto>.That.Matches(dto =>
                dto.Message == "Request started" &&
                dto.HttpMethod == "POST" &&
                dto.RequestPath == "/error" &&
                dto.RequestBody == requestBodyText)))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHivelogsClient.SendLogAsync(A<CreateLogDto>.That.Matches(dto =>
                dto.Message == "Unhandled exception in middleware" &&
                dto.HttpMethod == "POST" &&
                dto.RequestPath == "/error" &&
                dto.RequestBody == requestBodyText &&
                dto.Exception.Contains("Test exception"))))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHivelogsClient.SendLogAsync(A<CreateLogDto>.That.Matches(dto =>
                dto.Message == "Request finished" &&
                dto.HttpMethod == "POST" &&
                dto.RequestPath == "/error" &&
                dto.ResponseStatus == context.Response.StatusCode.ToString())))
                .MustHaveHappenedOnceExactly();
        }
    }
}
namespace Hivelogs.Client.Sample.Services
{
    public interface ITestService
    {
        public Task DoWorkAsync();
        public Task DoWithExceptionAsync();
    }
}
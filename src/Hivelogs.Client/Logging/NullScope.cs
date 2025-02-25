namespace Hivelogs.Client.Logging
{
    internal class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
}

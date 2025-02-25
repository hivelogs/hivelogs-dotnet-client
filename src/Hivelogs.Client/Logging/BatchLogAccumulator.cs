using Hivelogs.Client.Dtos;

namespace Hivelogs.Client.Logging
{
    public class BatchLogAccumulator : ILogAccumulator
    {
        private readonly List<CreateLogDto> _logs = [];
        public void AddLog(CreateLogDto log) => _logs.Add(log);
        public IEnumerable<CreateLogDto> GetLogs() => _logs;
        public void Clear() => _logs.Clear();
    }
}

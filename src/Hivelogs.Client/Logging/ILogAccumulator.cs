using Hivelogs.Client.Dtos;

namespace Hivelogs.Client.Logging
{
    public interface ILogAccumulator
    {
        void AddLog(CreateLogDto log);
        IEnumerable<CreateLogDto> GetLogs();
        void Clear();
    }
}

using Hivelogs.Client.Dtos;

namespace Hivelogs.Client.Abstractions
{
    public interface IHivelogsClient
    {
        Task SendLogAsync(CreateLogDto log);
        Task SendLogsAsync(IEnumerable<CreateLogDto> logs);
    }
}

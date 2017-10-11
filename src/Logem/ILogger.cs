using System.Threading.Tasks;

namespace Logem
{
    public interface ILogger
    {
        Task LogAsync(string log, object data);
    }
}

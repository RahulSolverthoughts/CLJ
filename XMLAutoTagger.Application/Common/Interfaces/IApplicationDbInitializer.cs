using System.Threading.Tasks;

namespace XMLAutoTagger.Application.Common.Interfaces
{
    public interface IApplicationDbInitializer
    {
        Task InitializeAsync();
    }
}

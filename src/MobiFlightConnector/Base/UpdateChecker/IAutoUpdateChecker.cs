using System.Threading.Tasks;

namespace MobiFlight.Base.UpdateChecker;

public interface IAutoUpdateChecker
{
    Task CheckForUpdateAsync(bool silent = false);
}
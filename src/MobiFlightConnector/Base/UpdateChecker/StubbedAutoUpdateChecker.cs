using System.Threading.Tasks;

namespace MobiFlight.Base.UpdateChecker;

public sealed class StubbedAutoUpdateChecker : IAutoUpdateChecker
{
    public Task CheckForUpdateAsync(bool silent = false)
    {
        return Task.CompletedTask;
    }
}
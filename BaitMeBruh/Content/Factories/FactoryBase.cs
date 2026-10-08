using Vapok.Common.Abstractions;
using Vapok.Common.Managers.Configuration;

namespace BaitMeBruh.Content.Factories;

public abstract class FactoryBase
{
    private readonly ILogIt _logger;
    private readonly ConfigSyncBase _config;

    internal ILogIt Log => _logger;
    internal ConfigSyncBase Config => _config;

    internal FactoryBase(ILogIt logger, ConfigSyncBase configs)
    {
        _logger = logger;
        _config = configs;
    }
}

using Gagspeak.API.Dto.User;
using FFStreamViewer.WebAPI.PlayerData.Pairs;
using FFStreamViewer.WebAPI.Services.Mediator;
using FFStreamViewer.WebAPI.Services.ServerConfiguration;
using GagSpeak.API.Dto.UserPair;

namespace FFStreamViewer.WebAPI.PlayerData.Factories;

public class PairFactory
{
    private readonly PairHandlerFactory _cachedPlayerFactory;               // the factory of cached players
    private readonly ILoggerFactory _loggerFactory;                         // the logger factory
    private readonly GagspeakMediator _gagspeakMediator;                    // the gagspeak Mediator service
    private readonly ServerConfigurationManager _serverConfigurationManager;// the server configuration manager (primary i think?)

    public PairFactory(ILoggerFactory loggerFactory, PairHandlerFactory cachedPlayerFactory,
        GagspeakMediator gagspeakMediator, ServerConfigurationManager serverConfigurationManager)
    {
        _loggerFactory = loggerFactory;
        _cachedPlayerFactory = cachedPlayerFactory;
        _gagspeakMediator = gagspeakMediator;
        _serverConfigurationManager = serverConfigurationManager;
    }

    /// <summary> Creates a new Pair object from the UserPairDto</summary>
    /// <param name="userPairDto"> The data transfer object of a user pair</param>
    /// <returns> A new Pair object </returns>
    public Pair Create(UserPairDto userPairDto)
    {
        return new Pair(_loggerFactory.CreateLogger<Pair>(), new(userPairDto.User, userPairDto.IndividualPairStatus),
            _cachedPlayerFactory, _gagspeakMediator, _serverConfigurationManager);
    }
}

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using RetakesPlugin.Managers;
using RetakesPlugin.Utils;

namespace RetakesPlugin.Commands.Player;

public class PlayCommand
{
    private readonly RetakesPlugin _plugin;
    private readonly GameManager _gameManager;

    public PlayCommand(RetakesPlugin plugin, GameManager gameManager)
    {
        _plugin = plugin;
        _gameManager = gameManager;
    }

    public void OnCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!PlayerHelper.IsValid(player))
        {
            return;
        }

        if (_gameManager.QueueManager.ActivePlayers.Contains(player) || _gameManager.QueueManager.QueuePlayers.Contains(player))
        {
            player.PrintToChat($"{_plugin.Localizer["retakes.prefix"]} You are already in the queue or playing.");
            return;
        }

        // Simulate joining T side to trigger queue addition
        _gameManager.QueueManager.PlayerJoinedTeam(player, player!.Team, CsTeam.Terrorist);

        // If they were added to active players immediately (e.g. warmup or empty server), ensure they are on a valid team
        if (_gameManager.QueueManager.ActivePlayers.Contains(player) && player.Team == CsTeam.Spectator)
        {
            player.ChangeTeam(CsTeam.CounterTerrorist);
        }

        player.PrintToChat($"{_plugin.Localizer["retakes.prefix"]} You have joined the queue.");
        Logger.LogInfo("Commands", $"{player.PlayerName} used !play command");
    }
}

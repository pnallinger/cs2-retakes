using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using RetakesPlugin.Managers;
using RetakesPlugin.Utils;

namespace RetakesPlugin.Commands.Player;

public class AfkCommand
{
    private readonly RetakesPlugin _plugin;
    private readonly GameManager _gameManager;

    public AfkCommand(RetakesPlugin plugin, GameManager gameManager)
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

        if (player!.Team == CsTeam.Spectator)
        {
            player.PrintToChat($"{_plugin.Localizer["retakes.prefix"]} You are already AFK (Spectator).");
            return;
        }

        player.ChangeTeam(CsTeam.Spectator);
        
        // Ensure they are removed from queues if ChangeTeam doesn't trigger OnPlayerTeam immediately or if we want to be safe
        // Note: OnPlayerTeam event handler handles RemoveSpectators, which calls RemovePlayerFromQueues.
        // However, checking changing logic, often direct calls to RemovePlayerFromQueues are safer if we are manipulating state.
        
        // If the player is in ActivePlayers or QueuePlayers, we remove them.
        if (_gameManager.QueueManager.ActivePlayers.Contains(player) || _gameManager.QueueManager.QueuePlayers.Contains(player))
        {
             _gameManager.QueueManager.RemovePlayerFromQueues(player);
        }

        player.PrintToChat($"{_plugin.Localizer["retakes.prefix"]} You are now AFK (Spectator).");
        Logger.LogInfo("Commands", $"{player.PlayerName} used !afk command");
    }
}

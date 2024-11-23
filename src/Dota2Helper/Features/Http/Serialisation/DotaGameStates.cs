namespace Dota2Helper.Features.Http.Serialisation;

public static class DotaGameStates
{
    /*
     * https://dev.overwolf.com/ow-native/reference/live-game-data-gep/supported-games/dota-2/#game_state_changed
     * DOTA_GAMERULES_STATE_WAIT_FOR_PLAYERS_TO_LOAD
     * DOTA_GAMERULES_STATE_HERO_SELECTION
     * DOTA_GAMERULES_STATE_STRATEGY_TIME
     * DOTA_GAMERULES_STATE_PRE_GAME
     * DOTA_GAMERULES_STATE_GAME_IN_PROGRESS
     * DOTA_GAMERULES_STATE_POST_GAME
     * DOTA_GAMERULES_STATE_TEAM_SHOWCASE
     */

    public const string DOTA_GAMERULES_STATE_PRE_GAME = nameof(DOTA_GAMERULES_STATE_PRE_GAME);

    public const string DOTA_GAMERULES_STATE_GAME_IN_PROGRESS = nameof(DOTA_GAMERULES_STATE_GAME_IN_PROGRESS);
}
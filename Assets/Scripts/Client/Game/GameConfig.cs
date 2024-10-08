/// <summary>
/// Configs for the game
/// </summary>
public static class GameConfig
{
    /// <summary>
    /// How many cells should the chunk contain (CHUNK_SIZE^2)
    /// </summary>
    public const int CHUNK_SIZE = 32;

    /// <summary>
    /// How many chunks should the game have (CHUNK_COUNT^2)
    /// </summary>
    public const int CHUNK_COUNT = 64;

    /// <summary>
    /// How big is a single cell (x and z axis)
    /// </summary>
    public const int CHUNK_CELL = 32;

    /// <summary>
    /// What is the chunk's scale
    /// </summary>
    public const float CHUNK_SCALE = 1f;

    /// <summary>
    /// What is the lowest amount of damage which can be taken
    /// </summary>
    public const float CAR_DAMAGE_LOWER_LIMIT = 0.5f;

    /// <summary>
    /// What is the highest amount of damage which can be taken
    /// </summary>
    public const float CAR_DAMAGE_UPPER_LIMIT = 10f;

    /// <summary>
    /// At what percentage of health does the car starts smoking
    /// </summary>
    public const float SMOKE_THRESHOLD = 0.6f;

    /// <summary>
    /// At what percentage of health does the car starts burning
    /// </summary>
    public const float FIRE_THRESHOLD = 0.2f;

    /// <summary>
    /// How long should the police car touch the player to trigger a game over
    /// </summary>
    public const float POLICE_GAME_OVER = 1.5f;

    /// <summary>
    /// The forward raycast's distance
    /// </summary>
    public const float POLICE_RAYCAST_FORWARD_DISTANCE = 15f;

    /// <summary>
    /// The side raycast's distance
    /// </summary>
    public const float POLICE_RAYCAST_SIDE_DISTANCE = 17f;

    /// <summary>
    /// At what distance should the reverse movement be triggered
    /// </summary>
    public const float POLICE_REVERSE_DISTANCE = 8f;

    /// <summary>
    /// How frequest should the coin's spawning be
    /// </summary>
    public const float COIN_RATE = 0.1f;

    /// <summary>
    /// How long should the turbo last
    /// </summary>
    public const float TURBO_DURATION = 5f;

    /// <summary>
    /// How long should the immunity last
    /// </summary>
    public const float IMMUNITY_DURATION = 3f;

    /// <summary>
    /// How much does the turbo costs
    /// </summary>
    public const int TURBO_COST = 250;

    /// <summary>
    /// How much does the immunity costs
    /// </summary>
    public const int IMMUNITY_COST = 500;

    /// <summary>
    /// How much does the double costs
    /// </summary>
    public const int DOUBLE_COIN_COST = 1000;
}
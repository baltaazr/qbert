using System;
using System.Collections.Generic;
public class Config
{
  public const int MAP_SIZE = 7;

  public const int NUMBER_OF_CUBES = (MAP_SIZE * (MAP_SIZE + 1)) / 2;

  public const double HEX_SIZE = 1;

  public const double VERTICAL_OFFSET = 1;

  public static double[] PLAYER_OFFSET = new double[2] { 0, 0.5 };

  public static double[] BALL_OFFSET = new double[2] { 0, 0.3 };

  public static double[] WRONGWAY_OFFSET = new double[2] { -0.4, -0.3 };

  public static double[] UGG_OFFSET = new double[2] { 0.4, -0.3 };

  public static double[] PLAYER_OFFSET_3D = new double[3] { 0, 1.2, 0 };

  public static double[] BALL_OFFSET_3D = new double[3] { 0, 0.75, 0 };

  public static double[] WRONGWAY_OFFSET_3D = new double[3] { -1.75, 0, 0 };

  public static double[] UGG_OFFSET_3D = new double[3] { 0, 0, -1 };

  public static string[] HEX_SURROUNDINGS = new string[4] {
    "botLeft","botRight","topLeft","topRight"
  };

  public static Dictionary<string, Coords> HEX_SURROUNDINGS_OFFSET = new Dictionary<string, Coords>()
    {
        {"topLeft", new Coords(0,-1)},
        {"topRight", new Coords(1,-1)},
        {"botLeft", new Coords(-1,1)},
        {"botRight", new Coords(0,1)},
        {"left", new Coords(-1,0)},
        {"right", new Coords(1,0)}
    };

  public const int NUMBER_OF_LEVELS = 5;

  public const int NUMBER_OF_ROUNDS = 4;

  public const float ANIMATION_DELAY = 0.1f;

  // 5 levels in total
  // 4 rounds per level

  public static int[,][] LEVELS = new int[,][]{
    { new int[2]{0,1}, new int[2]{0,5}, new int[2]{0,6}, new int[2]{0,7} },

    { new int[2]{5,3}, new int[3]{5,18,4}, new int[3]{5,4,6}, new int[3]{5,1,19} },

    { new int[3]{6,3,4}, new int[3]{6,19,8}, new int[3]{6,1,0}, new int[3]{6,5,20} } ,

    { new int[2]{8,1}, new int[2]{8,6}, new int[3]{8,3,4}, new int[3]{8,7,9} } ,

    { new int[3]{11,9,10}, new int[3]{10,8,11}, new int[3]{11,6,13}, new int[4]{2,4,3,12} } ,

    { new int[3]{13,17,14}, new int[4]{13,12,18,16}, new int[4]{16,12,10,14}, new int[5]{15,16,18,20,21} }
  };

  // {Chance of Redball, Chance of Ugg / Wrongway, Chance of Slick / Sam}
  // 3 = 3 types enemies apart from Coily
  // Coily spawns once each round at the beginning
  public static float[,,] ENEMY_SPAWNING_ALGORITHM = new float[NUMBER_OF_LEVELS, NUMBER_OF_ROUNDS, 3]{
    {{1f,0f,0f},{1f,0f,0f},{1f,0f,0f},{0.7f, 0.3f,0f}},
    {{0.8f,0.2f,0f},{0.55f,0.45f,0f},{0.5f,0.5f,0f},{0.1f, 0.9f,0f}},
    {{0f,1f,0f},{0.2f,0.7f,0.1f},{0.5f,0f,0.5f},{0.3f, 0.3f,0.4f}},
    {{0.2f,0.3f,0.5f},{0f,1f,0f},{0.2f,0.2f,0.6f},{0.25f, 0.25f,0.5f}},
    {{0.1f,0.2f,0.7f},{0.2f,0.4f,0.4f},{0f,0f,1f},{0f, 0.5f,0.5f}}
  };

  public const float MOVE_TIME = 0.2f;

  public const float JUMP_TIME = 1f;

  public const float JUMP_HEIGHT = 0.5f;

  public const float ENEMY_FALLING_TIME = 1.1f;

  public const float PLAYER_REAPPEAR_DELAY = 3f;

  public const float GAME_OVER_DELAY = 2f;

  public const float INIT_ENEMIES_DELAY = 3f;

  public const float DELAY_PER_ENEMY_MOVE = 1f;

  public const float DELAY_PER_ENEMY_SPAWN = 5f;

  public const float LEVEL_START_DELAY = 3f;

  public const float INIT_TELE_DELAY = 0.5f;

  public const int END_ROUND_COLOR_CHANGING_REPEATS = 10;
}
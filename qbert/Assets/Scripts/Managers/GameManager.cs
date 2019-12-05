using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
  public static GameManager instance = null;

  public string type;
  public GameObject coily;
  public GameObject redBall;
  public GameObject wrongway;
  public GameObject ugg;
  public GameObject slick;
  public GameObject sam;
  public GameObject lifeSprite;
  public Sprite[] gameOverSprites;
  [HideInInspector] public MapManager mapScript;
  [HideInInspector] public bool playersTurn = true;
  [HideInInspector] public bool doingSetup = true;

  private List<GameObject> lifeSprites;
  private GameObject levelImage;
  private GameObject gameOverImage;
  private GameObject endingImage;
  private int lives = 2;
  private int level = 1;
  private int round = 1;
  private List<Enemy> enemies;
  private int nCoily = 0;

  void Awake()
  {
    if (instance == null)
      instance = this;
    else if (instance != null)
      Destroy(gameObject);
    DontDestroyOnLoad(gameObject);
    enemies = new List<Enemy>();
    mapScript = GetComponent<MapManager>();
    InitGame();
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  static public void CallbackInitialization()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
  {
    if (MapManager.loaded)
    {
      instance.round += 1;
      if (instance.round > Config.NUMBER_OF_ROUNDS)
      {
        instance.round = 1;
        instance.level += 1;

      }
      if (instance.level <= Config.NUMBER_OF_LEVELS)
      {
        instance.InitGame();
      }
      else
      {
        instance.End();
      }
    }
  }

  void InitGame()
  {
    mapScript.SetupScene(level, round);
    InitLives();
    InitUI();
    StartCoroutine(InitGameScreens());
  }

  public void EndRound()
  {
    playersTurn = true;
    doingSetup = true;
    nCoily = 0;
    enemies.Clear();
    StopAllCoroutines();
  }

  void InitLives()
  {
    lives = level + 1;
    lifeSprites = new List<GameObject>();
    Transform livesHolder = GameObject.Find("Lives").transform;
    for (int i = 0; i < lives; i++)
    {
      GameObject instance = Instantiate(lifeSprite, livesHolder) as GameObject;
      instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, -150f - i * (50f));
      lifeSprites.Add(instance);
    }
  }

  void InitUI()
  {
    levelImage = GameObject.Find("LevelImage");
    gameOverImage = GameObject.Find("GameOverImage");
    endingImage = GameObject.Find("EndingImage");
    gameOverImage.SetActive(false);
    endingImage.SetActive(false);

    TextMeshProUGUI roundText = GameObject.Find("RoundText").GetComponent<TextMeshProUGUI>();
    Image alterToCubeImage = GameObject.Find("AlterToCube").GetComponent<Image>();
    TextMeshProUGUI levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
    roundText.text = string.Format("Round: {0}", round);
    levelText.text = string.Format("Level: {0}", level);
    if (type == "2D")
      alterToCubeImage.sprite = mapScript.getAlterToCubeColor();
    else
      alterToCubeImage.material = mapScript.getAlterToCubeColor();
  }

  IEnumerator InitGameScreens()
  {
    if (round == 1)
    {
      TextMeshProUGUI levelImageText = GameObject.Find("LevelImageText").GetComponent<TextMeshProUGUI>();
      levelImageText.text = string.Format("{0}", level);
      levelImage.SetActive(true);
      yield return new WaitForSeconds(Config.LEVEL_START_DELAY);
    }
    levelImage.SetActive(false);
    doingSetup = false;
    yield return new WaitForSeconds(Config.INIT_ENEMIES_DELAY);
    InitEnemies();
  }

  public void LoseLife()
  {
    lives -= 1;
    GameObject lostLife = lifeSprites[lifeSprites.Count - 1];
    Destroy(lostLife);
    lifeSprites.Remove(lostLife);
    StopAllCoroutines();
    Invoke("DestroyEnemies", Config.PLAYER_REAPPEAR_DELAY);
    if (lives == 0)
      Invoke("GameOver", Config.GAME_OVER_DELAY);
    else
      Invoke("InitEnemies", Config.INIT_ENEMIES_DELAY + Config.PLAYER_REAPPEAR_DELAY);
  }

  void GameOver()
  {
    doingSetup = true;
    StartCoroutine(GameOverAnimation());
  }

  void End()
  {
    endingImage.SetActive(true);
    doingSetup = true;
    StopAllCoroutines();
    enemies.Clear();
    StartCoroutine(StopGame());
  }

  public void AddEnemyToList(Enemy script)
  {
    enemies.Add(script);
  }

  public void RemoveEnemyFromList(Enemy script)
  {
    enemies.Remove(script);
  }

  IEnumerator SpawnEnemy()
  {
    for (; ; )
    {
      if (nCoily < 1)
      {
        Instantiate(coily, new Vector3(-1, -1, 0f), Quaternion.identity);
        nCoily += 1;
      }
      else
      {
        double redBallChance = Config.ENEMY_SPAWNING_ALGORITHM[level - 1, round - 1, 0];
        double uggWrongwayChance = Config.ENEMY_SPAWNING_ALGORITHM[level - 1, round - 1, 1];
        float rand = Random.value;
        if (rand < redBallChance)
        {
          Instantiate(redBall, new Vector3(-1, -1, 0f), Quaternion.identity);
        }
        else if (rand < redBallChance + uggWrongwayChance)
        {
          if (Random.Range(0, 2) == 1)
            Instantiate(ugg, new Vector3(-1, -1, 0f), Quaternion.identity);
          else
            Instantiate(wrongway, new Vector3(-1, -1, 0f), Quaternion.identity);
        }
        else
        {

          if (Random.Range(0, 2) == 1)
            Instantiate(slick, new Vector3(-1, -1, 0f), Quaternion.identity);
          else
            Instantiate(sam, new Vector3(-1, -1, 0f), Quaternion.identity);
        }
      }

      yield return new WaitForSeconds(Config.DELAY_PER_ENEMY_SPAWN);
    }
  }

  IEnumerator MoveEnemies()
  {
    for (; ; )
    {
      for (int i = 0; i < enemies.Count; i++)
      {
        enemies[i].MoveEnemy();
      }
      yield return new WaitForSeconds(Config.DELAY_PER_ENEMY_MOVE);
    }
  }

  void InitEnemies()
  {
    StartCoroutine(SpawnEnemy());
    StartCoroutine(MoveEnemies());
  }

  void DestroyEnemies()
  {
    foreach (Enemy enemy in enemies)
      enemy.DestroyEnemy();
    enemies.Clear();
    nCoily = 0;
  }

  IEnumerator GameOverAnimation()
  {
    gameOverImage.SetActive(true);
    Image gameOverSprite = GameObject.Find("GameOverSprite").GetComponent<Image>();
    for (int i = 0; i < gameOverSprites.Length; i++)
    {
      gameOverSprite.sprite = gameOverSprites[i];
      yield return new WaitForSeconds(Config.ANIMATION_DELAY);
    }
    StartCoroutine(StopGame());
  }

  IEnumerator StopGame()
  {
    yield return new WaitForSeconds(3f);
    Destroy(gameObject);
    SceneManager.LoadScene(0);
  }

  public void DecreaseCoilyCount() { nCoily -= 1; }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
  }
}

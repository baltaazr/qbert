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

  public float levelStartDelay = 2f;
  public float roundEndDelay = 2f;
  public float delayPerEnemySpawn = 5f;
  public float delayPerEnemyMove = 1f;
  public float initEnemySpawnDelay = 2f;
  public float deathDelay = 2f;
  public int lives = 2;
  public GameObject coily;
  public GameObject redBall;
  public GameObject wrongway;
  public GameObject ugg;
  public GameObject slick;
  public GameObject sam;
  public GameObject lifeSprite;
  public List<GameObject> lifeSprites;
  public Sprite[] gameOverSprites;
  [HideInInspector] public MapManager mapScript;
  [HideInInspector] public bool playersTurn = true;
  [HideInInspector] public bool doingSetup = true;

  private GameObject levelImage;
  private GameObject gameOverImage;
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
    instance.round += 1;
    if (instance.round > Config.NUMBER_OF_ROUNDS)
    {
      instance.round = 1;
      instance.level += 1;
    }

    instance.InitGame();
  }

  void InitGame()
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
    playersTurn = true;
    StopAllCoroutines();

    levelImage = GameObject.Find("LevelImage");
    gameOverImage = GameObject.Find("GameOverImage");
    gameOverImage.SetActive(false);
    if (round == 1)
    {
      doingSetup = true;
      TextMeshProUGUI levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
      TextMeshProUGUI levelImageText = GameObject.Find("LevelImageText").GetComponent<TextMeshProUGUI>();
      levelText.text = string.Format("Level: {0}", level);
      levelImageText.text = string.Format("{0}", level);
      levelImage.SetActive(true);
      Invoke("HideLevelImage", levelStartDelay);
    }
    else
    {
      levelImage.SetActive(false);
      Invoke("InitEnemies", initEnemySpawnDelay);
    }
    enemies.Clear();
    mapScript.SetupScene(level, round);
    nCoily = 0;

    TextMeshProUGUI roundText = GameObject.Find("RoundText").GetComponent<TextMeshProUGUI>();
    Image alterToCubeImage = GameObject.Find("AlterToCube").GetComponent<Image>();
    roundText.text = string.Format("Round: {0}", round);
    alterToCubeImage.sprite = mapScript.getAlterToCubeSprite();

  }

  void HideLevelImage()
  {
    levelImage.SetActive(false);
    doingSetup = false;
    Invoke("InitEnemies", initEnemySpawnDelay);
  }

  public void LoseLife()
  {
    lives -= 1;
    GameObject lostLife = lifeSprites[lifeSprites.Count - 1];
    Destroy(lostLife);
    lifeSprites.Remove(lostLife);
    if (lives == 0)
    {
      Invoke("GameOver", deathDelay);
    }
  }

  void GameOver()
  {
    doingSetup = true;
    StopAllCoroutines();
    enemies.Clear();
    StartCoroutine(GameOverAnimation());
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

      yield return new WaitForSeconds(delayPerEnemySpawn);
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
      yield return new WaitForSeconds(delayPerEnemyMove);
    }
  }

  void InitEnemies()
  {
    StartCoroutine(SpawnEnemy());
    StartCoroutine(MoveEnemies());
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
  }
  public void DecreaseCoilyCount() { nCoily -= 1; }
}

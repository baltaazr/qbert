using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
  public GameObject cube;
  public GameObject tele;
  public AudioClip nextCubeSound1;
  public AudioClip nextCubeSound2;
  public AudioClip nextCubeSound3;
  public AudioClip prevCubeSound;
  public AudioClip teleSound;
  public static bool loaded = false;
  // 2D
  public Sprite[] cubeSprites;
  public Sprite[] teleDisappearSprites;
  // 3D
  public Material[] cubeMaterials;

  private Map map = new Map();
  private Transform mapHolder;
  private Dictionary<string, GameObject> cubes;
  private Dictionary<string, GameObject> teles;
  private int cubesChanged = 0;
  private string removeTeleCoords;
  private dynamic[] curCubeColors;
  private dynamic[] endRoundCubeColors;

  public void SetupScene(int level, int round)
  {
    dynamic[] cubeColors = null;
    if (GameManager.instance.type == "2D")
      cubeColors = cubeSprites;
    else
      cubeColors = cubeMaterials;

    cubes = new Dictionary<string, GameObject>();
    teles = new Dictionary<string, GameObject>();
    int[] curCubeColorsIdx = Config.LEVELS[level - 1, round - 1];
    curCubeColors = new dynamic[curCubeColorsIdx.Length];
    endRoundCubeColors = new dynamic[3];
    for (int i = 0; i < curCubeColors.Length; i++)
    {
      curCubeColors[i] = cubeColors[curCubeColorsIdx[i]];
    }
    if (curCubeColors.Length == 2)
    {
      endRoundCubeColors[0] = curCubeColors[0];
      endRoundCubeColors[1] = curCubeColors[1];
      endRoundCubeColors[2] = cubeColors[curCubeColorsIdx[1] + 1];
    }
    else
    {
      for (int i = 3; i > 0; i--)
      {
        endRoundCubeColors[3 - i] = curCubeColors[curCubeColors.Length - i];
      }
    }
    MapSetup();

    InitTeles(level);
  }

  void MapSetup()
  {
    mapHolder = new GameObject("Map").transform;
    foreach (Node node in map.nodeArray)
    {
      float x = 0, y = 0, z = 0;
      GameObject instance = null;
      if (GameManager.instance.type == "2D")
      {
        (x, y) = node.coords.get2DCoords("none");
        instance = Instantiate(cube, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
        instance.GetComponent<SpriteRenderer>().sprite = curCubeColors[0];
      }
      else
      {
        (x, y, z) = node.coords.get3DCoords("none");
        instance = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity) as GameObject;
        instance.GetComponent<Renderer>().material = curCubeColors[0];
      }

      cubes.Add(node.coords.getStringRep(), instance);
      instance.transform.SetParent(mapHolder);
    }
  }

  void InitTeles(int level)
  {
    for (int i = 0; i < level; i++)
    {
      Coords teleCoords = new Coords(0, 0);
      bool added = false;
      while (!added)
      {
        if (i % 2 == 0)
        {
          int r = Random.Range(-1, Config.MAP_SIZE - 1);
          int q = 1;
          teleCoords = new Coords(q, r);
        }
        else
        {
          int q = Random.Range(-Config.MAP_SIZE + 1, 1);
          int r = -1 - q;
          teleCoords = new Coords(q, r);
        }
        if (!teles.ContainsKey(teleCoords.getStringRep()))
          added = true;
      }
      float x = 0, y = 0, z = 0;
      if (GameManager.instance.type == "2D")
        (x, y) = teleCoords.get2DCoords("none");
      else
        (x, y, z) = teleCoords.get3DCoords("none");
      GameObject instance = Instantiate(tele, new Vector3(x, y, z), Quaternion.identity) as GameObject;
      teles.Add(teleCoords.getStringRep(), instance);

      instance.transform.SetParent(mapHolder);
    }
  }

  public bool AttemptTele(string stringRep)
  {
    if (!teles.ContainsKey(stringRep)) { return false; }
    SoundManager.instance.PlaySingle(teleSound);
    removeTeleCoords = stringRep;
    Invoke("Tele", Config.INIT_TELE_DELAY);

    return true;
  }

  void Tele()
  {
    StartCoroutine(TeleAnimation());
  }

  IEnumerator TeleAnimation()
  {
    if (GameManager.instance.type == "2D")
      for (int i = 0; i < teleDisappearSprites.Length; i++)
      {
        teles[removeTeleCoords].GetComponent<SpriteRenderer>().sprite = teleDisappearSprites[i];
        yield return new WaitForSeconds(Config.ANIMATION_DELAY);
      }
    Destroy(teles[removeTeleCoords]);
    teles.Remove(removeTeleCoords);
  }

  public bool NextCube(string stringRep)
  {
    if (!cubes.ContainsKey(stringRep)) { return false; }
    dynamic color = null;
    if (GameManager.instance.type == "2D")
      color = cubes[stringRep].GetComponent<SpriteRenderer>().sprite;
    else
      color = cubes[stringRep].GetComponent<Renderer>().sharedMaterial;

    if (color != curCubeColors[curCubeColors.Length - 1])
    {
      SoundManager.instance.RandomizeSfx(nextCubeSound1, nextCubeSound2, nextCubeSound3);
      if (GameManager.instance.type == "2D")
        cubes[stringRep].GetComponent<SpriteRenderer>().sprite = curCubeColors[Array.IndexOf(curCubeColors, color) + 1];
      else
        cubes[stringRep].GetComponent<Renderer>().material = curCubeColors[Array.IndexOf(curCubeColors, color) + 1];

      cubesChanged += 1;
      if (cubesChanged == (curCubeColors.Length - 1) * Config.NUMBER_OF_CUBES)
      {
        cubesChanged = 0;
        StartCoroutine(Restart());
      }
    }
    return true;
  }

  public void PrevCube(string stringRep)
  {
    if (!cubes.ContainsKey(stringRep)) { return; }
    dynamic color = null;
    if (GameManager.instance.type == "2D")
      color = cubes[stringRep].GetComponent<SpriteRenderer>().sprite;
    else
      color = cubes[stringRep].GetComponent<Renderer>().sharedMaterial;

    if (color != curCubeColors[0])
    {
      SoundManager.instance.PlaySingle(prevCubeSound);
      if (GameManager.instance.type == "2D")
        cubes[stringRep].GetComponent<SpriteRenderer>().sprite = curCubeColors[Array.IndexOf(curCubeColors, color) - 1];
      else
        cubes[stringRep].GetComponent<Renderer>().material = curCubeColors[Array.IndexOf(curCubeColors, color) - 1];
      cubesChanged -= 1;
    }
  }

  IEnumerator Restart()
  {
    GameManager.instance.EndRound();
    for (int i = 0; i < Config.END_ROUND_COLOR_CHANGING_REPEATS; i++)
    {
      for (int j = 0; j < 3; j++)
      {
        foreach (KeyValuePair<string, GameObject> cube in cubes)
        {
          if (GameManager.instance.type == "2D")
            cube.Value.GetComponent<SpriteRenderer>().sprite = endRoundCubeColors[j];
          else
            cube.Value.GetComponent<Renderer>().material = endRoundCubeColors[j];

        }
        yield return new WaitForSeconds(Config.ANIMATION_DELAY);
      }
    }
    loaded = true;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
  }

  public Node getInitNode() { return map.nodeArray[0]; }

  public Node getLeftNode() { return map.nodeArray[1]; }

  public Node getRightNode() { return map.nodeArray[2]; }

  public Node getBottomLeftNode() { return map.nodeArray[Config.NUMBER_OF_CUBES - Config.MAP_SIZE]; }

  public Node getBottomRightNode() { return map.nodeArray[Config.NUMBER_OF_CUBES - 1]; }

  public dynamic getAlterToCubeColor()
  {
    if (GameManager.instance.type == "2D")
      return curCubeColors[curCubeColors.Length - 1];
    else
    {
      Material m = new Material(curCubeColors[curCubeColors.Length - 1]);
      m.shader = Shader.Find("TextMeshPro/Sprite");
      return m;
    }
  }
}

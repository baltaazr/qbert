using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
  public static float teleDelay = 0.5f;

  public GameObject cube;
  public GameObject tele;
  public Sprite[] cubeSprites;
  public Sprite[] teleDisappearSprites;

  private Map map = new Map();
  private Transform mapHolder;
  private Dictionary<string, GameObject> cubes;
  private Dictionary<string, GameObject> teles;
  private Sprite[] curCubeColors;
  private int cubesChanged = 0;
  private string removeTeleCoords;


  public void SetupScene(int level, int round)
  {
    cubes = new Dictionary<string, GameObject>();
    teles = new Dictionary<string, GameObject>();
    int[] curCubeColorsIdx = Config.LEVELS[level - 1, round - 1];
    curCubeColors = new Sprite[curCubeColorsIdx.Length];
    for (int i = 0; i < curCubeColorsIdx.Length; i++)
    {
      curCubeColors[i] = cubeSprites[curCubeColorsIdx[i]];
    }
    MapSetup();

    InitTeles(level);
  }

  void MapSetup()
  {
    mapHolder = new GameObject("Map").transform;
    foreach (Node node in map.nodeArray)
    {
      (float x, float y) = node.coords.getAbsoluteCoords("none");
      GameObject instance = Instantiate(cube, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
      instance.GetComponent<SpriteRenderer>().sprite = curCubeColors[0];
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
        {
          added = true;
        }
      }
      (float x, float y) = teleCoords.getAbsoluteCoords("none");
      GameObject instance = Instantiate(tele, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
      teles.Add(teleCoords.getStringRep(), instance);

      instance.transform.SetParent(mapHolder);
    }
  }

  public bool attemptTele(string stringRep)
  {
    if (!teles.ContainsKey(stringRep)) { return false; }
    removeTeleCoords = stringRep;
    Invoke("Tele", teleDelay);
    return true;
  }

  void Tele()
  {
    StartCoroutine(TeleAnimation());
  }

  IEnumerator TeleAnimation()
  {
    for (int i = 0; i < teleDisappearSprites.Length; i++)
    {
      teles[removeTeleCoords].GetComponent<SpriteRenderer>().sprite = teleDisappearSprites[i];
      yield return new WaitForSeconds(Config.ANIMATION_DELAY);
    }
    Destroy(teles[removeTeleCoords]);
    teles.Remove(removeTeleCoords);
  }

  public void nextCube(string stringRep)
  {
    if (!cubes.ContainsKey(stringRep)) { return; }
    Sprite sprite = cubes[stringRep].GetComponent<SpriteRenderer>().sprite;
    if (sprite != curCubeColors[curCubeColors.Length - 1])
    {
      cubes[stringRep].GetComponent<SpriteRenderer>().sprite = curCubeColors[Array.IndexOf(curCubeColors, sprite) + 1];
      cubesChanged += 1;
      if (cubesChanged == (curCubeColors.Length - 1) * Config.NUMBER_OF_CUBES)
      {
        cubesChanged = 0;
        Restart();
      }
    }
  }

  public void prevCube(string stringRep)
  {
    if (!cubes.ContainsKey(stringRep)) { return; }
    Sprite sprite = cubes[stringRep].GetComponent<SpriteRenderer>().sprite;
    if (sprite != curCubeColors[0])
    {
      cubes[stringRep].GetComponent<SpriteRenderer>().sprite = curCubeColors[Array.IndexOf(curCubeColors, sprite) - 1];
      cubesChanged -= 1;
    }
  }

  void Restart()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
  }

  public Node getInitNode() { return map.nodeArray[0]; }

  public Node getLeftNode() { return map.nodeArray[1]; }

  public Node getRightNode() { return map.nodeArray[2]; }

  public Node getBottomLeftNode() { return map.nodeArray[Config.NUMBER_OF_CUBES - Config.MAP_SIZE]; }

  public Node getBottomRightNode() { return map.nodeArray[Config.NUMBER_OF_CUBES - 1]; }

  public Sprite getAlterToCubeSprite() { return curCubeColors[curCubeColors.Length - 1]; }
}

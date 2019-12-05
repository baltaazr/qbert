using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Coily : Enemy
{
  // 2D
  public Sprite[] eggToSnake;
  public Sprite frontViewSnake;
  public Sprite backViewSnake;
  public Sprite hopDownSnake;
  public Sprite jumpUpSnake;

  // 3D
  public GameObject snake;

  private bool eggForm;
  private bool transition;

  protected override void Start()
  {
    base.dir = "down";
    base.coordType = "ball";
    base.Start();

    eggForm = true;
    transition = false;
  }

  public override void MoveEnemy()
  {
    if (transition || base.falling) return;
    if (eggForm && base.node.coords.r != Config.MAP_SIZE - 1)
    {
      base.MoveEnemy();
      return;
    }
    else if (eggForm)
    {
      StartCoroutine(ChangeToSnake());
      return;
    }
    bool fall = false;

    Coords playerCoords = base.player.getNodeCoords();

    // GREEDY SEARCH
    string[] s = Config.HEX_SURROUNDINGS;
    IEnumerator enumerator = base.node.sur.Keys.GetEnumerator();
    enumerator.MoveNext();
    string curDir = (string)enumerator.Current;
    double curDist = base.node.sur[curDir].coords.dist(playerCoords);
    for (int i = 0; i < s.Length; i++)
    {
      if (base.node.sur.ContainsKey(s[i]))
      {
        double tempDist = base.node.sur[s[i]].coords.dist(playerCoords);
        if (tempDist < curDist || (tempDist == curDist && fall))
        {
          fall = false;
          curDir = s[i];
          curDist = tempDist;
        }
      }
      else
      {
        Coords offset = Config.HEX_SURROUNDINGS_OFFSET[s[i]];
        double tempDist = new Coords(base.node.coords.q + offset.q, base.node.coords.r + offset.r).dist(playerCoords);
        if (tempDist < curDist)
        {
          fall = true;
          curDir = s[i];
          curDist = tempDist;
        }
      }
    }
    if (fall)
    {
      Coords offset = Config.HEX_SURROUNDINGS_OFFSET[curDir];
      base.node = new Node(new Coords(base.node.coords.q + offset.q, base.node.coords.r + offset.r));
      GameManager.instance.DecreaseCoilyCount();
      StartCoroutine(base.Fall(0, -10, 0, curDir == "topLeft" || curDir == "topRight"));
    }
    else
      base.node = base.node.sur[curDir];

    switch (curDir)
    {
      case "topLeft":
        base.interSprite = jumpUpSnake;
        base.endSprite = backViewSnake;
        transform.rotation = GameManager.instance.type == "2D" ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        break;
      case "topRight":
        base.interSprite = jumpUpSnake;
        base.endSprite = backViewSnake;
        transform.rotation = GameManager.instance.type == "2D" ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
        break;
      case "botLeft":
        base.interSprite = hopDownSnake;
        base.endSprite = frontViewSnake;
        transform.rotation = GameManager.instance.type == "2D" ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 270, 0);
        break;
      case "botRight":
        base.interSprite = hopDownSnake;
        base.endSprite = frontViewSnake;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        break;
    }

    float newX = 0, newY = 0, newZ = 0;
    if (GameManager.instance.type == "2D")
      (newX, newY) = base.node.coords.get2DCoords(base.coordType);
    else
      (newX, newY, newZ) = base.node.coords.get3DCoords(base.coordType);

    if (GameManager.instance.type == "2D")
      base.Move(newX, newY, newZ);
    else
      base.Jump(newX, newY, newZ, base.dir);
  }

  IEnumerator ChangeToSnake()
  {
    transition = true;
    base.coordType = "player";
    if (GameManager.instance.type == "2D")
    {
      for (int i = 0; i < eggToSnake.Length; i++)
      {
        base.spriteRenderer.sprite = eggToSnake[i];
        yield return new WaitForSeconds(Config.ANIMATION_DELAY);
      }
    }
    else
    {
      foreach (Transform child in transform)
        Destroy(child.gameObject);
      Instantiate(snake, transform);
      yield return new WaitForSeconds(Config.ANIMATION_DELAY * 5);
    }

    transition = false;
    eggForm = false;
  }
}

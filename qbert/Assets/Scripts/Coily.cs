using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Coily : Enemy
{
  public Sprite[] eggToSnake;
  public Sprite frontViewSnake;
  public Sprite backViewSnake;
  public Sprite hopDownSnake;
  public Sprite jumpUpSnake;

  private Player player;
  private bool eggForm;
  private bool transition;

  protected override void Start()
  {
    base.dir = "down";
    base.coordType = "player";
    base.Start();

    eggForm = true;
    transition = false;

    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
    (float x, float y) = base.node.coords.getAbsoluteCoords(base.coordType);
    Sprite interSprite = hopDownSnake;
    Sprite endSprite = frontViewSnake;
    bool flip = false;
    bool fall = false;

    Coords playerCoords = player.getNodeCoords();
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
      StartCoroutine(base.Fall(0, -10, curDir == "topLeft" || curDir == "topRight"));
    }
    else
    {
      base.node = base.node.sur[curDir];
    }
    switch (curDir)
    {
      case "topLeft":
        flip = true;
        interSprite = jumpUpSnake;
        endSprite = backViewSnake;
        break;
      case "topRight":
        interSprite = jumpUpSnake;
        endSprite = backViewSnake;
        break;
      case "botLeft":
        interSprite = hopDownSnake;
        endSprite = frontViewSnake;
        break;
      case "botRight":
        flip = true;
        interSprite = hopDownSnake;
        endSprite = frontViewSnake;
        break;
    }
    (float newX, float newY) = base.node.coords.getAbsoluteCoords(base.coordType);
    if (flip)
      transform.rotation = Quaternion.Euler(0, 180, 0);
    else
      transform.rotation = Quaternion.Euler(0, 0, 0);
    base.interSprite = interSprite;
    base.endSprite = endSprite;
    base.Move(newX - x, newY - y);

  }

  IEnumerator ChangeToSnake()
  {
    transition = true;
    for (int i = 0; i < eggToSnake.Length; i++)
    {
      base.spriteRenderer.sprite = eggToSnake[i];
      yield return new WaitForSeconds(Config.ANIMATION_DELAY);
    }
    transition = false;
    eggForm = false;
  }
}

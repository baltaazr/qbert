using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Coily : Enemy
{
  public Sprite egg;
  public Sprite eggHop;
  public Sprite[] eggToSnake;
  public Sprite frontView;
  public Sprite backView;
  public Sprite hopDown;
  public Sprite jumpUp;

  protected override Node node { get; set; }
  protected override Player player { get; set; }

  private bool eggForm;
  private bool transition;

  protected override void Start()
  {
    base.Start();
    (float x, float y) = node.coords.getAbsoluteCoords("player");
    transform.position = new Vector2(x, y);

    eggForm = true;
    transition = false;

    base.interSprite = eggHop;
    base.endSprite = egg;
  }

  public override void MoveEnemy()
  {
    if (transition || base.falling) return;
    if (eggForm && node.coords.r != Config.MAP_SIZE - 1)
    {
      base.MoveEnemy();
      return;
    }
    else if (eggForm)
    {
      StartCoroutine(ChangeToSnake());
      return;
    }
    (float x, float y) = node.coords.getAbsoluteCoords("player");
    Sprite interSprite = hopDown;
    Sprite endSprite = frontView;
    bool flip = false;

    Coords playerCoords = player.getNodeCoords();
    // GREEDY SEARCH
    string[] s = Config.HEX_SURROUNDINGS;
    IEnumerator enumerator = node.sur.Keys.GetEnumerator();
    enumerator.MoveNext();
    string curDir = (string)enumerator.Current;
    double curDist = node.sur[curDir].coords.dist(playerCoords);
    for (int i = 0; i < s.Length; i++)
    {
      if (node.sur.ContainsKey(s[i]))
      {
        double tempDist = node.sur[s[i]].coords.dist(playerCoords);
        if (tempDist < curDist)
        {
          curDir = s[i];
          curDist = tempDist;
        }
      }
      else
      {
        Coords offset = Config.HEX_SURROUNDINGS_OFFSET[s[i]];
        double tempDist = new Coords(node.coords.q + offset.q, node.coords.r + offset.r).dist(playerCoords);
        if (tempDist < curDist)
        {
          curDir = s[i];
          curDist = tempDist;
        }
      }
    }
    if (!node.sur.ContainsKey(curDir))
    {
      Coords offset = Config.HEX_SURROUNDINGS_OFFSET[curDir];
      node = new Node(new Coords(node.coords.q + offset.q, node.coords.r + offset.r));
      GameManager.instance.DecreaseCoilyCount();
      StartCoroutine(base.Fall(0, -10, true));
    }
    else
    {
      node = node.sur[curDir];
    }
    switch (curDir)
    {
      case "topLeft":
        flip = true;
        interSprite = jumpUp;
        endSprite = backView;
        break;
      case "topRight":
        interSprite = jumpUp;
        endSprite = backView;
        break;
      case "botLeft":
        interSprite = hopDown;
        endSprite = frontView;
        break;
      case "botRight":
        flip = true;
        interSprite = hopDown;
        endSprite = frontView;
        break;
      default:
        break;
    }
    (float newX, float newY) = node.coords.getAbsoluteCoords("player");
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

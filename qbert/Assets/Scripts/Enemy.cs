using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public abstract class Enemy : MovingObject
{
  public Sprite frontView;
  public Sprite hopDown;

  protected Node node { get; set; }
  protected bool falling { get; set; }
  protected string dir { get; set; }
  protected string coordType { get; set; }

  protected override void Start()
  {
    base.Start();
    GameManager.instance.AddEnemyToList(this);

    switch (dir)
    {
      case "down":
        if ((int)Math.Round(Random.value) == 1)
          node = GameManager.instance.mapScript.getRightNode();
        else
          node = GameManager.instance.mapScript.getLeftNode();
        transform.position = new Vector2(0, 5);
        break;
      case "left":
        node = GameManager.instance.mapScript.getBottomRightNode();
        transform.position = new Vector2(5, -5);
        break;
      case "right":
        node = GameManager.instance.mapScript.getBottomLeftNode();
        transform.position = new Vector2(-5, -5);
        break;
    }

    base.interSprite = hopDown;
    base.endSprite = frontView;

    (float x, float y) = node.coords.getAbsoluteCoords(coordType);
    base.Move(x - transform.position.x, y - transform.position.y);
    falling = true;
    Invoke("EndFalling", Config.ENEMY_FALLING_TIME);
  }

  protected virtual void EndFalling()
  {
    falling = false;
  }

  public virtual void MoveEnemy()
  {
    if (falling) { return; }
    (float x, float y) = node.coords.getAbsoluteCoords(coordType);
    bool fall = false;
    string randDir = "";
    int randInt = (int)Math.Round(Random.value);
    bool flip = randInt == 1;
    switch (dir)
    {
      case "down":
        fall = node.coords.r == Config.MAP_SIZE - 1;
        randDir = randInt == 0 ? "botRight" : "botLeft";
        transform.rotation = flip ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        break;
      case "left":
        fall = node.coords.q + node.coords.r == 0;
        randDir = randInt == 0 ? "topLeft" : "left";
        transform.rotation = flip ? Quaternion.Euler(180, 0, 90) : Quaternion.Euler(0, 0, 0);
        break;
      case "right":
        fall = node.coords.q == 0;
        randDir = randInt == 0 ? "topRight" : "right";
        transform.rotation = flip ? Quaternion.Euler(0, 180, 90) : Quaternion.Euler(0, 0, 0);
        break;
    }
    if (fall)
    {
      Coords offset = Config.HEX_SURROUNDINGS_OFFSET[randDir];
      node = new Node(new Coords(node.coords.q + offset.q, node.coords.r + offset.r));
      int fallX = dir == "down" ? 0 : dir == "left" ? -5 : 5;
      int fallY = dir == "down" ? -5 : 5;
      StartCoroutine(Fall(fallX, fallY, false));
    }
    else
    {
      node = node.sur[randDir];
    }
    (float newX, float newY) = node.coords.getAbsoluteCoords(coordType);
    base.Move(newX - x, newY - y);
  }

  protected IEnumerator Fall(float dirX, float dirY, bool backwards)
  {
    falling = true;
    if (backwards)
      base.spriteRenderer.sortingLayerName = "Default";
    yield return new WaitForSeconds(Config.MOVE_TIME);
    base.Move(dirX, dirY);
    yield return new WaitForSeconds(Config.ENEMY_FALLING_TIME);
    GameManager.instance.RemoveEnemyFromList(this);
    Destroy(gameObject);
  }

  public void DestroyEnemy()
  {
    Destroy(gameObject);
  }
}

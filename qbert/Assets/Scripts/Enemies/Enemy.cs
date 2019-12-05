using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public abstract class Enemy : MovingObject
{
  public Sprite frontView;
  public Sprite hopDown;

  protected Player player { get; set; }
  protected Node node { get; set; }
  protected bool falling { get; set; }
  protected string dir { get; set; }
  protected string coordType { get; set; }

  protected override void Start()
  {
    base.Start();
    GameManager.instance.AddEnemyToList(this);
    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    float initX = 0, initY = 0, initZ = 0;
    switch (dir)
    {
      case "down":
        int randInt = (int)Math.Round(Random.value);
        if (randInt == 1)
          node = GameManager.instance.mapScript.getRightNode();
        else
          node = GameManager.instance.mapScript.getLeftNode();

        if (GameManager.instance.type == "2D")
          (initX, initY) = node.coords.get2DCoords(coordType);
        else
        {
          (initX, initY, initZ) = node.coords.get3DCoords(coordType);
          transform.rotation = randInt == 1 ? Quaternion.Euler(0, 270, 0) : Quaternion.Euler(0, 180, 0);
        }
        transform.position = new Vector3(initX, 5, initZ);
        break;
      case "left":
        node = GameManager.instance.mapScript.getBottomRightNode();
        if (GameManager.instance.type == "2D")
          transform.position = new Vector3(5, -5, 0);
        else
        {
          (initX, initY, initZ) = node.coords.get3DCoords(coordType);
          transform.position = new Vector3(initX, initY, initZ - 5);
          transform.rotation = Quaternion.Euler(0, 270, 90);
        }
        break;
      case "right":
        node = GameManager.instance.mapScript.getBottomLeftNode();
        if (GameManager.instance.type == "2D")
          transform.position = new Vector3(-5, -5, 0);
        else
        {
          (initX, initY, initZ) = node.coords.get3DCoords(coordType);
          transform.position = new Vector3(initX - 5, initY, initZ);
          transform.rotation = Quaternion.Euler(180, 0, 90);
        }
        break;
    }

    float x = 0, y = 0, z = 0;

    if (GameManager.instance.type == "2D")
    {
      base.interSprite = hopDown;
      base.endSprite = frontView;
      (x, y) = node.coords.get2DCoords(coordType);
    }
    else
      (x, y, z) = node.coords.get3DCoords(coordType);

    base.Move(x, y, z);
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
    bool fall = false;
    string randDir = "";
    int randInt = (int)Math.Round(Random.value);
    bool flip = randInt == 1;
    switch (dir)
    {
      case "down":
        fall = node.coords.r == Config.MAP_SIZE - 1;
        randDir = randInt == 0 ? "botRight" : "botLeft";
        if (GameManager.instance.type == "2D")
          transform.rotation = flip ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        else
          transform.rotation = flip ? Quaternion.Euler(0, 270, 0) : Quaternion.Euler(0, 180, 0);
        break;
      case "left":
        fall = node.coords.q + node.coords.r == 0;
        randDir = randInt == 0 ? "topLeft" : "left";
        if (GameManager.instance.type == "2D")
          transform.rotation = flip ? Quaternion.Euler(180, 0, 90) : Quaternion.Euler(0, 0, 0);
        else
          transform.rotation = flip ? Quaternion.Euler(0, 270, 90) : Quaternion.Euler(270, 270, 90);
        break;
      case "right":
        fall = node.coords.q == 0;
        randDir = randInt == 0 ? "topRight" : "right";
        if (GameManager.instance.type == "2D")
          transform.rotation = flip ? Quaternion.Euler(0, 180, 90) : Quaternion.Euler(0, 0, 0);
        else
          transform.rotation = flip ? Quaternion.Euler(180, 0, 90) : Quaternion.Euler(270, 0, 90);
        break;
    }
    if (fall)
    {
      Coords offset = Config.HEX_SURROUNDINGS_OFFSET[randDir];
      node = new Node(new Coords(node.coords.q + offset.q, node.coords.r + offset.r));

      int fallX = 0, fallY = 0, fallZ = 0;
      if (GameManager.instance.type == "2D")
      {
        fallX = dir == "down" ? 0 : dir == "left" ? -5 : 5;
        fallY = dir == "down" ? -5 : 5;
        fallZ = dir == "down" ? 0 : dir == "left" ? 5 : -5;
      }
      else
      {
        fallX = dir == "right" ? 5 : 0;
        fallY = dir == "down" ? -5 : 0;
        fallZ = dir == "left" ? 5 : 0;
      }

      StartCoroutine(Fall(fallX, fallY, fallZ, false));
    }
    else
    {
      node = node.sur[randDir];
    }
    float newX = 0, newY = 0, newZ = 0;
    if (GameManager.instance.type == "2D")
      (newX, newY) = node.coords.get2DCoords(coordType);
    else
      (newX, newY, newZ) = node.coords.get3DCoords(coordType);
    if (GameManager.instance.type == "2D")
      base.Move(newX, newY, newZ);
    else
      base.Jump(newX, newY, newZ, dir);
  }

  protected IEnumerator Fall(float xDir, float yDir, float zDir, bool backwards)
  {
    falling = true;
    if (GameManager.instance.type == "2D" && backwards)
    {
      base.spriteRenderer.sortingLayerName = "Default";
      yield return new WaitForSeconds(Config.MOVE_TIME);
    }
    else
    {
      yield return new WaitForSeconds(Config.JUMP_TIME);
    }

    base.Move(xDir + transform.position.x, yDir + transform.position.y, zDir + transform.position.z);
    yield return new WaitForSeconds(Config.ENEMY_FALLING_TIME);
    GameManager.instance.RemoveEnemyFromList(this);
    Destroy(gameObject);
  }

  public void DestroyEnemy()
  {
    Destroy(gameObject);
  }
}

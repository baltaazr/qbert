using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MovingObject
{
  public float deathDelay = 0.1f;

  protected abstract Node node { get; set; }
  protected abstract Player player { get; set; }
  protected bool falling { get; set; }

  protected override void Start()
  {
    base.Start();
    GameManager.instance.AddEnemyToList(this);

    if (Random.Range(0, 2) == 1)
      node = GameManager.instance.mapScript.getRightNode();
    else
      node = GameManager.instance.mapScript.getLeftNode();

    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
  }

  public virtual void MoveEnemy()
  {
    if (falling) { return; }
    (float x, float y) = node.coords.getAbsoluteCoords("ball");
    if (node.coords.r == Config.MAP_SIZE - 1)
    {
      Coords offset = Config.HEX_SURROUNDINGS_OFFSET["botLeft"];
      node = new Node(new Coords(node.coords.q + offset.q, node.coords.r + offset.r));
      StartCoroutine(Fall(0, -10, false));
    }
    else
    {
      if (Random.Range(0, 2) == 1)
        node = node.sur["botRight"];
      else
        node = node.sur["botLeft"];
    }
    (float newX, float newY) = node.coords.getAbsoluteCoords("ball");
    base.Move(newX - x, newY - y);
  }

  protected IEnumerator Fall(int dirX, int dirY, bool backwards)
  {
    falling = true;
    if (backwards)
      base.spriteRenderer.sortingLayerName = "Default";
    yield return new WaitForSeconds(deathDelay);
    base.Move(dirX, dirY);
    yield return new WaitForSeconds(deathDelay);
    GameManager.instance.RemoveEnemyFromList(this);
    Destroy(gameObject);
  }
}

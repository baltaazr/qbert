using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlickSam : Enemy
{

  protected override void Start()
  {
    base.dir = "down";
    base.coordType = "player";
    base.Start();
  }

  protected override void EndFalling()
  {
    base.EndFalling();
    GameManager.instance.mapScript.prevCube(base.node.coords.getStringRep());
  }

  public override void MoveEnemy()
  {
    if (base.falling) { return; }
    base.MoveEnemy();
    GameManager.instance.mapScript.prevCube(base.node.coords.getStringRep());
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
    {
      GameManager.instance.RemoveEnemyFromList(this);
      Destroy(gameObject);
    }
  }
}

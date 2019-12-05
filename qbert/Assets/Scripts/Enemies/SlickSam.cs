using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlickSam : Enemy
{

  protected override void Start()
  {
    base.dir = "down";
    if (GameManager.instance.type == "2D")
      base.coordType = "player";
    else
      base.coordType = "ball";
    base.Start();
  }

  protected override void EndFalling()
  {
    base.EndFalling();
    GameManager.instance.mapScript.PrevCube(base.node.coords.getStringRep());
  }

  public override void MoveEnemy()
  {
    if (base.falling) { return; }
    base.MoveEnemy();
    GameManager.instance.mapScript.PrevCube(base.node.coords.getStringRep());
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
      RemoveEnemy();
  }

  public void RemoveEnemy()
  {
    GameManager.instance.RemoveEnemyFromList(this);
    base.DestroyEnemy();
  }
}

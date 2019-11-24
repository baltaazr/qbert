using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlickSam : Enemy
{
  public Sprite frontView;
  public Sprite hopDown;

  protected override Node node { get; set; }
  protected override Player player { get; set; }

  protected override void Start()
  {
    base.Start();
    (float x, float y) = node.coords.getAbsoluteCoords("player");
    transform.position = new Vector2(x, y);

    base.interSprite = hopDown;
    base.endSprite = frontView;
  }

  public override void MoveEnemy()
  {
    base.MoveEnemy();
    GameManager.instance.mapScript.prevCube(node.coords.getStringRep());
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player" && !base.falling)
    {
      GameManager.instance.RemoveEnemyFromList(this);
      Destroy(gameObject);
    }
  }
}

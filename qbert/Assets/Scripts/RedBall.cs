using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class RedBall : Enemy
{
  public Sprite redBall;

  protected override Node node { get; set; }
  protected override Player player { get; set; }

  protected override void Start()
  {
    base.Start();
    (float x, float y) = node.coords.getAbsoluteCoords("ball");
    transform.position = new Vector2(x, y);

    base.interSprite = redBall;
    base.endSprite = redBall;
  }
}

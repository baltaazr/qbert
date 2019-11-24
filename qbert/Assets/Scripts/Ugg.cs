using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ugg : Enemy
{
  public Sprite frontView;
  public Sprite hopDown;

  protected override Node node { get; set; }
  protected override Player player { get; set; }

  protected override void Start()
  {
    base.Start();
    node = GameManager.instance.mapScript.getBottomRightNode();
    (float x, float y) = node.coords.getAbsoluteCoords("ugg");
    transform.position = new Vector2(x, y);

    base.interSprite = hopDown;
    base.endSprite = frontView;
  }

  public override void MoveEnemy()
  {
    if (base.falling) { return; }
    (float x, float y) = node.coords.getAbsoluteCoords("ugg");
    bool flip = false;

    if (node.coords.q + node.coords.r == 0)
    {
      Coords offset = Config.HEX_SURROUNDINGS_OFFSET["left"];
      node = new Node(new Coords(node.coords.q + offset.q, node.coords.r + offset.r));
      StartCoroutine(base.Fall(-10, 0, false));
    }
    else
    {
      if (Random.Range(0, 2) == 1)
        node = node.sur["topLeft"];
      else
      {
        node = node.sur["left"];
        flip = true;
      }
    }
    (float newX, float newY) = node.coords.getAbsoluteCoords("ugg");
    if (flip)
      transform.rotation = Quaternion.Euler(180, 0, 90);
    else
      transform.rotation = Quaternion.Euler(0, 0, 0);
    base.Move(newX - x, newY - y);
  }
}

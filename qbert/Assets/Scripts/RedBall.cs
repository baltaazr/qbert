using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class RedBall : Enemy
{
  protected override void Start()
  {
    base.dir = "down";
    base.coordType = "ball";
    base.Start();
  }
}

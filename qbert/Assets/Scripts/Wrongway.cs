using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrongway : Enemy
{
  protected override void Start()
  {
    base.dir = "right";
    base.coordType = "wrongway";
    base.Start();
  }
}

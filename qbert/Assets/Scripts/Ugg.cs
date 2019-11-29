using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ugg : Enemy
{
  protected override void Start()
  {
    base.dir = "left";
    base.coordType = "ugg";
    base.Start();
  }
}

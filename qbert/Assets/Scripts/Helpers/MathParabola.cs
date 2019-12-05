using UnityEngine;
using System;

public class MathParabola
{

  public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t, string dir)
  {
    Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

    var mid = Vector3.Lerp(start, end, t);

    return new Vector3(dir == "right" ? Mathf.Lerp(start.x, end.x, t) - f(t) : mid.x, dir == "down" ? Mathf.Lerp(start.y, end.y, t) + f(t) : mid.y, dir == "left" ? Mathf.Lerp(start.z, end.z, t) - f(t) : mid.z);
  }

  public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
  {
    Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

    var mid = Vector2.Lerp(start, end, t);

    return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
  }

}
using System;
public class Coords
{
  public int q { get; set; }
  public int r { get; set; }
  public Coords(int qCoord, int rCoord)
  {
    q = qCoord;
    r = rCoord;
  }

  public (float, float) getAbsoluteCoords(string offsetType)
  {
    double[] offset;
    switch (offsetType)
    {
      case "player":
        offset = Config.PLAYER_OFFSET;
        break;
      case "ball":
        offset = Config.BALL_OFFSET;
        break;
      case "wrongway":
        offset = Config.WRONGWAY_OFFSET;
        break;
      case "ugg":
        offset = Config.UGG_OFFSET;
        break;
      default:
        offset = new double[2] { 0, 0 };
        break;
    }
    float x = (float)((0.85 / Math.Sqrt(3)) * (Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r));
    float y = -(float)(0.75 * r);
    x += (float)offset[0];
    y += (float)offset[1] + (float)Config.VERTICAL_OFFSET;
    return (x, y);
  }

  public string getStringRep()
  {
    return string.Format("{0},{1}", q, r);
  }

  public bool withinMap()
  {
    return q <= 0 && r < Config.MAP_SIZE && q + r >= 0;
  }

  public float dist(Coords o)
  {
    return (float)((Math.Abs(q - o.q)
          + Math.Abs(q + r - o.q - o.r)
          + Math.Abs(r - o.r)) / 2);
  }
}
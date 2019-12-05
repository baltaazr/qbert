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

  public (float, float) get2DCoords(string offsetType)
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

  public (float, float, float) get3DCoords(string offsetType)
  {
    double[] offset;
    switch (offsetType)
    {
      case "player":
        offset = Config.PLAYER_OFFSET_3D;
        break;
      case "ball":
        offset = Config.BALL_OFFSET_3D;
        break;
      case "wrongway":
        offset = Config.WRONGWAY_OFFSET_3D;
        break;
      case "ugg":
        offset = Config.UGG_OFFSET_3D;
        break;
      default:
        offset = new double[3] { 0, 0, 0 };
        break;
    }
    float x = q;
    float y = -r;
    float z = -q - r;
    x += (float)offset[0];
    y += (float)offset[1];
    z += (float)offset[2];
    return (x, y, z);
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

  public override bool Equals(object obj)
  {
    if ((obj == null) || !this.GetType().Equals(obj.GetType()))
      return false;
    else
    {
      Coords c = (Coords)obj;
      return (q == c.q) && (r == c.r);
    }
  }

  public override int GetHashCode() { return q + r; }
}
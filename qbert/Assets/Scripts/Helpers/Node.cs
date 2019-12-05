using System;
using System.Collections.Generic;

public class Node
{
  public Coords coords { get; set; }

  // keys = [topLeft, topRight, botLeft, botRight, left, right]
  public Dictionary<string, Node> sur { get; set; }
  public Node(Coords c)
  {
    coords = c;
    sur = new Dictionary<string, Node>();
  }
}
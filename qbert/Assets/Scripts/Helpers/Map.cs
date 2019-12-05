using System;

public class Map
{
  public Node[] nodeArray { get; set; }

  public Map()
  {
    int counter = 0;
    nodeArray = new Node[Config.NUMBER_OF_CUBES];
    for (int i = 0; i < Config.MAP_SIZE; i++)
    {
      for (int j = 0; j <= i; j++)
      {
        nodeArray[counter] = new Node(new Coords(j - i, i));
        counter += 1;
      }
    }
    counter = 0;
    for (int i = 1; i <= Config.MAP_SIZE; i++)
    {
      for (int j = 1; j <= i; j++)
      {
        if (j != 1)
        {
          nodeArray[counter].sur.Add("topLeft", nodeArray[counter - i]);
          nodeArray[counter].sur.Add("left", nodeArray[counter - 1]);
        }
        if (j != i)
        {
          nodeArray[counter].sur.Add("topRight", nodeArray[counter - i + 1]);
          nodeArray[counter].sur.Add("right", nodeArray[counter + 1]);
        }
        if (i != Config.MAP_SIZE)
        {
          nodeArray[counter].sur.Add("botLeft", nodeArray[counter + i]);
          nodeArray[counter].sur.Add("botRight", nodeArray[counter + i + 1]);
        }
        counter += 1;
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MovingObject
{
  public Sprite frontView;
  public Sprite backView;
  public Sprite hopDown;
  public Sprite jumpUp;
  public Sprite[] teleSprites;
  public Sprite[] reappearSprites;
  public Sprite[] disintegrateSprites;

  private Node node;
  private bool teleporting = false;
  private bool dying = false;

  protected override void Start()
  {
    base.Start();

    node = GameManager.instance.mapScript.getInitNode();
    if (GameManager.instance.type == "2D")
    {
      (float x, float y) = node.coords.get2DCoords("player");
      transform.position = new Vector2(x, y);
    }
  }

  void Update()
  {
    if (!GameManager.instance.playersTurn || GameManager.instance.doingSetup || teleporting || dying) return;
    if (GameManager.instance.type == "2D")
    {
      Sprite interSprite = jumpUp;
      Sprite endSprite = backView;
      bool move = false;
      bool flip = false;
      if (Input.GetKeyDown("q"))
      {
        flip = true;
        interSprite = jumpUp;
        endSprite = backView;
        move = true;
        if (node.sur.ContainsKey("topLeft"))
        {
          node = node.sur["topLeft"];
        }
        else
        {
          node = new Node(new Coords(node.coords.q, node.coords.r - 1));
          if (GameManager.instance.mapScript.AttemptTele(node.coords.getStringRep()))
          {
            Invoke("Tele", Config.INIT_TELE_DELAY);
          }
          else
          {
            base.spriteRenderer.sortingLayerName = "Default";
            Fall();
          }
        }

      }
      else if (Input.GetKeyDown("w"))
      {
        interSprite = jumpUp;
        endSprite = backView;
        move = true;
        if (node.sur.ContainsKey("topRight"))
        {
          node = node.sur["topRight"];
        }
        else
        {
          node = new Node(new Coords(node.coords.q + 1, node.coords.r - 1));
          if (GameManager.instance.mapScript.AttemptTele(node.coords.getStringRep()))
          {
            Invoke("Tele", Config.INIT_TELE_DELAY);
          }
          else
          {
            base.spriteRenderer.sortingLayerName = "Default";
            Fall();
          }
        }
      }
      else if (Input.GetKeyDown("a"))
      {
        interSprite = hopDown;
        endSprite = frontView;
        move = true;
        if (node.sur.ContainsKey("botLeft"))
        {
          node = node.sur["botLeft"];
        }
        else
        {
          node = new Node(new Coords(node.coords.q - 1, node.coords.r + 1));
          Fall();
        }
      }
      else if (Input.GetKeyDown("s"))
      {
        flip = true;
        interSprite = hopDown;
        endSprite = frontView;
        move = true;
        if (node.sur.ContainsKey("botRight"))
        {
          node = node.sur["botRight"];
        }
        else
        {
          node = new Node(new Coords(node.coords.q, node.coords.r + 1));
          Fall();
        }
      }
      if (move)
      {
        (float newX, float newY) = node.coords.get2DCoords("player");
        if (flip)
          transform.rotation = Quaternion.Euler(0, 180, 0);
        else
          transform.rotation = Quaternion.Euler(0, 0, 0);
        GameManager.instance.playersTurn = false;
        base.interSprite = interSprite;
        base.endSprite = endSprite;
        base.Move(newX, newY, 0);
        GameManager.instance.mapScript.NextCube(node.coords.getStringRep());
      }
    }
    else
    {
      if (transform.position.y <= -10)
        Fall();
      int q = (int)Math.Round(transform.position.x);
      int r = -(int)Math.Round(transform.position.y - 1.3f);
      Coords newCoords = new Coords(q, r);
      if ((int)Math.Round(transform.position.z) == -q - r && !node.coords.Equals(newCoords))
      {
        node = new Node(newCoords);
        if (!GameManager.instance.mapScript.NextCube(node.coords.getStringRep()))
          if (GameManager.instance.mapScript.AttemptTele(node.coords.getStringRep()))
            Invoke("Tele", Config.INIT_TELE_DELAY);
      }
    }

  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Enemy" && !dying && !teleporting)
      Disintegrate();
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Enemy" && !dying && !teleporting)
      Disintegrate();
    else if (other.tag == "SlickSam")
      other.GetComponent<SlickSam>().RemoveEnemy();
  }

  void Die()
  {
    GameManager.instance.LoseLife();
    dying = true;
  }

  void Fall()
  {
    Die();
    if (GameManager.instance.type == "2D")
      base.Move(0 + transform.position.x, -10 + transform.position.y, 0);
    StartCoroutine(ReappearAnimation());
  }

  public void Disintegrate()
  {
    if (dying || teleporting) { return; }
    Die();
    if (GameManager.instance.type == "2D")
      StartCoroutine(DisintegrateAnimation());
    else
      base.rb.velocity = new Vector3(0, 0, 0);
    StartCoroutine(ReappearAnimation());
  }

  void Tele()
  {
    teleporting = true;
    if (GameManager.instance.type == "2D")
      StartCoroutine(TeleAnimation());
    else
      base.rb.velocity = new Vector3(0, 100, 0);
    StartCoroutine(ReappearAnimation());
  }

  IEnumerator TeleAnimation()
  {
    if (GameManager.instance.type == "2D")
    {
      for (int i = 0; i < teleSprites.Length; i++)
      {
        base.spriteRenderer.sprite = teleSprites[i];
        yield return new WaitForSeconds(Config.ANIMATION_DELAY);
      }
      base.spriteRenderer.enabled = false;
    }
  }

  IEnumerator DisintegrateAnimation()
  {
    for (int i = 0; i < disintegrateSprites.Length; i++)
    {
      base.spriteRenderer.sprite = disintegrateSprites[i];
      yield return new WaitForSeconds(Config.ANIMATION_DELAY);
    }
    base.spriteRenderer.enabled = false;
  }

  IEnumerator ReappearAnimation()
  {
    yield return new WaitForSeconds(Config.PLAYER_REAPPEAR_DELAY);
    if (GameManager.instance.type == "2D")
    {
      base.spriteRenderer.sortingLayerName = "Units";
      base.spriteRenderer.enabled = true;

      node = GameManager.instance.mapScript.getInitNode();
      (float x, float y) = node.coords.get2DCoords("player");
      transform.position = new Vector2(x, y);
      for (int i = 0; i < reappearSprites.Length; i++)
      {
        base.spriteRenderer.sprite = reappearSprites[i];
        yield return new WaitForSeconds(Config.ANIMATION_DELAY);
      }
    }
    else
    {
      node = GameManager.instance.mapScript.getInitNode();
      transform.position = new Vector3(0, 5, 0);
      base.rb.velocity = new Vector3(0, 0, 0);
    }

    if (teleporting)
    {
      GameManager.instance.mapScript.NextCube(node.coords.getStringRep());
      teleporting = false;
    }
    else if (dying)
    {
      dying = false;
    }
  }

  public Coords getNodeCoords() { return node.coords; }
  public bool getTeleporting() { return teleporting; }
  public bool getDying() { return dying; }
}

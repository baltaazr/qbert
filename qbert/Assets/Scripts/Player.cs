using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
  public Sprite frontView;
  public Sprite backView;
  public Sprite hopDown;
  public Sprite jumpUp;
  public Sprite[] teleSprites;
  public Sprite[] reappearSprites;
  public Sprite[] disintegrateSprites;
  public AudioClip moveSound1;
  public AudioClip moveSound2;
  public AudioClip moveSound3;
  public AudioClip teleportSound;

  private Node node;
  private bool teleporting;
  private bool dying;

  protected override void Start()
  {
    base.Start();

    node = GameManager.instance.mapScript.getInitNode();
    (float x, float y) = node.coords.getAbsoluteCoords("player");
    transform.position = new Vector2(x, y);
  }

  void Update()
  {
    if (!GameManager.instance.playersTurn || GameManager.instance.doingSetup || teleporting || dying) return;
    (float x, float y) = node.coords.getAbsoluteCoords("player");
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
        if (GameManager.instance.mapScript.attemptTele(node.coords.getStringRep()))
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
        if (GameManager.instance.mapScript.attemptTele(node.coords.getStringRep()))
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
      if (!dying)
      {
        SoundManager.instance.RandomizeSfx(moveSound1, moveSound2, moveSound3);
      }
      (float newX, float newY) = node.coords.getAbsoluteCoords("player");
      if (flip)
        transform.rotation = Quaternion.Euler(0, 180, 0);
      else
        transform.rotation = Quaternion.Euler(0, 0, 0);
      GameManager.instance.playersTurn = false;
      base.interSprite = interSprite;
      base.endSprite = endSprite;
      base.Move(newX - x, newY - y);
      GameManager.instance.mapScript.nextCube(node.coords.getStringRep());
    }

  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Enemy" && !dying && !teleporting)
    {
      Disintegrate();
    }
  }

  void Fall()
  {
    GameManager.instance.LoseLife();
    dying = true;
    base.Move(0, -10);
    StartCoroutine(ReappearAnimation());
  }

  void Disintegrate()
  {
    GameManager.instance.LoseLife();
    dying = true;
    StartCoroutine(DisintegrateAnimation());
    StartCoroutine(ReappearAnimation());
  }

  void Tele()
  {
    SoundManager.instance.PlaySingle(teleportSound);
    teleporting = true;
    StartCoroutine(TeleAnimation());
  }

  IEnumerator TeleAnimation()
  {
    for (int i = 0; i < teleSprites.Length; i++)
    {
      base.spriteRenderer.sprite = teleSprites[i];
      yield return new WaitForSeconds(Config.ANIMATION_DELAY);
    }
    base.spriteRenderer.enabled = false;
    StartCoroutine(ReappearAnimation());
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
    base.spriteRenderer.sortingLayerName = "Units";
    base.spriteRenderer.enabled = true;

    node = GameManager.instance.mapScript.getInitNode();
    (float x, float y) = node.coords.getAbsoluteCoords("player");
    transform.position = new Vector2(x, y);
    for (int i = 0; i < reappearSprites.Length; i++)
    {
      base.spriteRenderer.sprite = reappearSprites[i];
      yield return new WaitForSeconds(Config.ANIMATION_DELAY);
    }
    if (teleporting)
    {
      GameManager.instance.mapScript.nextCube(node.coords.getStringRep());
      teleporting = false;
    }
    else if (dying)
    {
      dying = false;
    }
  }

  public Coords getNodeCoords() { return node.coords; }
}

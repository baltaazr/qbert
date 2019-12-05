using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
  private float inverseMoveTime;
  // 2D
  protected Sprite interSprite;
  protected Sprite endSprite;
  protected SpriteRenderer spriteRenderer;
  private Rigidbody2D rb2D;

  // 3D
  protected Rigidbody rb;

  protected virtual void Start()
  {
    if (GameManager.instance.type == "2D")
    {
      rb2D = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
    }
    else
      rb = GetComponent<Rigidbody>();

    inverseMoveTime = 1f / Config.MOVE_TIME;
  }

  protected void Move(float xEnd, float yEnd, float zEnd)
  {
    Vector3 end = new Vector3(xEnd, yEnd, zEnd);
    StartCoroutine(SmoothMovement(end));
  }

  protected void Jump(float xEnd, float yEnd, float zEnd, string dir)
  {
    Vector3 end = new Vector3(xEnd, yEnd, zEnd);
    StartCoroutine(ParabolaJump(end, dir));
  }

  IEnumerator SmoothMovement(Vector3 end)
  {
    if (GameManager.instance.type == "2D")
      changeSprite(interSprite);

    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    while (sqrRemainingDistance > float.Epsilon)
    {
      Vector3 newPosition = Vector3.MoveTowards(GameManager.instance.type == "2D" ? (Vector3)rb2D.position : rb.position, end, inverseMoveTime * Time.deltaTime);
      if (GameManager.instance.type == "2D")
        rb2D.MovePosition(newPosition);
      else
        rb.MovePosition(newPosition);
      sqrRemainingDistance = (transform.position - end).sqrMagnitude;
      yield return null;
    }
    if (GameManager.instance.type == "2D")
    {
      changeSprite(endSprite);
      GameManager.instance.playersTurn = true;
    }

  }

  IEnumerator ParabolaJump(Vector3 end, string dir)
  {
    float t = Time.deltaTime;
    while (t < Config.JUMP_TIME)
    {
      transform.position = MathParabola.Parabola(rb.position, end, Config.JUMP_HEIGHT, t / Config.JUMP_TIME, dir);
      t += Time.deltaTime;
      yield return null;
    }
    transform.position = end;
  }

  void changeSprite(Sprite newSprite)
  {
    spriteRenderer.sprite = newSprite;
  }
}

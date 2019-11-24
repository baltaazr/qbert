using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
  public float moveTime = 0.2f;
  public LayerMask blockingLayer;

  protected Sprite interSprite;
  protected Sprite endSprite;
  protected SpriteRenderer spriteRenderer;

  private BoxCollider2D boxCollider;
  private Rigidbody2D rb2D;
  private float inverseMoveTime;

  protected virtual void Start()
  {
    boxCollider = GetComponent<BoxCollider2D>();
    rb2D = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    inverseMoveTime = 1f / moveTime;
  }

  protected void Move(float xDir, float yDir)
  {
    Vector2 start = transform.position;
    Vector2 end = start + new Vector2(xDir, yDir);

    StartCoroutine(SmoothMovement(end));
  }

  IEnumerator SmoothMovement(Vector3 end)
  {
    changeSprite(interSprite);
    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

    while (sqrRemainingDistance > float.Epsilon)
    {
      Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
      rb2D.MovePosition(newPosition);
      sqrRemainingDistance = (transform.position - end).sqrMagnitude;
      yield return null;
    }
    changeSprite(endSprite);
    GameManager.instance.playersTurn = true;
  }

  void changeSprite(Sprite newSprite)
  {
    spriteRenderer.sprite = newSprite;
  }
}

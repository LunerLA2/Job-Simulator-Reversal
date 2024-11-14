using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreakoutBall : MonoBehaviour
{
	[SerializeField]
	private Vector2 startOffsetFromPaddle;

	[SerializeField]
	private Rigidbody2D rb;

	private Vector3 direction;

	[SerializeField]
	private float speed;

	[SerializeField]
	private float speedIncreaseOnBlockBreak;

	private float currentSpeed;

	[SerializeField]
	private GameObject leftSide;

	[SerializeField]
	private GameObject rightSide;

	[SerializeField]
	private GameObject topSide;

	[SerializeField]
	private GameObject bottomSide;

	[SerializeField]
	private float paddleBounceXDamping;

	[SerializeField]
	private float blockBounceXDamping;

	private GameObject prefabReference;

	private GameObject paddle;

	private BreakoutComputerProgram program;

	[SerializeField]
	private Image ballImage;

	[SerializeField]
	private Sprite[] ballSprites;

	[SerializeField]
	private float blinkCounterMin;

	[SerializeField]
	private float blinkCounterMax;

	private float blinkTick;

	private float blinkCounter;

	[SerializeField]
	private float blinkLength;

	private bool blinking;

	[SerializeField]
	private float showSurprisedFaceLength;

	private Coroutine showSurprisedFace;

	[SerializeField]
	private float blockForceMultiplierX;

	[SerializeField]
	private float blockForceMultiplierY;

	[SerializeField]
	private float blockRotationMultiplier;

	[SerializeField]
	private float gravityScaleToSet;

	public Vector2 StartOffsetFromPaddle
	{
		get
		{
			return startOffsetFromPaddle;
		}
	}

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
	}

	private void Awake()
	{
		currentSpeed = speed;
		if (ballSprites[0] != null)
		{
			ballImage.sprite = ballSprites[0];
		}
	}

	private void Update()
	{
		if (direction != Vector3.zero)
		{
			base.transform.position += direction * currentSpeed * Time.deltaTime;
		}
		CheckForOOB();
		BallAnimations();
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject == leftSide || other.gameObject == rightSide)
		{
			FlipHorizontalDirection();
		}
		else if (other.gameObject == topSide)
		{
			FlipVerticalDirection();
		}
		else if (other.gameObject == paddle)
		{
			Vector2 vector = new Vector2((other.contacts[0].point.x - other.transform.position.x) * paddleBounceXDamping, 1f);
			direction = vector.normalized;
			program.InvokePaddleAnimation();
		}
		else if (other.gameObject == bottomSide)
		{
			program.SubtractLife();
		}
		else
		{
			Rigidbody2D component = other.gameObject.GetComponent<Rigidbody2D>();
			Collider2D componentInChildren = component.GetComponentInChildren<Collider2D>();
			if (component != null && componentInChildren != null)
			{
				component.gravityScale = gravityScaleToSet;
				component.AddForce(new Vector3(direction.x * blockForceMultiplierX, direction.y * blockForceMultiplierY));
				component.AddTorque((other.contacts[0].point.x - base.transform.position.x) * blockRotationMultiplier);
				componentInChildren.isTrigger = true;
				Object.Destroy(other.gameObject, 2f);
			}
			else
			{
				Object.Destroy(other.gameObject);
			}
			program.RemoveBlockFromList(other.gameObject);
			currentSpeed += speedIncreaseOnBlockBreak;
			GameEventsManager.Instance.ItemActionOccurred(program.WID, "USED");
			if (program.Blocks.Count == 0)
			{
				program.ShowWinScreen();
			}
			Vector2 vector2 = new Vector2((other.contacts[0].point.x - other.transform.position.x) * blockBounceXDamping, other.contacts[0].point.y - other.transform.position.y);
			direction = vector2.normalized;
		}
		if (showSurprisedFace == null && base.isActiveAndEnabled)
		{
			showSurprisedFace = StartCoroutine(CollisionAnimation());
		}
	}

	private void CheckForOOB()
	{
		if (base.transform.position.x < leftSide.transform.position.x || base.transform.position.x > rightSide.transform.position.x || base.transform.position.y > topSide.transform.position.y || base.transform.position.y < bottomSide.transform.position.y)
		{
			program.ResetBall();
		}
	}

	private void FlipHorizontalDirection()
	{
		direction = new Vector2(0f - direction.x, direction.y);
	}

	private void FlipVerticalDirection()
	{
		direction = new Vector2(direction.x, 0f - direction.y);
	}

	public void Nooch()
	{
		direction = Vector2.zero;
	}

	public void SetBlockPrefab(GameObject g)
	{
		prefabReference = g;
	}

	public void SetPaddle(GameObject g)
	{
		paddle = g;
	}

	public void SetDirection(Vector2 dir)
	{
		direction = new Vector3(dir.x, dir.y, 0f);
	}

	public void SetProgram(BreakoutComputerProgram bcp)
	{
		program = bcp;
	}

	public void ResetSpeed()
	{
		currentSpeed = speed;
	}

	private void BallAnimations()
	{
		if (showSurprisedFace != null)
		{
			return;
		}
		if (blinkCounter == 0f)
		{
			blinkCounter = Random.Range(blinkCounterMin, blinkCounterMax);
		}
		if (blinking)
		{
			if (blinkTick >= blinkLength)
			{
				blinkTick = 0f;
				blinking = false;
				ballImage.sprite = ballSprites[0];
			}
		}
		else if (blinkTick >= blinkCounterMax)
		{
			blinkTick = 0f;
			ballImage.sprite = ballSprites[1];
			blinking = true;
		}
		blinkTick += Time.deltaTime;
	}

	public void ResetAnimState()
	{
		showSurprisedFace = null;
		ballImage.sprite = ballSprites[0];
		blinkTick = 0f;
	}

	private IEnumerator CollisionAnimation()
	{
		blinkTick = 0f;
		ballImage.sprite = ballSprites[2];
		while (blinkTick < showSurprisedFaceLength)
		{
			blinkTick += Time.deltaTime;
			yield return null;
		}
		blinkTick = 0f;
		ballImage.sprite = ballSprites[0];
		showSurprisedFace = null;
	}
}

using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class BreakoutComputerProgram : ComputerProgram
{
	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private Transform paddle;

	[SerializeField]
	private float paddleMoveSpeed;

	[SerializeField]
	private BreakoutBall ball;

	private Vector3 paddleMovement;

	private Vector3 paddlePositionCache;

	[SerializeField]
	private float maxDistanceFromCenter;

	[SerializeField]
	private Transform rootProgramObject;

	private bool holdingBall = true;

	private Vector2 launchRight = new Vector2(1f, 1f);

	private Vector2 launchLeft = new Vector2(-1f, 1f);

	[SerializeField]
	private GameObject blockPrefab;

	[SerializeField]
	private int numRows;

	[SerializeField]
	private int numColumns;

	private List<GameObject> blocks = new List<GameObject>();

	[SerializeField]
	private float blockGenMaxYPos;

	private Vector2 startGenerationSpot;

	private Vector2 currGenerationSpot;

	[SerializeField]
	private float blockSpacingX;

	[SerializeField]
	private float blockSpacingY;

	[SerializeField]
	private float mouseToPaddleOffset;

	[SerializeField]
	private float mouseDamping;

	[SerializeField]
	private TextMeshProUGUI winScreen;

	[SerializeField]
	private TextMeshProUGUI lifeCounter;

	[SerializeField]
	private int maxLives;

	private int lives;

	private bool showingWinScreen;

	private Vector3 paddleScaleCache;

	[SerializeField]
	private AnimationCurve ballHitPaddleValueX;

	[SerializeField]
	private AnimationCurve ballHitPaddleValueY;

	private Coroutine ballHitPaddleRoutine;

	public WorldItemData WID
	{
		get
		{
			return worldItemData;
		}
	}

	public List<GameObject> Blocks
	{
		get
		{
			return blocks;
		}
	}

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.BreakBot;
		}
	}

	private void OnEnable()
	{
		if (hostComputer != null)
		{
			hostComputer.HideCursor();
		}
		ResetGame();
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
	}

	private void OnDisable()
	{
		if (hostComputer != null)
		{
			hostComputer.ShowCursor();
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
	}

	private void Awake()
	{
		ResetGame();
		paddleScaleCache = paddle.localScale;
	}

	protected override bool OnMouseMove(Vector2 cursorPos)
	{
		paddle.localPosition = new Vector2((cursorPos.x + mouseToPaddleOffset) * mouseDamping, paddle.localPosition.y);
		return true;
	}

	protected override bool OnMouseClick(Vector2 cursorPos)
	{
		if (holdingBall && !showingWinScreen)
		{
			LaunchBall();
		}
		if (showingWinScreen)
		{
			ResetGame();
		}
		return true;
	}

	public void ResetBall()
	{
		ball.gameObject.SetActive(true);
		ball.Nooch();
		ball.transform.SetParent(paddle.transform, true);
		ball.transform.localPosition = ball.StartOffsetFromPaddle;
		holdingBall = true;
		ball.ResetAnimState();
	}

	private void LaunchBall()
	{
		ball.transform.SetParent(rootProgramObject, true);
		if (Random.Range(0, 2) == 0)
		{
			ball.SetDirection(launchLeft);
		}
		else
		{
			ball.SetDirection(launchRight);
		}
		ball.Direction.Normalize();
		holdingBall = false;
	}

	private void Generate()
	{
		blocks = new List<GameObject>();
		startGenerationSpot = new Vector2((0f - (float)(numColumns - 1) * blockSpacingX) * 0.5f, blockGenMaxYPos);
		currGenerationSpot = startGenerationSpot;
		for (int i = 0; i < numRows; i++)
		{
			for (int j = 0; j < numColumns; j++)
			{
				GameObject gameObject = Object.Instantiate(blockPrefab, rootProgramObject) as GameObject;
				blocks.Add(gameObject);
				gameObject.transform.localPosition = new Vector3(currGenerationSpot.x, currGenerationSpot.y, 0f);
				currGenerationSpot += new Vector2(blockSpacingX, 0f);
			}
			currGenerationSpot = new Vector2(startGenerationSpot.x, blockGenMaxYPos - blockSpacingY * (float)(i + 1));
		}
	}

	public void RemoveBlockFromList(GameObject g)
	{
		blocks.Remove(g);
	}

	public void SubtractLife()
	{
		lives--;
		UpdateLives();
		if (lives < 0)
		{
			ResetGame();
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
		}
		else
		{
			ResetBall();
		}
	}

	public void UpdateLives()
	{
		lifeCounter.text = "LIVES: " + lives;
	}

	public void ShowWinScreen()
	{
		showingWinScreen = true;
		winScreen.gameObject.SetActive(true);
		ball.gameObject.SetActive(false);
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
		ball.ResetSpeed();
	}

	private void ResetGame()
	{
		for (int num = blocks.Count - 1; num >= 0; num--)
		{
			Object.Destroy(blocks[num]);
		}
		paddlePositionCache = paddle.localPosition;
		lives = maxLives;
		UpdateLives();
		ResetBall();
		Generate();
		ball.SetBlockPrefab(blockPrefab);
		ball.SetPaddle(paddle.gameObject);
		ball.SetProgram(this);
		winScreen.gameObject.SetActive(false);
		ball.ResetSpeed();
		showingWinScreen = false;
	}

	public void InvokePaddleAnimation()
	{
		if (ballHitPaddleRoutine == null)
		{
			ballHitPaddleRoutine = StartCoroutine(PaddleAnimation());
		}
	}

	private IEnumerator PaddleAnimation()
	{
		float timer = 0f;
		float lastFrameTime = ballHitPaddleValueX.keys[ballHitPaddleValueX.length - 1].time;
		while (timer < lastFrameTime)
		{
			paddle.localScale = new Vector3(ballHitPaddleValueX.Evaluate(timer) * paddleScaleCache.x, ballHitPaddleValueY.Evaluate(timer) * paddleScaleCache.y);
			timer += Time.deltaTime;
			yield return null;
		}
		paddle.localScale = paddleScaleCache;
		ballHitPaddleRoutine = null;
	}
}

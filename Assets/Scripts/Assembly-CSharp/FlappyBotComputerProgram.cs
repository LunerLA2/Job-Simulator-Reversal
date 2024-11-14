using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class FlappyBotComputerProgram : ComputerProgram
{
	private enum GameState
	{
		Waiting = 0,
		Flapping = 1,
		Dead = 2
	}

	private const float GAME_SPEED = 170f;

	private const float MAX_PIPE_SPACING = 300f;

	private const float MIN_PIPE_SPACING = 65f;

	private const float PIPE_SPACING_SHRINK_RATE = 4f;

	private const float PIPE_GAP_SIZE = 200f;

	private const float MIN_BREATHING_SPACE = 100f;

	private const float MIN_PIPE_EXPOSURE = 40f;

	private const int PIXELS_PER_POINT = 10;

	private const float MAX_PIPE_OFFSET = 100f;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text highScoreText;

	[SerializeField]
	private RectTransform ground;

	[SerializeField]
	private RawImage groundImage;

	[SerializeField]
	private RectTransform clouds;

	[SerializeField]
	private RawImage cloudsImage;

	[SerializeField]
	private Collider2D ceilingCollider;

	[SerializeField]
	private FlappyBot botPrefab;

	[SerializeField]
	private GameObject pipePairPrefab;

	[SerializeField]
	private RectTransform pipePairsRoot;

	[SerializeField]
	private GameObject instructions;

	[SerializeField]
	private WorldItemData worldItemData;

	private GameState state;

	private FlappyBot bot;

	private float flapDistance;

	private int score;

	private int highScore;

	private RectTransform lastPipePair;

	private float pipeSpacing;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.FlappyBot;
		}
	}

	private void OnEnable()
	{
		hostComputer.HideCursor();
		state = GameState.Waiting;
		Reset();
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
	}

	private void OnDisable()
	{
		hostComputer.ShowCursor();
		ClearObjects();
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
	}

	private void Update()
	{
		if (state != 0)
		{
			if (state == GameState.Flapping)
			{
				flapDistance += Time.deltaTime * 170f;
				pipeSpacing = Mathf.Max(65f, pipeSpacing - Time.deltaTime * 4f);
				UpdatePipes();
				if (bot.State == FlappyBotState.Dead)
				{
					GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
					state = GameState.Dead;
				}
			}
			else if (state == GameState.Dead)
			{
				UpdatePipes();
				if (bot == null)
				{
					Reset();
				}
			}
		}
		Rect uvRect = groundImage.uvRect;
		uvRect.x += Time.deltaTime * (170f / ground.rect.width * uvRect.width);
		groundImage.uvRect = uvRect;
		Rect uvRect2 = cloudsImage.uvRect;
		uvRect2.x += Time.deltaTime * (170f / clouds.rect.width * uvRect2.width) / 2f;
		cloudsImage.uvRect = uvRect2;
		int num = (int)(flapDistance / 10f);
		if (num != score)
		{
			score = num;
			scoreText.text = score.ToString();
			if (score > highScore)
			{
				highScore = score;
				highScoreText.text = highScore.ToString();
			}
		}
	}

	private void UpdatePipes()
	{
		for (int i = 0; i < pipePairsRoot.childCount; i++)
		{
			(pipePairsRoot.GetChild(i) as RectTransform).anchoredPosition += new Vector2(-170f * Time.deltaTime, 0f);
		}
		if (pipePairsRoot.childCount > 0)
		{
			RectTransform rectTransform = pipePairsRoot.GetChild(0) as RectTransform;
			if (rectTransform.anchoredPosition.x < 0f - pipePairsRoot.rect.width)
			{
				Object.Destroy(rectTransform.gameObject);
			}
		}
		if (lastPipePair == null || lastPipePair.anchoredPosition.x < 0f - pipeSpacing)
		{
			RectTransform component = Object.Instantiate(pipePairPrefab).GetComponent<RectTransform>();
			component.SetParent(pipePairsRoot, false);
			float num = (pipePairsRoot.rect.height - 200f - 40f) / 2f;
			float num2 = ((!(lastPipePair != null)) ? 0f : lastPipePair.anchoredPosition.y);
			float max = Mathf.Min(num, num2 + 100f);
			float min = Mathf.Max(0f - num, num2 - 100f);
			component.anchoredPosition = new Vector2(0f, Random.Range(min, max));
			RectTransform rectTransform2 = component.GetChild(0) as RectTransform;
			RectTransform rectTransform3 = component.GetChild(1) as RectTransform;
			rectTransform2.anchoredPosition = new Vector2(0f, 100f);
			rectTransform3.anchoredPosition = new Vector2(0f, -100f);
			lastPipePair = component;
		}
	}

	private void ClearObjects()
	{
		if (bot != null)
		{
			Object.Destroy(bot.gameObject);
		}
		while (pipePairsRoot.childCount > 0)
		{
			Transform child = pipePairsRoot.GetChild(0);
			if (child != null)
			{
				child.SetParent(null);
				Object.Destroy(child.gameObject);
			}
		}
	}

	private void Reset()
	{
		ClearObjects();
		flapDistance = 0f;
		pipeSpacing = 300f;
		bot = Object.Instantiate(botPrefab);
		bot.transform.SetParent(base.transform, false);
		bot.AddNonLethal(ceilingCollider);
		bot.HostComputer = hostComputer;
		instructions.SetActive(true);
		state = GameState.Waiting;
	}

	protected override bool OnKeyPress(string code)
	{
		if (code == "1")
		{
			Flap();
		}
		return true;
	}

	protected override bool OnMouseClick(Vector2 cursorPos)
	{
		Flap();
		return true;
	}

	private void Flap()
	{
		if (state == GameState.Waiting)
		{
			instructions.SetActive(false);
			state = GameState.Flapping;
			bot.BeginFlapping();
		}
		else if (state == GameState.Flapping)
		{
			bot.Flap();
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "USED");
		}
	}
}

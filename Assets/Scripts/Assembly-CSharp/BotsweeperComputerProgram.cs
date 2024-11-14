using System;
using System.Collections.Generic;
using System.Linq;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class BotsweeperComputerProgram : ComputerProgram
{
	[Serializable]
	private class Difficulty
	{
		[SerializeField]
		public string Description;

		[SerializeField]
		public Sprite Graphic;

		[SerializeField]
		public int NumMines;
	}

	[SerializeField]
	private int rows = 7;

	[SerializeField]
	private int columns = 13;

	[SerializeField]
	private Difficulty[] difficulties;

	[SerializeField]
	private int defaultDifficultyIndex = 2;

	[SerializeField]
	private Sprite idleStatusAvatarSprite;

	[SerializeField]
	private Sprite nervousStatusAvatarSprite;

	[SerializeField]
	private Sprite winStatusAvatarSprite;

	[SerializeField]
	private Sprite deadStatusAvatarSprite;

	[SerializeField]
	private GameObject newGameScreen;

	[SerializeField]
	private GameObject playingScreen;

	[SerializeField]
	private Text difficultyText;

	[SerializeField]
	private ComputerClickable increaseDifficultyButton;

	[SerializeField]
	private ComputerClickable decreaseDifficultyButton;

	[SerializeField]
	private Image difficultyGraphicImage;

	[SerializeField]
	private ComputerClickable startButton;

	[SerializeField]
	private GridLayoutGroup grid;

	[SerializeField]
	private ComputerClickable newGameButton;

	[SerializeField]
	private Text mineCountText;

	[SerializeField]
	private Image statusAvatarImage;

	[SerializeField]
	private ComputerClickable quitButton;

	[SerializeField]
	private BotsweeperCell cellPrefab;

	[SerializeField]
	private AudioClip revealSound;

	[SerializeField]
	private AudioClip cascadeSound;

	[SerializeField]
	private AudioClip winSound;

	[SerializeField]
	private AudioClip deathSound;

	[SerializeField]
	private WorldItemData worldItemData;

	private bool isPlaying;

	private bool isGameOver;

	private int selectedDifficultyIndex;

	private BotsweeperCell[,] cells;

	private List<BotsweeperCell> mineCells;

	private int numMinesLeft;

	private int numCellsUp;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Botsweeper;
		}
	}

	private void Awake()
	{
		selectedDifficultyIndex = defaultDifficultyIndex;
		cells = new BotsweeperCell[rows, columns];
		mineCells = new List<BotsweeperCell>();
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				BotsweeperCell botsweeperCell = UnityEngine.Object.Instantiate(cellPrefab);
				botsweeperCell.gameObject.RemoveCloneFromName();
				botsweeperCell.name = "Cell_" + i + "_" + j;
				botsweeperCell.transform.SetParent(grid.transform, false);
				botsweeperCell.SetCoordinates(i, j);
				botsweeperCell.gameObject.SetActive(true);
				cells[i, j] = botsweeperCell;
				botsweeperCell.Clickable.Highlighted += OnCellHighlighted;
				botsweeperCell.Clickable.Unhighlighted += OnCellUnhighlighted;
				botsweeperCell.Clickable.Clicked += OnCellClicked;
				botsweeperCell.Clickable.ClickedUp += OnCellClickedUp;
			}
		}
		UpdateDifficulty();
	}

	private void UpdateDifficulty()
	{
		difficultyText.text = difficulties[selectedDifficultyIndex].Description;
		difficultyGraphicImage.sprite = difficulties[selectedDifficultyIndex].Graphic;
		increaseDifficultyButton.SetInteractive(selectedDifficultyIndex < difficulties.Length - 1);
		decreaseDifficultyButton.SetInteractive(selectedDifficultyIndex > 0);
	}

	private void OnEnable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
	}

	private void OnDisable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
	}

	private void GoToPlayingScreen()
	{
		newGameScreen.SetActive(false);
		playingScreen.SetActive(true);
		isPlaying = true;
	}

	private void GoToNewGameScreen()
	{
		isPlaying = false;
		playingScreen.SetActive(false);
		newGameScreen.SetActive(true);
	}

	private void StartGame()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				BotsweeperCell botsweeperCell = cells[i, j];
				botsweeperCell.Reset();
			}
		}
		numCellsUp = rows * columns;
		numMinesLeft = Mathf.Min(difficulties[selectedDifficultyIndex].NumMines, numCellsUp);
		mineCountText.text = numMinesLeft.ToString("000");
		mineCells.Clear();
		if (selectedDifficultyIndex == difficulties.Length - 1)
		{
			int num = UnityEngine.Random.Range(0, rows);
			int num2 = UnityEngine.Random.Range(0, columns);
			for (int k = 0; k < rows; k++)
			{
				for (int l = 0; l < columns; l++)
				{
					if (k != num || l != num2)
					{
						BotsweeperCell botsweeperCell2 = cells[k, l];
						botsweeperCell2.PlantMine();
						mineCells.Add(botsweeperCell2);
					}
				}
			}
		}
		else
		{
			while (mineCells.Count < numMinesLeft)
			{
				BotsweeperCell botsweeperCell3 = null;
				int num3 = -1;
				int num4 = -1;
				while (botsweeperCell3 == null || botsweeperCell3.HasMine)
				{
					num3 = UnityEngine.Random.Range(0, rows);
					num4 = UnityEngine.Random.Range(0, columns);
					botsweeperCell3 = cells[num3, num4];
				}
				botsweeperCell3.PlantMine();
				mineCells.Add(botsweeperCell3);
				for (int m = num3 - 1; m <= num3 + 1; m++)
				{
					if (m < 0 || m >= rows)
					{
						continue;
					}
					for (int n = num4 - 1; n <= num4 + 1; n++)
					{
						if (n >= 0 && n < columns)
						{
							cells[m, n].IncrementDanger();
						}
					}
				}
			}
		}
		isGameOver = false;
	}

	private void EndGame(bool win)
	{
		isGameOver = true;
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				BotsweeperCell botsweeperCell = cells[i, j];
				botsweeperCell.ShowMine();
				botsweeperCell.Clickable.SetInteractive(false);
			}
		}
		statusAvatarImage.sprite = ((!win) ? deadStatusAvatarSprite : winStatusAvatarSprite);
		hostComputer.PlaySound((!win) ? deathSound : winSound);
	}

	private void RevealFrom(BotsweeperCell epicenter)
	{
		HashSet<BotsweeperCell> hashSet = new HashSet<BotsweeperCell>();
		hashSet.Add(epicenter);
		hostComputer.PlaySound((epicenter.Danger <= 0) ? cascadeSound : revealSound);
		while (hashSet.Count > 0)
		{
			BotsweeperCell[] array = hashSet.ToArray();
			foreach (BotsweeperCell botsweeperCell in array)
			{
				hashSet.Remove(botsweeperCell);
				if (botsweeperCell.State == BotsweeperCellState.Revealed)
				{
					continue;
				}
				botsweeperCell.SetState(BotsweeperCellState.Revealed);
				numCellsUp--;
				if (botsweeperCell.Danger > 0 || botsweeperCell.HasMine)
				{
					continue;
				}
				for (int j = -1; j <= 1; j++)
				{
					for (int k = -1; k <= 1; k++)
					{
						if (j != 0 || k != 0)
						{
							AddCellToFringe(hashSet, botsweeperCell.Row + j, botsweeperCell.Column + k);
						}
					}
				}
			}
		}
	}

	private void AddCellToFringe(HashSet<BotsweeperCell> fringe, int row, int col)
	{
		if (row >= 0 && row < rows && col >= 0 && col < columns)
		{
			BotsweeperCell botsweeperCell = cells[row, col];
			if (botsweeperCell.State != BotsweeperCellState.Revealed && !botsweeperCell.HasMine)
			{
				fringe.Add(botsweeperCell);
			}
		}
	}

	private void Update()
	{
	}

	protected override bool OnMouseClick(Vector2 cursorPos)
	{
		if (isPlaying && !isGameOver)
		{
			statusAvatarImage.sprite = nervousStatusAvatarSprite;
		}
		return true;
	}

	protected override bool OnMouseClickUp(Vector2 cursorPos)
	{
		if (isPlaying && !isGameOver)
		{
			statusAvatarImage.sprite = idleStatusAvatarSprite;
		}
		return true;
	}

	private void OnCellHighlighted(ComputerClickable clickable)
	{
		if (hostComputer.IsMouseButtonDown)
		{
			BotsweeperCell component = clickable.GetComponent<BotsweeperCell>();
			if (component.State == BotsweeperCellState.Up)
			{
				component.SetState(BotsweeperCellState.Down);
			}
		}
	}

	private void OnCellUnhighlighted(ComputerClickable clickable)
	{
		BotsweeperCell component = clickable.GetComponent<BotsweeperCell>();
		if (component.State == BotsweeperCellState.Down)
		{
			component.SetState(BotsweeperCellState.Up);
		}
	}

	private void OnCellClicked(ComputerClickable clickable)
	{
		BotsweeperCell component = clickable.GetComponent<BotsweeperCell>();
		if (component.State == BotsweeperCellState.Up)
		{
			component.SetState(BotsweeperCellState.Down);
		}
	}

	private void OnCellClickedUp(ComputerClickable clickable)
	{
		ComputerClickable highlightedClickable = hostComputer.HighlightedClickable;
		if (highlightedClickable == null)
		{
			return;
		}
		BotsweeperCell component = highlightedClickable.GetComponent<BotsweeperCell>();
		if (component == null || component.State == BotsweeperCellState.Revealed)
		{
			return;
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "USED");
		if (component.HasMine)
		{
			component.SetState(BotsweeperCellState.Revealed);
			EndGame(false);
			return;
		}
		RevealFrom(component);
		if (numCellsUp == mineCells.Count)
		{
			EndGame(true);
		}
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable == quitButton)
		{
			Finish();
		}
		else if (isPlaying)
		{
			if (clickable == newGameButton)
			{
				GoToNewGameScreen();
			}
		}
		else if (clickable == increaseDifficultyButton)
		{
			if (selectedDifficultyIndex < difficulties.Length - 1)
			{
				selectedDifficultyIndex++;
				UpdateDifficulty();
			}
		}
		else if (clickable == decreaseDifficultyButton)
		{
			if (selectedDifficultyIndex > 0)
			{
				selectedDifficultyIndex--;
				UpdateDifficulty();
			}
		}
		else if (clickable == startButton)
		{
			GoToPlayingScreen();
			StartGame();
		}
	}
}

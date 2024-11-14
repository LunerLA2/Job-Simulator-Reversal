using UnityEngine;
using UnityEngine.UI;

public class BotsweeperCell : MonoBehaviour
{
	[SerializeField]
	private Image cellImage;

	[SerializeField]
	private Text dangerText;

	[SerializeField]
	private Image mineIcon;

	[SerializeField]
	private Sprite upSprite;

	[SerializeField]
	private Sprite downSprite;

	[SerializeField]
	private Sprite revealedSprite;

	[SerializeField]
	private Color[] dangerColors;

	[SerializeField]
	private ComputerClickable clickable;

	private BotsweeperCellState state;

	private bool hasMine;

	private int danger;

	private int row;

	private int col;

	public bool HasMine
	{
		get
		{
			return hasMine;
		}
	}

	public BotsweeperCellState State
	{
		get
		{
			return state;
		}
	}

	public ComputerClickable Clickable
	{
		get
		{
			return clickable;
		}
	}

	public int Row
	{
		get
		{
			return row;
		}
	}

	public int Column
	{
		get
		{
			return col;
		}
	}

	public int Danger
	{
		get
		{
			return danger;
		}
	}

	private void Awake()
	{
	}

	public void SetCoordinates(int row, int col)
	{
		this.row = row;
		this.col = col;
	}

	public void Reset()
	{
		hasMine = false;
		danger = 0;
		mineIcon.enabled = false;
		SetState(BotsweeperCellState.Up);
	}

	public void PlantMine()
	{
		hasMine = true;
	}

	public void IncrementDanger()
	{
		danger++;
	}

	public void ShowMine()
	{
		if (hasMine)
		{
			mineIcon.enabled = true;
			mineIcon.color = ((state != BotsweeperCellState.Revealed) ? Color.black : Color.white);
		}
	}

	public void SetState(BotsweeperCellState state)
	{
		this.state = state;
		switch (state)
		{
		case BotsweeperCellState.Up:
			cellImage.color = Color.white;
			cellImage.sprite = upSprite;
			dangerText.enabled = false;
			clickable.SetInteractive(true);
			break;
		case BotsweeperCellState.Down:
			cellImage.color = Color.white;
			cellImage.sprite = downSprite;
			dangerText.enabled = false;
			clickable.SetInteractive(true);
			break;
		case BotsweeperCellState.Revealed:
			cellImage.sprite = revealedSprite;
			clickable.SetInteractive(false);
			if (hasMine)
			{
				cellImage.color = Color.red;
				dangerText.enabled = false;
				break;
			}
			cellImage.color = Color.white;
			dangerText.enabled = true;
			dangerText.text = danger.ToString();
			dangerText.color = dangerColors[danger];
			break;
		}
	}
}

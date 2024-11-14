using UnityEngine;
using UnityEngine.UI;

public class FidgetSite : ComputerProgram
{
	private bool SpinnerPage = true;

	[SerializeField]
	private ComputerClickable Design1Button;

	[SerializeField]
	private ComputerClickable Design2Button;

	[SerializeField]
	private ComputerClickable Design3Button;

	[SerializeField]
	private ComputerClickable FlipEasyButton;

	[SerializeField]
	private ComputerClickable FlipGoodButton;

	[SerializeField]
	private ComputerClickable SpinnerPageButton;

	[SerializeField]
	private ComputerClickable FlipPageButton;

	[SerializeField]
	private Image PreviewBase;

	[SerializeField]
	private Image PreviewLogo;

	private Material TargetMaterial;

	private Color TargetColor;

	[SerializeField]
	private Material SpinnerMat1;

	[SerializeField]
	private Material SpinnerMat2;

	[SerializeField]
	private Material SpinnerMat3;

	[SerializeField]
	private Sprite PreviewMat1;

	[SerializeField]
	private Sprite PreviewMat2;

	[SerializeField]
	private Sprite PreviewMat3;

	[SerializeField]
	private Sprite FlipEasySprite;

	[SerializeField]
	private Sprite FlipGoodSprite;

	[SerializeField]
	private Sprite SpinBaseSprite;

	[SerializeField]
	private ComputerClickable ColorPicker;

	[SerializeField]
	private Gradient ColorGradient;

	[SerializeField]
	private ComputerClickable PrintButton;

	[SerializeField]
	private GameObject SpinnerPrefab;

	[SerializeField]
	private GameObject FlipEasyPrefab;

	[SerializeField]
	private GameObject FlipGoodPrefab;

	private GameObject FlipPrefab;

	[SerializeField]
	private Sprite SpinSite;

	[SerializeField]
	private Sprite FlipSite;

	[SerializeField]
	private Image Content;

	[HideInInspector]
	public new ComputerClickable[] Clickables;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.FidgetSpinner;
		}
	}

	private void Start()
	{
		AddClickable(Design1Button);
		AddClickable(Design2Button);
		AddClickable(Design3Button);
		AddClickable(FlipEasyButton);
		AddClickable(FlipGoodButton);
		AddClickable(ColorPicker);
		AddClickable(PrintButton);
		AddClickable(SpinnerPageButton);
		Clickables = new ComputerClickable[8];
		Clickables[0] = Design1Button;
		Clickables[1] = Design2Button;
		Clickables[2] = Design3Button;
		Clickables[3] = FlipEasyButton;
		Clickables[4] = FlipGoodButton;
		Clickables[5] = ColorPicker;
		Clickables[6] = PrintButton;
		Clickables[7] = SpinnerPageButton;
		HideElements();
		TargetMaterial = SpinnerMat1;
		TargetColor = ColorGradient.Evaluate(0f);
		PreviewBase.color = TargetColor;
		PreviewLogo.sprite = PreviewMat1;
		base.gameObject.SetActive(false);
	}

	public new void AddClickable(ComputerClickable clickable)
	{
		clickable.CursorEntered += OnClickableCursorEntered;
		clickable.CursorExited += OnClickableCursorExited;
		clickable.Clicked += OnClickableClicked;
		clickable.ClickedUp += OnClickableStopClick;
		clickable.Highlighted += OnClickableHighlighted;
		clickable.Unhighlighted += OnClickableUnhighlighted;
	}

	protected override void OnClickableClicked(ComputerClickable Clicked)
	{
		MonoBehaviour.print("CLICKED " + Clicked);
		if (Clicked.name == "FidgetDesign1")
		{
			TargetMaterial = SpinnerMat1;
			PreviewLogo.sprite = PreviewMat1;
		}
		if (Clicked.name == "FidgetDesign2")
		{
			TargetMaterial = SpinnerMat2;
			PreviewLogo.sprite = PreviewMat2;
		}
		if (Clicked.name == "FidgetDesign3")
		{
			TargetMaterial = SpinnerMat3;
			PreviewLogo.sprite = PreviewMat3;
		}
		if (Clicked.name == "FidgetColorPicker")
		{
			float num = (hostComputer.CursorPosition.x - 249f) / 362f;
			MonoBehaviour.print(num);
			TargetColor = ColorGradient.Evaluate(num);
			PreviewBase.color = TargetColor;
		}
		if (Clicked.name == "FidgetPrintButton")
		{
			MonoBehaviour.print("Print");
			if (!hostComputer.IsPrinterBusy && SpinnerPage)
			{
				hostComputer.PrintObject(SpinnerPrefab, null, delegate(GameObject go)
				{
					go.GetComponent<FidgetSpinner>().SetupArt(TargetMaterial, TargetColor);
				});
			}
			if (!hostComputer.IsPrinterBusy && !SpinnerPage)
			{
				hostComputer.PrintObject(FlipPrefab);
			}
		}
		if (Clicked.name == "FlipEasy")
		{
			FlipPrefab = FlipEasyPrefab;
			PreviewBase.sprite = FlipEasySprite;
		}
		if (Clicked.name == "FlipGood")
		{
			FlipPrefab = FlipGoodPrefab;
			PreviewBase.sprite = FlipGoodSprite;
		}
		if (Clicked.name == "SpinProductPage")
		{
			SpinnerPage = true;
			Content.sprite = SpinSite;
			PreviewLogo.gameObject.SetActive(true);
			PreviewBase.sprite = SpinBaseSprite;
			PreviewBase.color = TargetColor;
			Design1Button.SetInteractive(true);
			Design2Button.SetInteractive(true);
			Design3Button.SetInteractive(true);
			FlipEasyButton.SetInteractive(false);
			FlipGoodButton.SetInteractive(false);
			ColorPicker.SetInteractive(true);
		}
		if (Clicked.name == "FlipProductPage")
		{
			SpinnerPage = false;
			Content.sprite = FlipSite;
			FlipPrefab = FlipGoodPrefab;
			PreviewBase.sprite = FlipGoodSprite;
			PreviewBase.color = Color.white;
			PreviewLogo.gameObject.SetActive(false);
			Design1Button.SetInteractive(false);
			Design2Button.SetInteractive(false);
			Design3Button.SetInteractive(false);
			FlipEasyButton.SetInteractive(true);
			FlipGoodButton.SetInteractive(true);
			ColorPicker.SetInteractive(false);
		}
	}

	private void RevealElements()
	{
		PrintButton.SetInteractive(true);
		if (!SpinnerPage)
		{
			Content.sprite = FlipSite;
			PreviewBase.gameObject.SetActive(true);
			PreviewBase.color = Color.white;
			Design1Button.SetInteractive(false);
			Design2Button.SetInteractive(false);
			Design3Button.SetInteractive(false);
			FlipEasyButton.SetInteractive(true);
			FlipGoodButton.SetInteractive(true);
			ColorPicker.SetInteractive(false);
		}
		else
		{
			Content.sprite = SpinSite;
			PreviewBase.gameObject.SetActive(true);
			TargetColor = ColorGradient.Evaluate(0f);
			PreviewBase.color = TargetColor;
			PreviewLogo.gameObject.SetActive(true);
			Design1Button.SetInteractive(true);
			Design2Button.SetInteractive(true);
			Design3Button.SetInteractive(true);
			FlipEasyButton.SetInteractive(false);
			FlipGoodButton.SetInteractive(false);
			ColorPicker.SetInteractive(true);
		}
	}

	private void HideElements()
	{
		PreviewBase.gameObject.SetActive(false);
		PreviewLogo.gameObject.SetActive(false);
	}

	private void Update()
	{
	}
}

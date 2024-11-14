using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class WordComputerProgram : ComputerProgram
{
	[Serializable]
	public class DocumentTemplate
	{
		public string Name;

		[Multiline(5)]
		public string Content;

		public string TriggerString;

		public Material PrintMaterial;

		public WorldItemData PrintWorldItemData;

		public GameObject PrintPrefab;
	}

	private const string CURSOR_TEXT = "<color=red>_</color>";

	[SerializeField]
	protected DocumentTemplate[] templates;

	[SerializeField]
	private GameObject templateScreen;

	[SerializeField]
	private GameObject editingScreen;

	[SerializeField]
	private ComputerClickable nextTemplateButton;

	[SerializeField]
	private ComputerClickable prevTemplateButton;

	[SerializeField]
	private ComputerClickable goButton;

	[SerializeField]
	private Text templateNameText;

	[SerializeField]
	protected ComputerClickable printButton;

	[SerializeField]
	private ComputerClickable newButton;

	[SerializeField]
	private ComputerClickable clearButton;

	[SerializeField]
	private ComputerClickable quitButton;

	[SerializeField]
	private GameObject clipBot;

	[SerializeField]
	private int clipBotTriggerCharCount = 8;

	[SerializeField]
	private ComputerClickable clipBotYesButton;

	[SerializeField]
	private ComputerClickable clipBotNoButton;

	[SerializeField]
	private Text contentText;

	[SerializeField]
	private Text clipBotText;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private AudioClip clipBotTriggerSound;

	[SerializeField]
	private AudioClip contentFinishedSound;

	[SerializeField]
	private GameObject printPrefab;

	[SerializeField]
	private int charactersPerKeypressMin = 1;

	[SerializeField]
	private int charactersPerKeypressMax = 1;

	protected int selectedTemplateIndex;

	protected int detectedTemplateIndex = -1;

	protected int numRevealedChars;

	public Action<int> OnTemplateWasPrinted;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Word;
		}
	}

	protected virtual void OnEnable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
	}

	protected virtual void OnDisable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
	}

	public virtual void Update()
	{
		bool flag = selectedTemplateIndex == 0 || numRevealedChars >= templates[selectedTemplateIndex].Content.Length;
		printButton.SetInteractive(editingScreen.activeInHierarchy && flag && !hostComputer.IsPrinterBusy && !clipBot.activeInHierarchy);
		printButton.Text.text = ((!hostComputer.IsPrinterBusy) ? "Print" : "Printing...");
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable == nextTemplateButton)
			{
				OffsetTemplateIndex(1);
			}
			else if (clickable == prevTemplateButton)
			{
				OffsetTemplateIndex(-1);
			}
			else if (clickable == goButton)
			{
				numRevealedChars = 0;
				templateScreen.SetActive(false);
				editingScreen.SetActive(true);
				UpdateContent(string.Empty);
			}
			else if (clickable == printButton)
			{
				Print();
			}
			else if (clickable == newButton)
			{
				numRevealedChars = 0;
				editingScreen.SetActive(false);
				templateScreen.SetActive(true);
				SetTemplateIndex(0);
			}
			else if (clickable == clearButton)
			{
				numRevealedChars = 0;
				UpdateContent(string.Empty);
			}
			else if (clickable == clipBotYesButton)
			{
				DismissClipBot();
				selectedTemplateIndex = detectedTemplateIndex;
				UpdateContent(string.Empty);
			}
			else if (clickable == clipBotNoButton)
			{
				DismissClipBot();
			}
			else if (clickable == quitButton)
			{
				Finish();
			}
		}
	}

	private void TriggerClipBot()
	{
		string text = contentText.text;
		detectedTemplateIndex = -1;
		for (int i = 1; i < templates.Length; i++)
		{
			if (text.StartsWith(templates[i].TriggerString))
			{
				detectedTemplateIndex = i;
				break;
			}
		}
		if (detectedTemplateIndex != -1)
		{
			hostComputer.PlaySound(clipBotTriggerSound);
			printButton.SetInteractive(false);
			newButton.SetInteractive(false);
			clearButton.SetInteractive(false);
			clipBot.SetActive(true);
			clipBotText.text = "It looks like you're trying to\nwrite a <color=blue>" + templates[detectedTemplateIndex].Name + "</color>\nWould you like help with that?";
		}
	}

	private void DismissClipBot()
	{
		clipBot.SetActive(false);
		printButton.SetInteractive(true);
		newButton.SetInteractive(true);
		clearButton.SetInteractive(true);
	}

	protected override bool OnKeyPress(string code)
	{
		int num = UnityEngine.Random.Range(charactersPerKeypressMin, charactersPerKeypressMax);
		if (selectedTemplateIndex == 0)
		{
			num = 1;
		}
		for (int i = 1; i <= num; i++)
		{
			IndividualKeyPress(code);
		}
		return true;
	}

	private void IndividualKeyPress(string code)
	{
		if (editingScreen.activeInHierarchy && !clipBot.activeInHierarchy && !hostComputer.IsPrinterBusy && (code == "0" || code == "1"))
		{
			numRevealedChars++;
			UpdateContent(code);
			if (selectedTemplateIndex == 0 && numRevealedChars == clipBotTriggerCharCount)
			{
				TriggerClipBot();
			}
		}
	}

	private void OffsetTemplateIndex(int offset)
	{
		SetTemplateIndex((selectedTemplateIndex + templates.Length + offset) % templates.Length);
	}

	private void SetTemplateIndex(int index)
	{
		selectedTemplateIndex = index;
		templateNameText.text = templates[selectedTemplateIndex].Name;
		UpdateContent(string.Empty);
	}

	private void UpdateContent(string input)
	{
		bool flag = false;
		contentText.text = contentText.text.Substring(0, Mathf.Max(0, contentText.text.Length - "<color=red>_</color>".Length));
		if (selectedTemplateIndex == 0)
		{
			contentText.text += input;
			contentText.text = contentText.text.Substring(0, numRevealedChars);
		}
		else
		{
			bool flag2 = contentText.text.Length >= templates[selectedTemplateIndex].Content.Length;
			string content = templates[selectedTemplateIndex].Content;
			numRevealedChars = Mathf.Min(numRevealedChars, content.Length);
			contentText.text = content.Substring(0, numRevealedChars);
			bool flag3 = contentText.text.Length >= templates[selectedTemplateIndex].Content.Length;
			if (flag3)
			{
				flag = true;
			}
			if (!flag2 && flag3)
			{
				hostComputer.PlaySound(contentFinishedSound);
			}
		}
		if (!flag)
		{
			contentText.text += "<color=red>_</color>";
		}
	}

	private void Print()
	{
		if (selectedTemplateIndex == 0)
		{
			string textToPrint = contentText.text.Substring(0, Mathf.Max(0, contentText.text.Length - "<color=red>_</color>".Length));
			hostComputer.PrintObject(printPrefab, null, delegate(GameObject go)
			{
				go.GetComponent<PostcardController>().SetupCustomText(textToPrint);
			});
		}
		else
		{
			Material matToPrint = templates[selectedTemplateIndex].PrintMaterial;
			WorldItemData printWorldItemData = templates[selectedTemplateIndex].PrintWorldItemData;
			GameObject gameObject = templates[selectedTemplateIndex].PrintPrefab;
			if (gameObject != null)
			{
				hostComputer.PrintObject(gameObject);
			}
			else if (matToPrint != null)
			{
				hostComputer.PrintObject(printPrefab, printWorldItemData, delegate(GameObject go)
				{
					go.GetComponent<PostcardController>().SetupSkin(matToPrint);
				});
			}
			else
			{
				string textToPrint2 = contentText.text.Substring(0, Mathf.Max(0, contentText.text.Length));
				hostComputer.PrintObject(printPrefab, printWorldItemData, delegate(GameObject go)
				{
					go.GetComponent<PostcardController>().SetupCustomText(textToPrint2);
				});
			}
		}
		if (OnTemplateWasPrinted != null)
		{
			OnTemplateWasPrinted(selectedTemplateIndex);
		}
	}

	protected void ResetProgram(ComputerProgram compProg)
	{
		numRevealedChars = 0;
		editingScreen.SetActive(false);
		templateScreen.SetActive(true);
		SetTemplateIndex(0);
	}
}

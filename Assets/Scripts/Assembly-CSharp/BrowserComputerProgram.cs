using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class BrowserComputerProgram : ComputerProgram
{
	[Serializable]
	private class Website
	{
		[SerializeField]
		public string Address;

		[SerializeField]
		public Sprite PageSprite;

		[SerializeField]
		public Sprite PageSpriteNight;

		[SerializeField]
		public GameObject PrefabToPrint;

		[SerializeField]
		public GameObject PrefabToPrintNight;

		[SerializeField]
		public WebsiteExtraPrintButton[] ExtraPrintButtons;

		[SerializeField]
		public bool IsProgram;

		[SerializeField]
		public GameObject PageObject;
	}

	[Serializable]
	private class WebsiteExtraPrintButton
	{
		[SerializeField]
		public ComputerClickable PrintButtonClickable;

		[SerializeField]
		public GameObject PrefabToPrint;
	}

	[SerializeField]
	private Website[] websites;

	[SerializeField]
	private Website[] gamedevjobWebsites;

	private Website[] activeWebsites;

	[SerializeField]
	private float approximagePageLoadDuration = 3f;

	[SerializeField]
	private GameObject connectionScreen;

	[SerializeField]
	private GameObject browsingScreen;

	[SerializeField]
	private ComputerClickable quitButton;

	[SerializeField]
	private Text connectionStatusText;

	[SerializeField]
	private ComputerClickable addressButton;

	[SerializeField]
	private ComputerToggleGroup addressOptionsToggleGroup;

	[SerializeField]
	private RectTransform printButtonRectParent;

	[SerializeField]
	private RectTransform printButtonGameDevRectParent;

	[SerializeField]
	private Image pageContentImage;

	[SerializeField]
	private Text pageStatusText;

	[SerializeField]
	private ComputerClickable printPageButton;

	[SerializeField]
	private AudioClip connectionSound;

	[SerializeField]
	private WorldItemData programWorldItemData;

	private bool isConnected;

	private bool isLoaded;

	private Coroutine connectionCoroutine;

	private Coroutine loadingCoroutine;

	private Coroutine addressMenuCoroutine;

	private bool isAddressMenuOpen;

	private ComputerTogglable[] addressTogglables;

	private bool isGamedev;

	private Website lastLoadedSite;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Browser;
		}
	}

	private void Awake()
	{
		isGamedev = false;
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.OfficeModMode))
		{
			isGamedev = true;
		}
		activeWebsites = ((!isGamedev) ? websites : gamedevjobWebsites);
		addressTogglables = new ComputerTogglable[activeWebsites.Length];
		for (int i = 0; i < activeWebsites.Length; i++)
		{
			addressTogglables[i] = addressOptionsToggleGroup.transform.GetChild(i).GetComponent<ComputerTogglable>();
			addressTogglables[i].Text.text = activeWebsites[i].Address;
		}
		addressOptionsToggleGroup.SelectionChanged += OnAddressChanged;
	}

	private void OnEnable()
	{
		if (!isConnected)
		{
			connectionCoroutine = StartCoroutine(ConnectAsync());
		}
		else if (!isLoaded)
		{
			ClearPage();
			if (addressOptionsToggleGroup.SelectionIndex != -1)
			{
				loadingCoroutine = StartCoroutine(LoadPageAsync(activeWebsites[addressOptionsToggleGroup.SelectionIndex]));
			}
		}
		GameEventsManager.Instance.ItemActionOccurred(programWorldItemData, "OPENED");
	}

	private void OnDisable()
	{
		if (connectionCoroutine != null)
		{
			StopCoroutine(connectionCoroutine);
			connectionCoroutine = null;
		}
		hostComputer.StopSound(connectionSound);
		if (loadingCoroutine != null)
		{
			StopCoroutine(loadingCoroutine);
			loadingCoroutine = null;
		}
		FullyCloseAddressMenu();
		GameEventsManager.Instance.ItemActionOccurred(programWorldItemData, "CLOSED");
	}

	private IEnumerator ConnectAsync()
	{
		isConnected = false;
		hostComputer.PlaySound(connectionSound);
		float connectionTimeLeft = connectionSound.length + 0.5f;
		int prevNumDots = 0;
		while (connectionTimeLeft > 0f)
		{
			if (connectionTimeLeft <= 1f)
			{
				connectionStatusText.text = "CONNECTED!";
			}
			else
			{
				int numDots = (int)(Time.time * 4f) % 4;
				if (numDots != prevNumDots)
				{
					connectionStatusText.text = "Connecting";
					for (int i = 0; i < numDots; i++)
					{
						connectionStatusText.text += ".";
					}
					prevNumDots = numDots;
				}
			}
			yield return null;
			connectionTimeLeft -= Time.deltaTime;
		}
		connectionScreen.SetActive(false);
		browsingScreen.SetActive(true);
		ClearPage();
		isConnected = true;
		connectionCoroutine = null;
	}

	private void ClearPage()
	{
		if (loadingCoroutine != null)
		{
			StopCoroutine(loadingCoroutine);
			loadingCoroutine = null;
		}
		pageContentImage.fillAmount = 0f;
		printPageButton.SetInteractive(false);
		pageStatusText.text = string.Empty;
		addressButton.SetInteractive(true);
		FullyCloseAddressMenu();
		isLoaded = false;
	}

	private IEnumerator LoadPageAsync(Website selectedWebsite)
	{
		if (lastLoadedSite != null)
		{
			if (lastLoadedSite.ExtraPrintButtons != null)
			{
				for (int i = 0; i < lastLoadedSite.ExtraPrintButtons.Length; i++)
				{
					lastLoadedSite.ExtraPrintButtons[i].PrintButtonClickable.gameObject.SetActive(false);
				}
			}
			if (lastLoadedSite.IsProgram && lastLoadedSite.PageObject != null)
			{
				lastLoadedSite.PageObject.SendMessage("HideElements");
				lastLoadedSite.PageObject.SetActive(false);
				for (int k = 0; k < lastLoadedSite.PageObject.GetComponent<FidgetSite>().Clickables.Length; k++)
				{
					lastLoadedSite.PageObject.GetComponent<FidgetSite>().Clickables[k].SetInteractive(false);
				}
			}
		}
		lastLoadedSite = selectedWebsite;
		if (!selectedWebsite.IsProgram)
		{
			if (lastLoadedSite != null && lastLoadedSite.ExtraPrintButtons != null)
			{
				for (int j = 0; j < lastLoadedSite.ExtraPrintButtons.Length; j++)
				{
					lastLoadedSite.ExtraPrintButtons[j].PrintButtonClickable.gameObject.SetActive(true);
				}
			}
			isLoaded = false;
			pageStatusText.text = "Loading...";
			printPageButton.SetInteractive(false);
			addressButton.SetInteractive(false);
			pageContentImage.sprite = selectedWebsite.PageSprite;
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && selectedWebsite.PageSpriteNight != null)
			{
				pageContentImage.sprite = selectedWebsite.PageSpriteNight;
			}
			RectTransform printButtonTransform = printPageButton.GetComponent<RectTransform>();
			RectTransform targetTransform = ((!isGamedev) ? (printButtonRectParent.GetChild(addressOptionsToggleGroup.SelectionIndex) as RectTransform) : (printButtonGameDevRectParent.GetChild(addressOptionsToggleGroup.SelectionIndex) as RectTransform));
			printButtonTransform.anchoredPosition = targetTransform.anchoredPosition;
			printButtonTransform.sizeDelta = targetTransform.sizeDelta;
			printButtonTransform.localScale = targetTransform.localScale;
			printButtonTransform.localRotation = targetTransform.localRotation;
			pageContentImage.fillAmount = 0f;
			while (pageContentImage.fillAmount < 1f)
			{
				float increment = UnityEngine.Random.Range(0.01f, 0.3f);
				yield return new WaitForSeconds(increment * approximagePageLoadDuration);
				pageContentImage.fillAmount = Mathf.Min(pageContentImage.fillAmount + increment, 1f);
			}
			addressButton.SetInteractive(true);
			printPageButton.SetInteractive(true);
			pageStatusText.text = string.Empty;
			isLoaded = true;
			loadingCoroutine = null;
		}
		else
		{
			isLoaded = false;
			pageStatusText.text = "Loading...";
			printPageButton.SetInteractive(false);
			addressButton.SetInteractive(false);
			pageContentImage.sprite = selectedWebsite.PageSprite;
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && selectedWebsite.PageSpriteNight != null)
			{
				pageContentImage.sprite = selectedWebsite.PageSpriteNight;
			}
			pageContentImage.fillAmount = 0f;
			while (pageContentImage.fillAmount < 1f)
			{
				float increment2 = UnityEngine.Random.Range(0.01f, 0.3f);
				yield return new WaitForSeconds(increment2 * approximagePageLoadDuration);
				pageContentImage.fillAmount = Mathf.Min(pageContentImage.fillAmount + increment2, 1f);
			}
			if (selectedWebsite.PageObject != null)
			{
				selectedWebsite.PageObject.SetActive(true);
				selectedWebsite.PageObject.SendMessage("RevealElements");
			}
			addressButton.SetInteractive(true);
			pageStatusText.text = string.Empty;
			isLoaded = true;
			loadingCoroutine = null;
		}
	}

	private IEnumerator OpenAddressMenuAsync()
	{
		isAddressMenuOpen = true;
		for (int j = 0; j < activeWebsites.Length; j++)
		{
			addressTogglables[j].SetInteractive(false);
		}
		float openness = addressOptionsToggleGroup.transform.localScale.y;
		while (openness < 1f)
		{
			openness += Time.deltaTime * 8f;
			addressOptionsToggleGroup.transform.localScale = new Vector3(1f, openness, 1f);
			yield return null;
		}
		addressOptionsToggleGroup.transform.localScale = Vector3.one;
		for (int i = 0; i < activeWebsites.Length; i++)
		{
			addressTogglables[i].SetInteractive(true);
		}
		addressMenuCoroutine = null;
	}

	private IEnumerator CloseAddressMenuAsync()
	{
		isAddressMenuOpen = false;
		for (int i = 0; i < activeWebsites.Length; i++)
		{
			addressTogglables[i].SetInteractive(false);
		}
		float openness = addressOptionsToggleGroup.transform.localScale.y;
		while (openness > 0f)
		{
			openness -= Time.deltaTime * 8f;
			addressOptionsToggleGroup.transform.localScale = new Vector3(1f, openness, 1f);
			yield return null;
		}
		addressOptionsToggleGroup.transform.localScale = new Vector3(1f, 0f, 1f);
		addressMenuCoroutine = null;
	}

	private void FullyCloseAddressMenu()
	{
		if (addressMenuCoroutine != null)
		{
			StopCoroutine(addressMenuCoroutine);
			addressMenuCoroutine = null;
		}
		for (int i = 0; i < activeWebsites.Length; i++)
		{
			addressTogglables[i].SetInteractive(false);
		}
		addressOptionsToggleGroup.transform.localScale = new Vector3(1f, 0f, 1f);
		isAddressMenuOpen = false;
	}

	private void Update()
	{
		if (!isLoaded)
		{
			return;
		}
		if (hostComputer.IsPrinterBusy)
		{
			pageStatusText.text = "Printing...";
		}
		else if (addressOptionsToggleGroup.SelectionIndex != 4)
		{
			if (printPageButton.IsHighlighted)
			{
				pageStatusText.text = "Print!";
			}
			else
			{
				pageStatusText.text = string.Empty;
			}
		}
		else if (printPageButton.IsHighlighted || websites[4].ExtraPrintButtons[0].PrintButtonClickable.IsHighlighted || websites[4].ExtraPrintButtons[1].PrintButtonClickable.IsHighlighted || websites[4].ExtraPrintButtons[2].PrintButtonClickable.IsHighlighted || websites[4].ExtraPrintButtons[3].PrintButtonClickable.IsHighlighted)
		{
			pageStatusText.text = "Print!";
		}
		else
		{
			pageStatusText.text = string.Empty;
		}
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (!(clickable != null))
		{
			return;
		}
		if (clickable == quitButton)
		{
			Finish();
		}
		else if (clickable == addressButton)
		{
			if (isConnected)
			{
				if (isAddressMenuOpen)
				{
					addressMenuCoroutine = StartCoroutine(CloseAddressMenuAsync());
				}
				else
				{
					addressMenuCoroutine = StartCoroutine(OpenAddressMenuAsync());
				}
			}
		}
		else if (clickable == printPageButton)
		{
			if (!hostComputer.IsPrinterBusy)
			{
				Website website = activeWebsites[addressOptionsToggleGroup.SelectionIndex];
				if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && website.PrefabToPrintNight != null)
				{
					hostComputer.PrintObject(website.PrefabToPrintNight);
				}
				else
				{
					hostComputer.PrintObject(website.PrefabToPrint);
				}
			}
		}
		else
		{
			if (lastLoadedSite == null || lastLoadedSite.ExtraPrintButtons == null)
			{
				return;
			}
			for (int i = 0; i < lastLoadedSite.ExtraPrintButtons.Length; i++)
			{
				if (lastLoadedSite.ExtraPrintButtons[i].PrintButtonClickable == clickable)
				{
					hostComputer.PrintObject(lastLoadedSite.ExtraPrintButtons[i].PrefabToPrint);
				}
			}
		}
	}

	private void OnAddressChanged(ComputerToggleGroup toggleGroup, ComputerTogglable selection, ComputerTogglable prevSelection)
	{
		ClearPage();
		if (toggleGroup.SelectionIndex != -1)
		{
			loadingCoroutine = StartCoroutine(LoadPageAsync(activeWebsites[toggleGroup.SelectionIndex]));
		}
	}
}

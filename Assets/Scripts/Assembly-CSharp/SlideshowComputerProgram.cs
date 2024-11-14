using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowComputerProgram : ComputerProgram
{
	private enum ProgramState
	{
		Setup = 0,
		Briefing = 1,
		ChoosingTheme = 2,
		EditingPages = 3,
		Ready = 4,
		Presenting = 5,
		Complete = 6
	}

	private const int NUM_PAGES = 4;

	private const int NUM_TRANSITIONS = 3;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private GameObject briefingScreen;

	[SerializeField]
	private GameObject themesScreen;

	[SerializeField]
	private GameObject editingScreen;

	[SerializeField]
	private GameObject readyScreen;

	[SerializeField]
	private GameObject presentingScreen;

	[SerializeField]
	private GameObject completeScreen;

	[SerializeField]
	private RectTransform navContainer;

	[SerializeField]
	private RectTransform pagesContainer;

	[SerializeField]
	private ComputerToggleGroup themesToggleGroup;

	[SerializeField]
	private ComputerClickable briefingOkButton;

	[SerializeField]
	private ComputerClickable menuButton;

	[SerializeField]
	private ComputerToggleGroup menuOptionsToggleGroup;

	[SerializeField]
	private ComputerClickable nextButton;

	[SerializeField]
	private ComputerClickable readyGoButton;

	[SerializeField]
	private ComputerClickable quitButton;

	[SerializeField]
	private SlideshowPresentationController presentationController;

	[SerializeField]
	private SlideshowTheme[] themes;

	[SerializeField]
	private SlideshowNavNode navNodePrefab;

	[SerializeField]
	private SlideshowThemeNode themeNodePrefab;

	[SerializeField]
	private SlideshowTransitionController transitionPrefab;

	private ProgramState state;

	private int themeIndex = -1;

	private int editingPageIndex = -1;

	private int editingTransitionIndex = -1;

	private SlideshowPageController[] pages = new SlideshowPageController[4];

	private SlideshowTransitionController[] transitions = new SlideshowTransitionController[3];

	private SlideshowNavNode[] navNodes = new SlideshowNavNode[4];

	private Coroutine menuCoroutine;

	private ComputerTogglable[] menuOptionsButtons;

	private bool isMenuOpen;

	private float targetContentX;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Slideshow;
		}
	}

	private bool isEditingTransition
	{
		get
		{
			return editingTransitionIndex == editingPageIndex;
		}
	}

	private void Awake()
	{
		if (OfficeManager.Instance != null)
		{
			OfficeManager.Instance.SlideshowPresentation.SetComputerProgram(this);
		}
		for (int i = 0; i < 4; i++)
		{
			pages[i] = pagesContainer.GetChild(i).GetComponent<SlideshowPageController>();
		}
		for (int j = 0; j < 3; j++)
		{
			SlideshowTransitionController slideshowTransitionController = Object.Instantiate(transitionPrefab);
			slideshowTransitionController.name = "Transition" + (j + 1);
			slideshowTransitionController.transform.SetParent(pagesContainer, false);
			slideshowTransitionController.transform.SetSiblingIndex(j * 2 + 1);
			transitions[j] = slideshowTransitionController;
		}
		menuOptionsButtons = new ComputerTogglable[menuOptionsToggleGroup.transform.childCount];
		for (int k = 0; k < menuOptionsButtons.Length; k++)
		{
			menuOptionsButtons[k] = menuOptionsToggleGroup.transform.GetChild(k).GetComponent<ComputerTogglable>();
		}
		themesToggleGroup.SelectionChanged += ThemeChanged;
		menuOptionsToggleGroup.SelectionChanged += MenuOptionChanged;
	}

	private void OnEnable()
	{
		if (state == ProgramState.Setup)
		{
			for (int i = 0; i < 4; i++)
			{
				SlideshowNavNode slideshowNavNode = Object.Instantiate(navNodePrefab);
				slideshowNavNode.name = navNodePrefab.name + "_" + (i + 1);
				slideshowNavNode.transform.SetParent(navContainer, false);
				slideshowNavNode.transform.localScale = Vector3.one;
				slideshowNavNode.gameObject.SetActive(true);
				navNodes[i] = slideshowNavNode;
			}
			for (int j = 0; j < themes.Length; j++)
			{
				SlideshowThemeNode slideshowThemeNode = Object.Instantiate(themeNodePrefab);
				slideshowThemeNode.name = themeNodePrefab.name + "_" + (j + 1);
				slideshowThemeNode.transform.SetParent(themesToggleGroup.transform, false);
				slideshowThemeNode.transform.localScale = Vector3.one;
				slideshowThemeNode.gameObject.SetActive(true);
				slideshowThemeNode.SetTheme(themes[j]);
			}
			themesToggleGroup.RefreshChoices();
			briefingScreen.SetActive(true);
			state = ProgramState.Briefing;
		}
		else if (state == ProgramState.EditingPages)
		{
			FullyCloseMenu();
			menuButton.SetInteractive(true);
			if (isEditingTransition)
			{
				pagesContainer.anchoredPosition = new Vector2(0f - (transitions[editingTransitionIndex].transform as RectTransform).anchoredPosition.x, 0f);
			}
			else
			{
				pagesContainer.anchoredPosition = new Vector2(0f - (pages[editingPageIndex].transform as RectTransform).anchoredPosition.x, 0f);
				RefreshNavNodes();
				RefreshPageThumbnail();
			}
			nextButton.SetInteractive(menuOptionsToggleGroup.SelectionIndex != -1);
		}
	}

	private void Update()
	{
	}

	private void ThemeChanged(ComputerToggleGroup toggleGroup, ComputerTogglable selection, ComputerTogglable prevSelection)
	{
		themeIndex = toggleGroup.SelectionIndex;
		if (themeIndex != -1)
		{
			nextButton.SetInteractive(true);
		}
	}

	private void MenuOptionChanged(ComputerToggleGroup toggleGroup, ComputerTogglable selection, ComputerTogglable prevSelection)
	{
		int selectionIndex = toggleGroup.SelectionIndex;
		if (selectionIndex != -1)
		{
			SelectMenuOption(selectionIndex);
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
		if (state == ProgramState.Briefing)
		{
			if (clickable == briefingOkButton)
			{
				briefingScreen.SetActive(false);
				themesScreen.SetActive(true);
				state = ProgramState.ChoosingTheme;
			}
		}
		else if (state == ProgramState.ChoosingTheme)
		{
			if (clickable == nextButton)
			{
				StartEditingPages();
			}
		}
		else if (state == ProgramState.EditingPages)
		{
			if (menuCoroutine != null)
			{
				return;
			}
			if (clickable == menuButton)
			{
				if (isMenuOpen)
				{
					menuCoroutine = StartCoroutine(CloseMenuAsync());
				}
				else
				{
					menuCoroutine = StartCoroutine(OpenMenuAsync());
				}
			}
			else if (clickable == nextButton)
			{
				EditNext();
			}
		}
		else if (state == ProgramState.Ready && clickable == readyGoButton)
		{
			StartPresentation();
		}
	}

	private void StartEditingPages()
	{
		for (int i = 0; i < 4; i++)
		{
			pages[i].SetTheme(themes[themeIndex]);
		}
		themesScreen.SetActive(false);
		editingScreen.SetActive(true);
		state = ProgramState.EditingPages;
		StartCoroutine(EditNextPageAsync());
	}

	private void SelectMenuOption(int optionIndex)
	{
		nextButton.SetInteractive(true);
		if (isEditingTransition)
		{
			menuButton.Text.text = transitions[editingTransitionIndex].OptionNames[optionIndex];
			transitions[editingTransitionIndex].ShowOption(optionIndex);
		}
		else
		{
			menuButton.Text.text = pages[editingPageIndex].OptionNames[optionIndex];
			pages[editingPageIndex].ShowOption(optionIndex);
		}
		StartCoroutine(CloseMenuAsync());
	}

	private void EditNext()
	{
		if (isEditingTransition)
		{
			StartCoroutine(EditNextPageAsync());
		}
		else if (editingPageIndex == 3)
		{
			nextButton.SetInteractive(false);
			editingScreen.SetActive(false);
			readyScreen.SetActive(true);
			state = ProgramState.Ready;
		}
		else
		{
			StartCoroutine(EditNextTransitionAsync());
		}
	}

	private void StartPresentation()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
		readyScreen.SetActive(false);
		presentingScreen.SetActive(true);
		state = ProgramState.Presenting;
		if (OfficeManager.Instance != null && OfficeManager.Instance.SlideshowPresentation != null)
		{
			OfficeManager.Instance.SlideshowPresentation.LoadPresentation(pagesContainer);
			OfficeManager.Instance.SlideshowPresentation.StartPresentation();
		}
		else
		{
			Debug.LogError("Could not find slideshow presentation.");
		}
	}

	public void EndPresentation()
	{
		state = ProgramState.Complete;
		presentingScreen.SetActive(false);
		completeScreen.SetActive(true);
	}

	private IEnumerator EditNextPageAsync()
	{
		nextButton.SetInteractive(false);
		menuButton.SetInteractive(false);
		yield return null;
		int newEditingPageIndex = editingPageIndex + 1;
		SlideshowPageController page = pages[newEditingPageIndex];
		string[] pageOptionNames = page.OptionNames;
		float targetX = 0f - (page.transform as RectTransform).anchoredPosition.x;
		while (true)
		{
			Vector2 scrollPos = pagesContainer.anchoredPosition;
			float delta = targetX - scrollPos.x;
			if (Mathf.Abs(delta) <= 2f)
			{
				break;
			}
			scrollPos.x += delta * Time.deltaTime * 8f;
			pagesContainer.anchoredPosition = scrollPos;
			yield return null;
		}
		pagesContainer.anchoredPosition = new Vector2(targetX, 0f);
		menuButton.SetInteractive(true);
		menuButton.Text.text = "Select " + page.name + "...";
		menuOptionsToggleGroup.ClearSelection();
		for (int i = 0; i < menuOptionsButtons.Length; i++)
		{
			ComputerTogglable button = menuOptionsButtons[i];
			if (i < pageOptionNames.Length)
			{
				button.SetInteractive(true);
				button.Text.text = pageOptionNames[i];
			}
			else
			{
				button.SetInteractive(false);
				button.Text.text = "---";
			}
		}
		editingPageIndex = newEditingPageIndex;
		RefreshNavNodes();
		RefreshPageThumbnail();
	}

	private IEnumerator EditNextTransitionAsync()
	{
		nextButton.SetInteractive(false);
		menuButton.SetInteractive(false);
		yield return null;
		int newEditingTransitionIndex = editingTransitionIndex + 1;
		SlideshowTransitionController transition = transitions[newEditingTransitionIndex];
		string[] transitionOptionNames = transition.OptionNames;
		float targetX = 0f - (transition.transform as RectTransform).anchoredPosition.x;
		while (true)
		{
			Vector2 scrollPos = pagesContainer.anchoredPosition;
			float delta = targetX - scrollPos.x;
			if (Mathf.Abs(delta) <= 2f)
			{
				break;
			}
			scrollPos.x += delta * Time.deltaTime * 8f;
			pagesContainer.anchoredPosition = scrollPos;
			yield return null;
		}
		pagesContainer.anchoredPosition = new Vector2(targetX, 0f);
		menuButton.SetInteractive(true);
		menuButton.Text.text = "Select Transition...";
		menuOptionsToggleGroup.ClearSelection();
		for (int i = 0; i < menuOptionsButtons.Length; i++)
		{
			ComputerTogglable button = menuOptionsButtons[i];
			if (i < transitionOptionNames.Length)
			{
				button.SetInteractive(true);
				button.Text.text = transitionOptionNames[i];
			}
			else
			{
				button.SetInteractive(false);
				button.Text.text = "---";
			}
		}
		editingTransitionIndex = newEditingTransitionIndex;
	}

	private IEnumerator OpenMenuAsync()
	{
		isMenuOpen = true;
		for (int j = 0; j < menuOptionsButtons.Length; j++)
		{
			menuOptionsButtons[j].SetInteractive(false);
		}
		float openness = menuOptionsToggleGroup.transform.localScale.y;
		while (openness < 1f)
		{
			openness += Time.deltaTime * 8f;
			menuOptionsToggleGroup.transform.localScale = new Vector3(1f, openness, 1f);
			yield return null;
		}
		menuOptionsToggleGroup.transform.localScale = Vector3.one;
		for (int i = 0; i < menuOptionsButtons.Length; i++)
		{
			menuOptionsButtons[i].SetInteractive(true);
		}
		menuCoroutine = null;
	}

	private IEnumerator CloseMenuAsync()
	{
		isMenuOpen = false;
		for (int i = 0; i < menuOptionsButtons.Length; i++)
		{
			menuOptionsButtons[i].SetInteractive(false);
		}
		float openness = menuOptionsToggleGroup.transform.localScale.y;
		while (openness > 0f)
		{
			openness -= Time.deltaTime * 8f;
			menuOptionsToggleGroup.transform.localScale = new Vector3(1f, openness, 1f);
			yield return null;
		}
		menuOptionsToggleGroup.transform.localScale = new Vector3(1f, 0f, 1f);
		if (!isEditingTransition)
		{
			RefreshPageThumbnail();
		}
		menuCoroutine = null;
	}

	private void RefreshPageThumbnail()
	{
		RawImage thumbnail = navNodes[editingPageIndex].Thumbnail;
		if (thumbnail.texture != null)
		{
			Object.Destroy(thumbnail.texture);
		}
		thumbnail.texture = hostComputer.TakeScreenshot(pages[editingPageIndex].transform as RectTransform);
	}

	private void RefreshNavNodes()
	{
		for (int i = 0; i < 4; i++)
		{
			SlideshowNavNode slideshowNavNode = navNodes[i];
			slideshowNavNode.SetState(i <= editingPageIndex, i == editingPageIndex);
		}
	}

	private void FullyCloseMenu()
	{
		if (menuCoroutine != null)
		{
			StopCoroutine(menuCoroutine);
			menuCoroutine = null;
		}
		for (int i = 0; i < menuOptionsButtons.Length; i++)
		{
			menuOptionsButtons[i].SetInteractive(false);
		}
		menuOptionsToggleGroup.transform.localScale = new Vector3(1f, 0f, 1f);
		isMenuOpen = false;
	}
}

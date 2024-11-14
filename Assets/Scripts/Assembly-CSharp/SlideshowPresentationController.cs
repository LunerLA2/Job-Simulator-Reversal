using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowPresentationController : MonoBehaviour
{
	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private Camera presentationCamera;

	[SerializeField]
	private Canvas presentationCanvas;

	[SerializeField]
	private MeshRenderer projection;

	[SerializeField]
	private RectTransform contentPane;

	[SerializeField]
	private GameObject startScreen;

	[SerializeField]
	private GameObject instructionsScreen;

	[SerializeField]
	private Transform audioSource;

	[SerializeField]
	private Transform transitionRoot;

	[SerializeField]
	private Image timer;

	[SerializeField]
	private float pageTimeout = 1f;

	[SerializeField]
	private float defaultTransitionDuration = 1f;

	private SlideshowComputerProgram computerProgram;

	private RectTransform content;

	private bool isPresentationLoaded;

	private List<SlideshowPageController> pages = new List<SlideshowPageController>();

	private List<SlideshowTransitionController> transitions = new List<SlideshowTransitionController>();

	private int pageIndex = -1;

	private AnimationCurve transitionSlideCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f));

	private SlideshowClickerController connectedClicker;

	private int numPages
	{
		get
		{
			return pages.Count;
		}
	}

	private int numTransitions
	{
		get
		{
			return transitions.Count;
		}
	}

	public void SetComputerProgram(SlideshowComputerProgram cp)
	{
		computerProgram = cp;
	}

	public void LoadPresentation(RectTransform contentTemplate)
	{
		if (isPresentationLoaded)
		{
			UnloadPresentation();
		}
		Debug.Log("Load Presentation");
		content = Object.Instantiate(contentTemplate.gameObject).transform as RectTransform;
		content.name = "Content";
		content.SetParent(contentPane, false);
		content.SetSiblingIndex(0);
		content.localScale = Vector3.one;
		content.anchorMin = new Vector2(0.5f, 0f);
		content.anchorMax = new Vector2(0.5f, 1f);
		for (int i = 0; i < content.childCount; i++)
		{
			SlideshowPageController component = content.GetChild(i).GetComponent<SlideshowPageController>();
			if (component != null)
			{
				pages.Add(component);
				if (i == 0)
				{
					content.anchoredPosition = new Vector2(0f - component.GetComponent<RectTransform>().anchoredPosition.x, 0f);
				}
				continue;
			}
			SlideshowTransitionController component2 = content.GetChild(i).GetComponent<SlideshowTransitionController>();
			if (component2 != null)
			{
				transitions.Add(component2);
				component2.gameObject.SetActive(false);
			}
		}
		presentationCamera.enabled = false;
		isPresentationLoaded = true;
		projection.enabled = true;
		startScreen.SetActive(true);
		instructionsScreen.SetActive(false);
	}

	private void Update()
	{
		if (isPresentationLoaded && Time.frameCount % 3 == 0)
		{
			presentationCamera.Render();
		}
	}

	public void UnloadPresentation()
	{
		Debug.Log("Unload Presentation");
		StopAllCoroutines();
		presentationCamera.enabled = false;
		pages.Clear();
		transitions.Clear();
		Object.Destroy(content.gameObject);
		content = null;
		pageIndex = -1;
		isPresentationLoaded = false;
		projection.enabled = false;
	}

	public void StartPresentation()
	{
		Debug.Log("Start Presentation");
		OfficeManager.Instance.ShowProjector();
		StartCoroutine(PresentAsync());
	}

	private IEnumerator PresentAsync()
	{
		while (!(connectedClicker != null) || !connectedClicker.Clicked)
		{
			yield return null;
		}
		startScreen.SetActive(false);
		for (pageIndex = 0; pageIndex < pages.Count; pageIndex++)
		{
			float pageTime = 0f;
			timer.enabled = true;
			while (pageTime < pageTimeout)
			{
				pageTime += Time.deltaTime;
				timer.fillAmount = Mathf.Clamp01(1f - pageTime / pageTimeout);
				yield return null;
			}
			timer.enabled = false;
			if (pageIndex == pages.Count - 1)
			{
				break;
			}
			while (!(connectedClicker != null) || !connectedClicker.Clicked)
			{
				yield return null;
			}
			SlideshowTransitionController transition = transitions[pageIndex];
			Transform transitionEffect = transitionRoot.GetChild(Mathf.Max(0, transition.OptionIndex));
			transitionEffect.gameObject.SetActive(true);
			AudioSourceHelper transitionAudio = transitionEffect.GetComponent<AudioSourceHelper>();
			Animation transitionAnimation = transitionEffect.GetComponent<Animation>();
			float transitionDuration = ((!(transitionAnimation != null)) ? defaultTransitionDuration : transitionAnimation.clip.length);
			float transitionTime = 0f;
			if (transitionAudio != null)
			{
				transitionAudio.Play();
			}
			if (transitionAnimation != null)
			{
				transitionAnimation.Play();
			}
			float startingX = content.anchoredPosition.x;
			float targetX = 0f - pages[pageIndex + 1].GetComponent<RectTransform>().anchoredPosition.x;
			while (transitionTime < transitionDuration)
			{
				transitionTime += Time.deltaTime;
				content.anchoredPosition = new Vector2(Mathf.Lerp(startingX, targetX, transitionSlideCurve.Evaluate(transitionTime / transitionDuration)), 0f);
				yield return null;
			}
			content.anchoredPosition = new Vector2(targetX, 0f);
			transitionEffect.gameObject.SetActive(false);
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
		computerProgram.EndPresentation();
	}

	public void ConnectClicker(SlideshowClickerController clicker)
	{
		Debug.Log("Connect Clicker");
		connectedClicker = clicker;
		instructionsScreen.SetActive(true);
	}
}

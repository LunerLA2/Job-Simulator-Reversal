using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

namespace SiliconTrail
{
	public class SiliconTrailComputerProgram : ComputerProgram
	{
		private const float INFO_CHARS_PER_SECOND = 120f;

		private const float KEY_PRESS_TIMEOUT = 0.2f;

		[SerializeField]
		private TrailManager trail;

		[SerializeField]
		private Animation travelAnimation;

		[SerializeField]
		private Transform graphicsRoot;

		[SerializeField]
		private GameObject infoPanel;

		[SerializeField]
		private Text daysText;

		[SerializeField]
		private Text moneyText;

		[SerializeField]
		private Text foodText;

		[SerializeField]
		private Text healthText;

		[SerializeField]
		private Text infoText;

		[SerializeField]
		private GameObject choicePanel;

		[SerializeField]
		private GameObject defaultOptionPanel;

		[SerializeField]
		private Text defaultOptionText;

		[SerializeField]
		private GameObject alternateOptionPanel;

		[SerializeField]
		private Text alternateOptionText;

		[SerializeField]
		private Transform cinematicsRoot;

		[SerializeField]
		private SurfingMinigame surfingMinigame;

		[SerializeField]
		private CanvasGroup shade;

		[SerializeField]
		private AudioClip infoCharSound;

		[SerializeField]
		private WorldItemData worldItemData;

		private int timesUsed;

		private Campaign campaign = new Campaign();

		private Task currentTask;

		private Queue<Task> upcomingTasks = new Queue<Task>();

		private Image activeGraphic;

		private Cinematic activeCinematic;

		private float trailProgress;

		private Coroutine showInfoCoroutine;

		private Image[] graphics;

		private Cinematic[] cinematics;

		private float lastKeyPressTime;

		public string DataUnit
		{
			get
			{
				return "kB";
			}
		}

		public override ComputerProgramID ProgramID
		{
			get
			{
				return ComputerProgramID.SiliconTrail;
			}
		}

		public Campaign Campaign
		{
			get
			{
				return campaign;
			}
		}

		public WorldItemData WorldItemData
		{
			get
			{
				return worldItemData;
			}
		}

		private void Awake()
		{
			graphics = new Image[graphicsRoot.childCount];
			for (int i = 0; i < graphicsRoot.childCount; i++)
			{
				graphics[i] = graphicsRoot.GetChild(i).GetComponent<Image>();
			}
			cinematics = new Cinematic[cinematicsRoot.childCount];
			for (int j = 0; j < cinematicsRoot.childCount; j++)
			{
				cinematics[j] = cinematicsRoot.GetChild(j).GetComponent<Cinematic>();
			}
		}

		private void StartCampaign()
		{
			trail.PopulatePlaces(campaign.Places);
			StartCoroutine(ProcessTasksAsync());
			QueueTask(new CinematicTask
			{
				CinematicName = "Title",
				SkipKeyCode = "1"
			});
			QueueTask(new PlaceTask());
		}

		private void OnEnable()
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
			hostComputer.HideCursor();
			Reset();
			StartCampaign();
		}

		private void OnDisable()
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
			Reset();
			hostComputer.ShowCursor();
		}

		private void Reset()
		{
			StopAllCoroutines();
			HideAll();
			campaign.Reset();
			trail.Reset();
			ClearTaskQueue();
			currentTask = null;
		}

		public void Restart()
		{
			Reset();
			StartCampaign();
		}

		public void HideAll()
		{
			trail.gameObject.SetActive(false);
			infoPanel.SetActive(false);
			if (activeGraphic != null)
			{
				activeGraphic.gameObject.SetActive(false);
				activeGraphic = null;
			}
			if (activeCinematic != null)
			{
				activeCinematic.gameObject.SetActive(false);
				activeCinematic = null;
			}
			choicePanel.SetActive(false);
			surfingMinigame.gameObject.SetActive(false);
		}

		public void UpdateStatsDisplay()
		{
			daysText.text = campaign.DaysLeft.ToString();
			moneyText.text = campaign.Money + " " + DataUnit;
			foodText.text = campaign.Food + " " + DataUnit;
			healthText.text = campaign.CalculatePartyHealth().ToString();
		}

		public void ShowTrail()
		{
			trail.gameObject.SetActive(true);
		}

		public void SetTrailProgress(float progress)
		{
			trail.SetProgress(progress);
		}

		public void ShowInfo(string text)
		{
			if (showInfoCoroutine != null)
			{
				StopCoroutine(showInfoCoroutine);
				showInfoCoroutine = null;
			}
			showInfoCoroutine = StartCoroutine(ShowInfoAsync(text));
		}

		public void UsedWID()
		{
			timesUsed++;
			if (timesUsed >= 3)
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
				timesUsed = 0;
			}
		}

		private IEnumerator ShowInfoAsync(string text)
		{
			infoPanel.SetActive(true);
			int numChars = 0;
			float numCharsSmooth = 0f;
			infoText.text = string.Empty;
			while (numCharsSmooth < (float)text.Length)
			{
				numCharsSmooth += 6f;
				int newNumChars = Mathf.Min((int)numCharsSmooth, text.Length);
				if (newNumChars != numChars)
				{
					numChars = newNumChars;
					infoText.text = text.Substring(0, numChars) + "<color=#0000>" + text.Substring(numChars) + "</color>";
					hostComputer.PlaySound(infoCharSound);
				}
				yield return new WaitForSeconds(0.05f);
			}
			infoText.text = text;
		}

		public void ShowChoice(string defaultText, string alternateText = null)
		{
			choicePanel.SetActive(true);
			defaultOptionPanel.SetActive(true);
			defaultOptionText.text = defaultText;
			if (!string.IsNullOrEmpty(alternateText))
			{
				alternateOptionPanel.SetActive(true);
				alternateOptionText.text = alternateText;
			}
			else
			{
				alternateOptionPanel.SetActive(false);
			}
		}

		public void ShowGraphic(string graphicName)
		{
			for (int i = 0; i < graphics.Length; i++)
			{
				Image image = graphics[i];
				if (image.gameObject.name == graphicName)
				{
					if (activeGraphic != null)
					{
						activeGraphic.gameObject.SetActive(false);
					}
					image.gameObject.SetActive(true);
					activeGraphic = image;
				}
			}
		}

		public Cinematic ShowCinematic(string cinematicName)
		{
			for (int i = 0; i < cinematics.Length; i++)
			{
				Cinematic cinematic = cinematics[i];
				if (cinematic.gameObject.name == cinematicName)
				{
					if (activeCinematic != null)
					{
						activeCinematic.gameObject.SetActive(false);
					}
					cinematic.gameObject.SetActive(true);
					activeCinematic = cinematic;
					return cinematic;
				}
			}
			return null;
		}

		public SurfingMinigame ShowSurfingMinigame()
		{
			surfingMinigame.gameObject.SetActive(true);
			return surfingMinigame;
		}

		public void QueueTask(Task task)
		{
			upcomingTasks.Enqueue(task);
		}

		public void QueueTasks(IEnumerable<Task> tasks)
		{
			foreach (Task task in tasks)
			{
				QueueTask(task);
			}
		}

		public void ClearTaskQueue()
		{
			upcomingTasks.Clear();
		}

		private IEnumerator ProcessTasksAsync()
		{
			while (true)
			{
				if (currentTask == null)
				{
					if (upcomingTasks.Count <= 0)
					{
						yield return null;
						continue;
					}
					currentTask = upcomingTasks.Dequeue();
				}
				yield return currentTask.Execute(this);
				UpdateStatsDisplay();
				currentTask = null;
			}
		}

		protected override bool OnKeyPress(string code)
		{
			if (Time.time - lastKeyPressTime > 0.2f)
			{
				lastKeyPressTime = Time.time;
				if (currentTask != null)
				{
					currentTask.OnKeyPress(code);
				}
			}
			return true;
		}

		protected override bool OnMouseMove(Vector2 cursorPos)
		{
			if (currentTask != null)
			{
				currentTask.OnMouseMove(cursorPos);
			}
			return true;
		}

		protected override bool OnMouseClick(Vector2 cursorPos)
		{
			if (currentTask != null)
			{
				currentTask.OnMouseClick(cursorPos);
			}
			return true;
		}
	}
}

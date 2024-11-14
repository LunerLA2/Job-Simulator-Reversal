using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class MailComputerProgram : ComputerProgram
{
	private const int MAX_EMAILS = 65535;

	[SerializeField]
	private int numVisibleEmails = 11;

	[SerializeField]
	private float emailScrollSpeed = 11f;

	[SerializeField]
	private float fetchDuration = 3f;

	[SerializeField]
	private RawImage emailsImage;

	[SerializeField]
	private Transform counter;

	[SerializeField]
	private Text counterNumber;

	[SerializeField]
	private ComputerClickable deleteClickable;

	[SerializeField]
	private AudioClip fetchingSound;

	[SerializeField]
	private AudioClip alertSound;

	[SerializeField]
	private AudioClip deleteSound;

	[SerializeField]
	private WorldItemData worldItemData;

	private bool cleared;

	private int numEmails;

	private float emailScrollIndex;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Mail;
		}
	}

	private void Awake()
	{
	}

	private void OnEnable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
		if (!cleared)
		{
			FetchMail();
		}
		else
		{
			ResetInbox();
		}
	}

	private void OnDisable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
		hostComputer.StopSound(fetchingSound);
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable.name == "X")
			{
				Finish();
			}
			else if (clickable.name == "Delete")
			{
				DeleteAllMail();
			}
		}
	}

	private void FetchMail()
	{
		StopAllCoroutines();
		StartCoroutine(FetchMailAsync());
	}

	private void ResetInbox()
	{
		numEmails = 0;
		emailScrollIndex = 0f;
		emailsImage.enabled = !cleared;
		RefreshDeleteButton();
		RefreshCounterNumber();
		RefreshEmailTable();
	}

	private IEnumerator FetchMailAsync()
	{
		ResetInbox();
		hostComputer.PlaySound(fetchingSound, 1f, 1f, true);
		float fetchTime = 0f;
		while (fetchTime < fetchDuration)
		{
			yield return null;
			fetchTime += Time.deltaTime;
			emailScrollIndex += Time.deltaTime * emailScrollSpeed;
			float fetchRatio = Mathf.Clamp01(fetchTime / fetchDuration);
			numEmails = (int)(fetchRatio * 65535f);
			RefreshCounterNumber();
			RefreshEmailTable();
		}
		hostComputer.StopSound(fetchingSound);
		hostComputer.PlaySound(alertSound);
		RefreshDeleteButton();
	}

	private void DeleteAllMail()
	{
		StopAllCoroutines();
		cleared = true;
		if (numEmails > 0)
		{
			ResetInbox();
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "DESTROYED");
			hostComputer.PlaySound(deleteSound);
		}
	}

	private void RefreshDeleteButton()
	{
		deleteClickable.SetInteractive(numEmails == 65535);
	}

	private void RefreshCounterNumber()
	{
		counterNumber.text = numEmails.ToString();
	}

	private void RefreshEmailTable()
	{
		emailsImage.uvRect = new Rect(0f, (0f - Mathf.Round(emailScrollIndex)) / (float)numVisibleEmails, 1f, 1f);
	}
}

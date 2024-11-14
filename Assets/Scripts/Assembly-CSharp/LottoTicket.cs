using System;
using OwlchemyVR;
using UnityEngine;

public class LottoTicket : MonoBehaviour
{
	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	[SerializeField]
	private ScratcherController scratcherController;

	[SerializeField]
	private SpriteRenderer slot1;

	[SerializeField]
	private SpriteRenderer slot2;

	[SerializeField]
	private SpriteRenderer slot3;

	[SerializeField]
	private Sprite[] possibleIconPool;

	private bool winner;

	private WorldItem worldItem;

	[SerializeField]
	private WorldItemData lottoTicketWinner;

	[SerializeField]
	private WorldItemData lottoTicketLoser;

	[SerializeField]
	private ParticleSystem winnerEffect;

	[SerializeField]
	private AudioSourceHelper winnerSound;

	private void Awake()
	{
		worldItem = GetComponent<WorldItem>();
		slot1.sprite = possibleIconPool[UnityEngine.Random.Range(0, possibleIconPool.Length)];
		slot2.sprite = possibleIconPool[UnityEngine.Random.Range(0, possibleIconPool.Length)];
		slot3.sprite = possibleIconPool[UnityEngine.Random.Range(0, possibleIconPool.Length)];
		if (slot1.sprite == slot2.sprite && slot1.sprite == slot3.sprite)
		{
			winner = true;
			Debug.Log("WinnerLottoTicketSpawned");
		}
	}

	private void OnEnable()
	{
		ScratcherController obj = scratcherController;
		obj.OnScratchOffCompleted = (Action)Delegate.Combine(obj.OnScratchOffCompleted, new Action(ScratchOffComplete));
	}

	private void OnDisable()
	{
		ScratcherController obj = scratcherController;
		obj.OnScratchOffCompleted = (Action)Delegate.Remove(obj.OnScratchOffCompleted, new Action(ScratchOffComplete));
	}

	private void ScratchOffComplete()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
		if (winner)
		{
			worldItem.ManualSetData(lottoTicketWinner);
			winnerEffect.Play();
			winnerSound.Play();
			AchievementManager.CompleteAchievement(9);
		}
		else
		{
			worldItem.ManualSetData(lottoTicketLoser);
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
	}

	public void Init(Mesh labelMesh)
	{
		meshFilter.mesh = labelMesh;
		outline.ForceRefreshMeshes();
	}
}

using System;
using UnityEngine;

public class RefrigeratorController : MonoBehaviour
{
	[SerializeField]
	private GameObject door;

	[SerializeField]
	private Animation doorAnimation;

	[SerializeField]
	private AnimationClip doorDownAnim;

	[SerializeField]
	private AnimationClip doorUpAnim;

	[SerializeField]
	private AudioClip doorOpenCloseAudioClip;

	[SerializeField]
	private GameObject spawnerContainer;

	[SerializeField]
	private PlayerPartDetector playerPartDetector;

	private ItemRespawner[] itemRespawners;

	private void Awake()
	{
		itemRespawners = spawnerContainer.GetComponentsInChildren<ItemRespawner>();
	}

	private void OnEnable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnFirstPartEntered = (Action<PlayerPartDetector>)Delegate.Combine(obj.OnFirstPartEntered, new Action<PlayerPartDetector>(PlayerEnteredTrigger));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnLastPartExited = (Action<PlayerPartDetector>)Delegate.Combine(obj2.OnLastPartExited, new Action<PlayerPartDetector>(PlayerExitedTrigger));
	}

	private void OnDisable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnFirstPartEntered = (Action<PlayerPartDetector>)Delegate.Remove(obj.OnFirstPartEntered, new Action<PlayerPartDetector>(PlayerEnteredTrigger));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnLastPartExited = (Action<PlayerPartDetector>)Delegate.Remove(obj2.OnLastPartExited, new Action<PlayerPartDetector>(PlayerExitedTrigger));
	}

	private void PlayerEnteredTrigger(PlayerPartDetector ppd)
	{
		SlideDoor(true);
	}

	private void PlayerExitedTrigger(PlayerPartDetector ppd)
	{
		SlideDoor(false);
	}

	private void SlideDoor(bool down)
	{
		doorAnimation.clip = ((!down) ? doorUpAnim : doorDownAnim);
		doorAnimation.Play();
		AudioManager.Instance.Play(door.transform, doorOpenCloseAudioClip, 1f, 1f);
		if (!down)
		{
			for (int i = 0; i < itemRespawners.Length; i++)
			{
				itemRespawners[i].RespawnIfRequired(0.4f);
			}
		}
	}
}

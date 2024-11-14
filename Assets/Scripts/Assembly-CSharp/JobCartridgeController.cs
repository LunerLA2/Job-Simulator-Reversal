using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class JobCartridgeController : MonoBehaviour
{
	private const string enterAnimation = "JobCartridgeCartEnter";

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float delay = 2f;

	[SerializeField]
	private float lengthOfIntroAnimation;

	[SerializeField]
	private SubtaskData associatedSubtask;

	[SerializeField]
	private TerminalUIController terminalUIController;

	[SerializeField]
	private BasePrefabSpawner[] cartridgeSpawners;

	[SerializeField]
	private MonoBehaviourPrefabSpawner overtimeGenie;

	private Rigidbody overtimeGenieRB;

	[SerializeField]
	private GameObject normalCart;

	[SerializeField]
	private GameObject upgradedCart;

	[SerializeField]
	private GameObject psvrNormalCart;

	[SerializeField]
	private GameObject psvrUpgradedCart;

	[SerializeField]
	private ParticleSystem upgradeParticle;

	[SerializeField]
	private AudioClip upgradecartsound;

	[SerializeField]
	private GameObject[] geniesToDisableOnPSVR;

	[SerializeField]
	private GameObject[] geniesToDisableOnOculus;

	private bool hasEntered;

	private void Awake()
	{
		if (!GlobalStorage.Instance.GameStateData.HasSeenGameComplete)
		{
			SetCartUpgradedState(false);
		}
		else
		{
			SwitchToUpgradedCart(0f, true);
		}
		for (int i = 0; i < cartridgeSpawners.Length; i++)
		{
			cartridgeSpawners[i].SpawnPrefab();
			GameObject lastSpawnedPrefabGO = cartridgeSpawners[i].LastSpawnedPrefabGO;
			if (lastSpawnedPrefabGO != null)
			{
				lastSpawnedPrefabGO.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		if (overtimeGenieRB != null)
		{
			overtimeGenieRB.isKinematic = true;
		}
		if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
		{
			for (int j = 0; j < geniesToDisableOnOculus.Length; j++)
			{
				geniesToDisableOnOculus[j].SetActive(false);
			}
		}
	}

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
	}

	private void SetCartUpgradedState(bool state)
	{
		normalCart.SetActive(!state);
		upgradedCart.SetActive(state);
		psvrNormalCart.SetActive(false);
		psvrUpgradedCart.SetActive(false);
	}

	private void SubtaskCompleted(SubtaskStatusController subtask)
	{
		if (subtask.Data == associatedSubtask)
		{
			PlayEnterAnim(delay);
		}
	}

	public void PlayEnterAnim(float aDelay)
	{
		if (!hasEntered)
		{
			hasEntered = true;
			StartCoroutine(PlayEnterAnimRoutine(aDelay));
		}
	}

	private IEnumerator PlayEnterAnimRoutine(float aDelay)
	{
		yield return new WaitForSeconds(aDelay);
		terminalUIController.PowerOffTerminal();
		animator.Play("JobCartridgeCartEnter");
		yield return new WaitForSeconds(lengthOfIntroAnimation);
		for (int i = 0; i < cartridgeSpawners.Length; i++)
		{
			GameObject go = cartridgeSpawners[i].LastSpawnedPrefabGO;
			if (go != null)
			{
				Rigidbody rb = go.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.isKinematic = false;
				}
			}
		}
		if (overtimeGenieRB != null)
		{
			overtimeGenieRB.isKinematic = false;
		}
	}

	public void SwitchToUpgradedCart(float delay = 0f, bool isInitial = false)
	{
		StartCoroutine(WaitAndSwitchCarts(delay, isInitial));
	}

	private IEnumerator WaitAndSwitchCarts(float delay, bool isInitial)
	{
		yield return new WaitForSeconds(delay);
		if (!isInitial && upgradecartsound != null)
		{
			AudioManager.Instance.Play(base.transform.position, upgradecartsound, 1f, 1f);
		}
		upgradeParticle.Play();
		SetCartUpgradedState(true);
		Debug.Log("upgraded cart GO");
	}
}

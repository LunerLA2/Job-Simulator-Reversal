using OwlchemyVR;
using UnityEngine;

public static class GenieManager
{
	public const float NO_GRAVITY_RIGIDBODY_DRAG = 1.5f;

	public const float NO_GRAVITY_RIGIDBODY_ANGULAR_DRAG = 0.5f;

	public const float NO_GRAVITY_GRAVITY = -1E-05f;

	public const float NO_GRAVITY_FLUID_POUR_MULTIPLIER = 1f;

	private static PhysicMaterial rubberModePhysicMaterial;

	public static void Init()
	{
		Debug.Log("Init with: " + GlobalStorage.Instance.CurrentGenieModes);
		if (IsNoGravityEnabled())
		{
			Physics.gravity = new Vector3(0f, -1E-05f, 0f);
		}
		else
		{
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
		}
		if (IsDollhouseModeEnabled())
		{
			float num = 3f;
			GlobalStorage.Instance.MasterHMDAndInputController.transform.localScale = new Vector3(num, num, num);
			GlobalStorage.Instance.MasterHMDAndInputController.transform.position = new Vector3(0f, -0.42f, 0f);
			MasterHMDAndInputController masterHMDAndInputController = GlobalStorage.Instance.MasterHMDAndInputController;
			for (int i = 0; i < masterHMDAndInputController.InteractionHandControllers.Count; i++)
			{
				masterHMDAndInputController.InteractionHandControllers[i].transform.localScale = new Vector3(1f / num, 1f / num, 1f / num);
			}
		}
		else if (IsShortModeEnabled())
		{
			float num2 = 1.25f;
			GlobalStorage.Instance.MasterHMDAndInputController.transform.localScale = new Vector3(num2, num2, num2);
			GlobalStorage.Instance.MasterHMDAndInputController.transform.position = new Vector3(0f, 0f, 0f);
			MasterHMDAndInputController masterHMDAndInputController2 = GlobalStorage.Instance.MasterHMDAndInputController;
			for (int j = 0; j < masterHMDAndInputController2.InteractionHandControllers.Count; j++)
			{
				masterHMDAndInputController2.InteractionHandControllers[j].transform.localScale = new Vector3(1f / num2, 1f / num2, 1f / num2);
			}
		}
		else if (GlobalStorage.Instance.MasterHMDAndInputController.transform.localScale != Vector3.one)
		{
			GlobalStorage.Instance.MasterHMDAndInputController.transform.localScale = Vector3.one;
			GlobalStorage.Instance.MasterHMDAndInputController.transform.position = Vector3.zero;
			MasterHMDAndInputController masterHMDAndInputController3 = GlobalStorage.Instance.MasterHMDAndInputController;
			for (int k = 0; k < masterHMDAndInputController3.InteractionHandControllers.Count; k++)
			{
				if (masterHMDAndInputController3.InteractionHandControllers[k].transform.localScale != Vector3.one)
				{
					masterHMDAndInputController3.InteractionHandControllers[k].transform.localScale = Vector3.one;
				}
			}
		}
		if (IsRubberModeEnabled())
		{
			SoundControlManager.SetImpactAudioDataOverride(GameSettings.Instance.RubberModeCustomImpactAudioData);
		}
		else
		{
			SoundControlManager.SetImpactAudioDataOverride(null);
		}
	}

	public static PhysicMaterial GetRubberModePhysicMaterial()
	{
		if (rubberModePhysicMaterial == null)
		{
			rubberModePhysicMaterial = GameSettings.Instance.RubberModePhysicMaterial;
		}
		return rubberModePhysicMaterial;
	}

	public static bool AreAnyJobGenieModesActive()
	{
		return GlobalStorage.Instance.CurrentGenieModes != JobGenieCartridge.GenieModeTypes.None;
	}

	public static bool IsNoGravityEnabled()
	{
		return DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.NoGravityMode);
	}

	public static bool IsDollhouseModeEnabled()
	{
		return DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.DollhouseMode);
	}

	public static bool IsRubberModeEnabled()
	{
		return DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.RubberMode);
	}

	public static bool DoesContainGenieMode(JobGenieCartridge.GenieModeTypes listOfGenieModes, JobGenieCartridge.GenieModeTypes checkIfContains)
	{
		return (listOfGenieModes & checkIfContains) > JobGenieCartridge.GenieModeTypes.None;
	}

	public static bool IsShortModeEnabled()
	{
		return GlobalStorage.Instance.isShortModeEnabled;
	}

	public static void SetShortModeEnabled(bool enabled)
	{
		GlobalStorage.Instance.isShortModeEnabled = enabled;
		Init();
	}
}

using OwlchemyVR;
using UnityEngine;

public class CoreGameLoader : MonoBehaviour
{
	[SerializeField]
	private MasterHMDAndInputController masterHMDAndInputControllerPrefab;

	[SerializeField]
	private GameObject[] initOtherCorePrefabs;

	private MasterHMDAndInputController masterHMDAndInputController;

	public MasterHMDAndInputController MasterHMDAndInputController
	{
		get
		{
			return masterHMDAndInputController;
		}
	}

	public void Load()
	{
		masterHMDAndInputController = Object.Instantiate(masterHMDAndInputControllerPrefab);
		masterHMDAndInputController.gameObject.RemoveCloneFromName();
		Object.DontDestroyOnLoad(masterHMDAndInputController.gameObject);
		for (int i = 0; i < initOtherCorePrefabs.Length; i++)
		{
			GameObject target = Object.Instantiate(initOtherCorePrefabs[i]);
			target.RemoveCloneFromName();
			Object.DontDestroyOnLoad(target);
		}
	}
}

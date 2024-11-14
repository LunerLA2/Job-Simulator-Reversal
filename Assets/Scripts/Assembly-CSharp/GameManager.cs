using UnityEngine;

public class GameManager : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		TimeManager.Init();
	}

	private void Update()
	{
		TimeManager.ManualUpdate();
	}
}

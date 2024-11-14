using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTest : MonoBehaviour
{
	[SerializeField]
	private string sceneName;

	[SerializeField]
	private float loadDelay;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(loadDelay);
		SceneManager.LoadScene(sceneName);
	}
}

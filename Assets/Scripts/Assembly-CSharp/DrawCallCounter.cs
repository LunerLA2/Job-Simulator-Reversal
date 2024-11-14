using System.Collections;
using System.Text;
using UnityEngine;

public class DrawCallCounter : MonoBehaviour
{
	public TextMesh textmesh;

	public Transform drawCallCounterHolder;

	private StringBuilder sb = new StringBuilder();

	private void Start()
	{
	}

	private bool CheckFor4By3()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		float num3 = num / num2;
		float num4 = 1.333f;
		Debug.Log(num3);
		return Mathf.Abs(num4 - num3) < 0.05f;
	}

	private IEnumerator PrepInfo()
	{
		yield return new WaitForSeconds(0.5f);
		if (!CheckFor4By3())
		{
			Debug.LogError("Your Game View is not set to 4:3! ABORTING!");
			yield break;
		}
		Vector3 newPos = new Vector3(0.53f, 1.976f, -1.541f);
		Transform mainCam = Object.FindObjectOfType<AudioListener>().transform;
		mainCam.position = newPos;
		mainCam.localRotation = Quaternion.Euler(29f, 339f, 0f);
		drawCallCounterHolder.transform.parent = mainCam;
		drawCallCounterHolder.transform.localRotation = Quaternion.identity;
		UpdateInfo();
	}

	private void UpdateInfo()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Tab))
		{
			StartCoroutine(PrepInfo());
		}
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			UpdateInfo();
		}
		UpdateInfo();
	}
}

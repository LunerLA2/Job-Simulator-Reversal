using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
	public Text creditsText;

	public float scrollSpeed;

	private void Update()
	{
		Vector3 localPosition = creditsText.rectTransform.localPosition;
		localPosition.y += Time.deltaTime * scrollSpeed;
		localPosition.z = 0f;
		localPosition.x = 0f;
		creditsText.rectTransform.localPosition = localPosition;
	}
}

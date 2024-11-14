using UnityEngine;
using UnityEngine.UI;

public class SlideshowNavNode : MonoBehaviour
{
	[SerializeField]
	private RawImage thumbnail;

	[SerializeField]
	private GameObject outline;

	private CanvasGroup canvasGroup;

	public RawImage Thumbnail
	{
		get
		{
			return thumbnail;
		}
	}

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void SetState(bool isCreated, bool isEditing)
	{
		canvasGroup.alpha = ((!isCreated) ? 0.25f : 1f);
		outline.SetActive(isEditing);
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarqueeController : MonoBehaviour
{
	[SerializeField]
	private float scrollSpeed;

	[SerializeField]
	private int spacing = 6;

	[SerializeField]
	private float flipDuration = 0.5f;

	[SerializeField]
	private Text text;

	[SerializeField]
	private Camera textCamera;

	private MarqueeSectionController[] sections;

	private RenderTexture primaryTextTexture;

	private RenderTexture secondaryTextTexture;

	private float primaryScroll;

	private float secondaryScroll;

	private void Awake()
	{
		sections = GetComponentsInChildren<MarqueeSectionController>();
		float num = 0f;
		for (int i = 0; i < sections.Length; i++)
		{
			if (i > 0)
			{
				sections[i].scrollOffset = num;
			}
			num += sections[i].Size.x;
		}
	}

	public void SetMessages(params string[] messages)
	{
		string text = string.Empty;
		for (int i = 0; i < messages.Length; i++)
		{
			text = text + messages[i] + new string('\u00a0', spacing);
		}
		this.text.text = text;
		if (base.gameObject.activeSelf)
		{
			StopAllCoroutines();
			StartCoroutine(SetTextAsync());
		}
	}

	private IEnumerator SetTextAsync()
	{
		if (secondaryTextTexture != null)
		{
			Object.Destroy(secondaryTextTexture);
		}
		secondaryTextTexture = new RenderTexture((int)text.preferredWidth, (int)text.preferredHeight, 16);
		secondaryTextTexture.wrapMode = TextureWrapMode.Repeat;
		textCamera.gameObject.SetActive(true);
		textCamera.targetTexture = secondaryTextTexture;
		textCamera.Render();
		textCamera.gameObject.SetActive(false);
		for (int i = 0; i < sections.Length; i++)
		{
			sections[i].SetText(secondaryTextTexture, flipDuration);
		}
		yield return new WaitForSeconds(flipDuration);
		if (primaryTextTexture != null)
		{
			Object.Destroy(primaryTextTexture);
		}
		primaryTextTexture = secondaryTextTexture;
		secondaryTextTexture = null;
	}

	private void Update()
	{
		primaryScroll += Time.deltaTime * scrollSpeed;
		secondaryScroll += Time.deltaTime * scrollSpeed;
		if (primaryTextTexture != null)
		{
			while (primaryScroll >= (float)primaryTextTexture.width)
			{
				primaryScroll -= primaryTextTexture.width;
			}
			for (int i = 0; i < sections.Length; i++)
			{
				sections[i].SetPrimaryScroll(primaryScroll);
			}
		}
		if (secondaryTextTexture != null)
		{
			while (secondaryScroll >= (float)secondaryTextTexture.width)
			{
				secondaryScroll -= secondaryTextTexture.width;
			}
			for (int j = 0; j < sections.Length; j++)
			{
				sections[j].SetSecondaryScroll(secondaryScroll);
			}
		}
	}
}

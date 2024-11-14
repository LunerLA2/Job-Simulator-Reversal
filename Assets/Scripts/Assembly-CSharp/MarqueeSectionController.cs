using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarqueeSectionController : MonoBehaviour
{
	[HideInInspector]
	public float scrollOffset;

	[SerializeField]
	private RectTransform textContainer;

	[SerializeField]
	private RawImage primaryTextImage;

	[SerializeField]
	private RawImage secondaryTextImage;

	private Vector2 size;

	private RectTransform primaryTextTrans;

	private RectTransform secondaryTextTrans;

	public Vector2 Size
	{
		get
		{
			return size;
		}
	}

	private void Awake()
	{
		size = textContainer.rect.size;
		primaryTextTrans = primaryTextImage.rectTransform;
		secondaryTextTrans = secondaryTextImage.rectTransform;
	}

	public void SetPrimaryScroll(float scroll)
	{
		SetScroll(primaryTextImage, scroll);
	}

	public void SetSecondaryScroll(float scroll)
	{
		SetScroll(secondaryTextImage, scroll);
	}

	private void SetScroll(RawImage textImage, float scroll)
	{
		if (!(textImage == null))
		{
			Rect uvRect = textImage.uvRect;
			if (!(textImage.texture == null))
			{
				uvRect.position = new Vector2((scroll + scrollOffset) / (float)textImage.texture.width, 0f);
				textImage.uvRect = uvRect;
			}
		}
	}

	private void SetX(RectTransform trans, float x)
	{
		Vector2 anchoredPosition = trans.anchoredPosition;
		anchoredPosition.x = x;
		trans.anchoredPosition = anchoredPosition;
	}

	private void SetY(RectTransform trans, float y)
	{
		Vector2 anchoredPosition = trans.anchoredPosition;
		anchoredPosition.y = y;
		trans.anchoredPosition = anchoredPosition;
	}

	public void SetText(Texture textTexture, float flipDuration)
	{
		secondaryTextImage.texture = textTexture;
		Rect uvRect = secondaryTextImage.uvRect;
		uvRect.size = new Vector2(size.x / (float)textTexture.width, 1f);
		secondaryTextImage.uvRect = uvRect;
		StopAllCoroutines();
		StartCoroutine(FlipTextAsync(flipDuration));
	}

	private IEnumerator FlipTextAsync(float flipDuration)
	{
		float flipTime = 0f;
		SetY(primaryTextTrans, 0f);
		SetY(secondaryTextTrans, 0f);
		while (flipTime < flipDuration)
		{
			float y = flipTime / flipDuration * size.y;
			SetY(primaryTextTrans, y);
			SetY(secondaryTextTrans, y);
			flipTime = Mathf.Min(flipTime + Time.deltaTime, flipDuration);
			yield return null;
		}
		primaryTextImage.texture = secondaryTextImage.texture;
		primaryTextImage.uvRect = secondaryTextImage.uvRect;
		SetY(primaryTextTrans, 0f);
		SetY(secondaryTextTrans, 0f);
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MuseumTerminalTaskCell : MonoBehaviour
{
	private const float unchosenScale = 0.7f;

	private const float animationTime = 0.15f;

	[SerializeField]
	public Image cellImage;

	[SerializeField]
	private Image cellBorder;

	[SerializeField]
	public Text cellText;

	[SerializeField]
	private Color uncompletedColor = Color.grey;

	[SerializeField]
	private Color completedColor = Color.blue;

	[SerializeField]
	private Color uncompletedFontColor = Color.blue;

	[SerializeField]
	private Color completedFontColor = Color.blue;

	private string taskName;

	private bool selected;

	public bool complete;

	public string TaskName
	{
		get
		{
			return taskName;
		}
	}

	private void Awake()
	{
		cellImage.rectTransform.localScale = Vector3.one * 0.7f;
		cellBorder.transform.localScale = Vector3.one * 0.7f;
		cellImage.color = uncompletedColor;
		cellText.color = uncompletedFontColor;
		cellBorder.color = uncompletedColor;
	}

	public void SetSelected(bool selected)
	{
		this.selected = selected;
		if (selected)
		{
			cellBorder.color = completedColor;
		}
		else
		{
			cellBorder.color = cellImage.color;
		}
		StopCoroutine(SelectedAnimation());
		StartCoroutine(SelectedAnimation());
	}

	public void SetValues(string fullName, int index, bool completed)
	{
		StopAllCoroutines();
		cellImage.rectTransform.localScale = Vector3.one * 0.7f;
		cellBorder.transform.localScale = Vector3.one * 0.7f;
		taskName = fullName;
		cellText.text = (index + 1).ToString();
		complete = completed;
		if (complete)
		{
			cellImage.color = completedColor;
			cellText.color = completedFontColor;
			cellBorder.color = cellImage.color;
		}
		else
		{
			cellImage.color = uncompletedColor;
			cellText.color = uncompletedFontColor;
			cellBorder.color = uncompletedColor;
		}
	}

	private IEnumerator SelectedAnimation()
	{
		Vector3 startScale = cellImage.rectTransform.localScale;
		Vector3 startBorderScale = cellBorder.rectTransform.localScale;
		Vector3 endScale = ((!selected) ? (Vector3.one * 0.7f) : Vector3.one);
		float t = 0f - Time.deltaTime;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.15f, 1f);
			cellImage.transform.localScale = Vector3.Lerp(startScale, endScale, t);
			cellBorder.transform.localScale = Vector3.Lerp(startBorderScale, endScale, t);
			yield return null;
		}
	}
}

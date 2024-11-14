using UnityEngine;
using UnityEngine.UI;

public class CalendarPage : MonoBehaviour
{
	[SerializeField]
	private Image image;

	public void SetupCalendarPage(Sprite sprite)
	{
		image.sprite = sprite;
	}
}

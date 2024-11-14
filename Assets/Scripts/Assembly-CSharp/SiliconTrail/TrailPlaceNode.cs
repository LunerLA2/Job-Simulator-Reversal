using UnityEngine;
using UnityEngine.UI;

namespace SiliconTrail
{
	public class TrailPlaceNode : MonoBehaviour
	{
		[SerializeField]
		private Image[] images;

		[SerializeField]
		private Text nameText;

		private Image activeImage;

		public void SetPlace(string placeName)
		{
			nameText.text = placeName;
			if (activeImage != null)
			{
				activeImage.gameObject.SetActive(false);
				activeImage = null;
			}
			for (int i = 0; i < images.Length; i++)
			{
				Image image = images[i];
				if (image.name == placeName)
				{
					activeImage = image;
					activeImage.gameObject.SetActive(true);
					break;
				}
			}
		}
	}
}

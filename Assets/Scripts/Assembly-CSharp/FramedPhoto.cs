using OwlchemyVR;
using UnityEngine;

public class FramedPhoto : MonoBehaviour
{
	[SerializeField]
	private GameObject[] pictureObjects;

	[SerializeField]
	private MeshRenderer customPicture;

	[SerializeField]
	private WorldItem worldItem;

	public Texture GetCustomPictureTexture()
	{
		return customPicture.material.mainTexture;
	}

	public void SetupFramedPhoto(int pictureIndex, WorldItemData worldItemData)
	{
		for (int i = 0; i < pictureObjects.Length; i++)
		{
			pictureObjects[i].SetActive(pictureIndex == i);
		}
		customPicture.gameObject.SetActive(false);
		worldItem.ManualSetData(worldItemData);
	}

	public void SetupCustomPicture(Texture2D texture, WorldItemData worldItemData)
	{
		for (int i = 0; i < pictureObjects.Length; i++)
		{
			pictureObjects[i].SetActive(false);
		}
		customPicture.gameObject.SetActive(true);
		customPicture.material.mainTexture = texture;
		worldItem.ManualSetData(worldItemData);
	}
}

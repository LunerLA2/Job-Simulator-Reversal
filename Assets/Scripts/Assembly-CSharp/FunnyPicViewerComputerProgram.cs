using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class FunnyPicViewerComputerProgram : ComputerProgram
{
	[SerializeField]
	private WorldItemData programWorldItem;

	[SerializeField]
	private Image pictureImage;

	[SerializeField]
	private Sprite[] pictures;

	[SerializeField]
	private Text indexText;

	private int pictureIndex = -1;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.FunnyPicViewer;
		}
	}

	private void OnEnable()
	{
		ShowPicture(pictureIndex);
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable.name == "Next")
			{
				ShowPicture(pictureIndex + 1);
			}
			else if (clickable.name == "Prev")
			{
				ShowPicture(pictureIndex - 1);
			}
		}
	}

	private void ShowPicture(int newPictureIndex)
	{
		newPictureIndex = Mathf.Clamp(newPictureIndex, 0, pictures.Length);
		if (pictureIndex != newPictureIndex)
		{
			pictureIndex = newPictureIndex;
			if (pictureIndex == pictures.Length)
			{
				GameEventsManager.Instance.ItemActionOccurred(programWorldItem, "ACTIVATED");
				Finish();
			}
			else
			{
				pictureImage.sprite = pictures[pictureIndex];
				indexText.text = string.Format("{0}/{1}", pictureIndex + 1, pictures.Length);
			}
		}
	}

	private IEnumerator CloseAsync()
	{
		GameEventsManager.Instance.ItemActionOccurred(programWorldItem, "ACTIVATED");
		yield return new WaitForSeconds(1f);
		Finish();
	}
}

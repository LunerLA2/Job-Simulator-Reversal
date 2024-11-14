using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class GalleryComputerProgram : ComputerProgram
{
	[Serializable]
	public class PhotoEntry
	{
		public Sprite Sprite;

		public GameObject Prefab;
	}

	[SerializeField]
	private Image photoImage;

	[SerializeField]
	private PhotoEntry[] photos;

	[SerializeField]
	private Text indexText;

	[SerializeField]
	private ComputerClickable printButton;

	[SerializeField]
	private ComputerClickable nextButton;

	[SerializeField]
	private ComputerClickable prevButton;

	[SerializeField]
	private ComputerClickable quitButton;

	private int photoIndex;

	[SerializeField]
	private WorldItemData worldItemData;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Gallery;
		}
	}

	private void OnEnable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
		ShowPhoto(photoIndex);
	}

	private void OnDisable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
	}

	private void Update()
	{
		if (printButton.IsInteractive && hostComputer.IsPrinterBusy)
		{
			printButton.SetInteractive(false);
			printButton.Text.text = "Printing...";
		}
		else if (!printButton.IsInteractive && !hostComputer.IsPrinterBusy)
		{
			printButton.SetInteractive(true);
			printButton.Text.text = "Print";
		}
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable == quitButton)
			{
				Finish();
			}
			else if (clickable == nextButton)
			{
				ShowPhoto(photoIndex + 1);
			}
			else if (clickable == prevButton)
			{
				ShowPhoto(photoIndex - 1);
			}
			else if (clickable == printButton)
			{
				Print();
			}
		}
	}

	private void ShowPhoto(int newPhotoIndex)
	{
		photoIndex = Mathf.Clamp(newPhotoIndex, 0, photos.Length - 1);
		photoImage.sprite = photos[photoIndex].Sprite;
		indexText.text = string.Format("{0}/{1}", photoIndex + 1, photos.Length);
		nextButton.SetInteractive(photoIndex < photos.Length - 1);
		prevButton.SetInteractive(photoIndex > 0);
	}

	private void Print()
	{
		hostComputer.PrintObject(photos[photoIndex].Prefab);
	}
}

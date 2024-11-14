using UnityEngine;

[RequireComponent(typeof(GrabbableSlider))]
public class FilingCabinetFolderPopupEffect : MonoBehaviour
{
	[SerializeField]
	private Transform[] folders;

	[SerializeField]
	private Transform front;

	[SerializeField]
	private float folderPopupSpeed = 0.5f;

	[SerializeField]
	private AnimationCurve popupCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(0.1f, 0.1f, 0f, 0f));

	private GrabbableSlider slider;

	private float[] folderOutnessOffsets;

	private float[] targetFolderHeights;

	private float initialFolderHeight;

	private void Start()
	{
		slider = GetComponent<GrabbableSlider>();
		initialFolderHeight = folders[0].localPosition.y;
		folderOutnessOffsets = new float[folders.Length];
		for (int i = 0; i < folders.Length; i++)
		{
			Transform transform = folders[i];
			folderOutnessOffsets[i] = Vector3.Dot(transform.position - front.position, slider.GetAxisVector());
		}
	}

	private void Update()
	{
		for (int i = 0; i < folders.Length; i++)
		{
			Transform transform = folders[i];
			float time = slider.Offset + folderOutnessOffsets[i];
			Vector3 localPosition = transform.localPosition;
			float num = initialFolderHeight + popupCurve.Evaluate(time);
			if (num < localPosition.y)
			{
				localPosition.y = num;
			}
			else if (num > localPosition.y)
			{
				localPosition.y = Mathf.Min(num, localPosition.y + Time.deltaTime * folderPopupSpeed);
			}
			transform.localPosition = localPosition;
		}
	}
}

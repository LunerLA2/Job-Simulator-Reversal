using UnityEngine;

namespace OwlchemyVR
{
	public class HMD : MonoBehaviour
	{
		public void Init(PlayerController playerController)
		{
			base.transform.SetParent(playerController.transform, false);
			base.name = "Head";
		}

		public void SuggestState(HmdState suggestedState)
		{
			if (suggestedState != null)
			{
				base.transform.localPosition = suggestedState.Position;
				base.transform.localEulerAngles = suggestedState.EulerAngles;
			}
		}
	}
}

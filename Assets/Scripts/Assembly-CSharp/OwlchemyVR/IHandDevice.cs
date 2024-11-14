namespace OwlchemyVR
{
	public interface IHandDevice
	{
		HandState GetSuggestedHandState(Hand hand);

		void TriggerHapticPulse(Hand hand, float pulseRate);
	}
}

namespace OwlchemyVR
{
	public static class HandednessUtils
	{
		public static Handedness GetOpposite(this Handedness handedness)
		{
			return 1 - handedness;
		}
	}
}

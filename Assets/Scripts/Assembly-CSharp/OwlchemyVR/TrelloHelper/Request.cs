namespace OwlchemyVR.TrelloHelper
{
	public class Request
	{
		public RequestConfig config { get; set; }

		public RequestAudio audio { get; set; }

		public Request()
		{
			config = new RequestConfig();
			audio = new RequestAudio();
		}
	}
}

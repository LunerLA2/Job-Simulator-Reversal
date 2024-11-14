namespace OwlchemyVR.EditorUI
{
	public class DataEntryUIController : FieldUIController
	{
		protected HandUIPointerController currSelectingHandUIPointerController;

		public void SetupHandUIPointer(HandUIPointerController currSelectingHandUIPointerController)
		{
			this.currSelectingHandUIPointerController = currSelectingHandUIPointerController;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			currSelectingHandUIPointerController = null;
		}
	}
}

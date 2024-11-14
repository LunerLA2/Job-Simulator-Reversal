namespace OwlchemyVR.EditorUI
{
	public class UIComponentController : SimpleButton
	{
		private ComponentInfoObject componentInfoObj;

		public ComponentInfoObject ComponentInfoObj
		{
			get
			{
				return componentInfoObj;
			}
		}

		public void Init(ComponentInfoObject componentInfoObj)
		{
			this.componentInfoObj = componentInfoObj;
			labelText.text = componentInfoObj.Name;
		}
	}
}

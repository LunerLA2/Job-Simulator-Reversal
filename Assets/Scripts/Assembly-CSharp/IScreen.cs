public interface IScreen
{
	void Process(MenuStack stack);

	void OnEnter();

	void OnExit();
}

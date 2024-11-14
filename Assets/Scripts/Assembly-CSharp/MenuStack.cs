using System.Collections.Generic;

public class MenuStack
{
	private MenuLayout activeMenu;

	private List<MenuLayout> menuStack = new List<MenuLayout>();

	public void SetMenu(MenuLayout menu)
	{
		if (activeMenu != null)
		{
			activeMenu.GetOwner().OnExit();
		}
		menuStack.Clear();
		activeMenu = menu;
		activeMenu.GetOwner().OnEnter();
	}

	public MenuLayout GetMenu()
	{
		return activeMenu;
	}

	public void PushMenu(MenuLayout menu)
	{
		if (activeMenu != null)
		{
			activeMenu.GetOwner().OnExit();
		}
		menuStack.Add(activeMenu);
		activeMenu = menu;
		activeMenu.GetOwner().OnEnter();
	}

	public void PopMenu()
	{
		if (menuStack.Count > 0)
		{
			activeMenu.GetOwner().OnExit();
			activeMenu = menuStack[menuStack.Count - 1];
			menuStack.RemoveAt(menuStack.Count - 1);
		}
	}
}

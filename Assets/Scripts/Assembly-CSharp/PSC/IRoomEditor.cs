using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PSC
{
	public interface IRoomEditor
	{
		Scene scene { get; }

		LayoutConfiguration configuration { get; }

		List<Layout> layouts { get; }
	}
}

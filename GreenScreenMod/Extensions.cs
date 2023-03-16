using UnityEngine;

namespace GreenScreenMod;

public static class Extensions
{
	public static string GetPath(this Transform current)
	{
		if (current.parent == null) return current.name;
		return current.parent.GetPath() + "/" + current.name;
	}
}

using OWML.Common;
using OWML.ModHelper;

namespace GreenScreenMod;
public class GreenScreenMod : ModBehaviour
{
	public static GreenScreenMod Instance { get; private set; }

	public void Awake() => Instance = this;

	public void Start()
	{
		LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
		{
			if (loadScene == OWScene.SolarSystem || loadScene == OWScene.EyeOfTheUniverse)
			{
				ModHelper.Events.Unity.FireOnNextUpdate(() =>
				{
					Locator.GetPlayerBody().gameObject.AddComponent<GreenScreenController>();
					Locator.GetPlayerBody().gameObject.AddComponent<PromptController>();
				});
			}
		};
	}

	public static void Write(string msg) => Instance.ModHelper.Console.WriteLine(msg, MessageType.Info);

	public static void WriteError(string msg) => Instance.ModHelper.Console.WriteLine(msg, MessageType.Error);

	public static void WriteWarning(string msg) => Instance.ModHelper.Console.WriteLine(msg, MessageType.Warning);
}


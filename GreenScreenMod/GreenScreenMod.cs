using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GreenScreenMod;
public class GreenScreenMod : ModBehaviour
{
	public static GreenScreenMod Instance { get; private set; }

	public Material SharedMaterial { get; private set; }

	public void Awake()
	{
		SharedMaterial = new Material(Shader.Find("Unlit/Color"));
        SharedMaterial.renderQueue = 4000;
		Instance = this;
	}

	public override void Configure(IModConfig config)
	{
		base.Configure(config);

		var colourString = config.GetSettingsValue<string>("Screen colour");

		if (!ColorUtility.TryParseHtmlString(colourString, out var colour))
		{
			WriteError($"Could not parse colour {colourString}. Must either be hexadecimal (formatted #RGB or #RRGGBB), or a supported literal name like green or blue.");
			colour = Color.green;
		}
		SharedMaterial.color = colour;

		if (Enum.TryParse<Key>(config.GetSettingsValue<string>("'Mark via raycast' key").ToUpperInvariant(), out var raycastKey))
		{
			GreenScreenController.RaycastKey = raycastKey;
		}
		else
		{
			WriteError($"Invalid key binding for raycast.");
		}

		if (Enum.TryParse<Key>(config.GetSettingsValue<string>("'Mark current position' key").ToUpperInvariant(), out var currentPositionKey))
		{
			GreenScreenController.CurrentPositionKey = currentPositionKey;
		}
		else
		{
			WriteError($"Invalid key binding for mark current position.");
		}

		if (Enum.TryParse<Key>(config.GetSettingsValue<string>("'Clear markers' key").ToUpperInvariant(), out var clearPointsKey))
		{
			GreenScreenController.ClearPointsKey = clearPointsKey;
		}
		else
		{
			WriteError($"Invalid key binding for clear markers.");
		}

		if (Enum.TryParse<Key>(config.GetSettingsValue<string>("'Clear screens' key").ToUpperInvariant(), out var clearScreensKey))
		{
			GreenScreenController.ClearScreensKey = clearScreensKey;
		}
		else
		{
			WriteError($"Invalid key binding for clear screens.");
		}

		PromptController.Instance?.ResetPrompts();
	}

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


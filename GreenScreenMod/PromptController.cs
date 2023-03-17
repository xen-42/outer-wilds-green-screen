using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GreenScreenMod;

public class PromptController : MonoBehaviour
{
	private ScreenPrompt _raycastPrompt, _atPositionPrompt, _clearMarkersPrompt, _clearScreensPrompt;
	private ScreenPrompt[] _prompts;

	public static PromptController Instance { get; private set; }

	public void Awake() => Instance = this;

	public void Start() => ResetPrompts();

	public void ResetPrompts()
	{
		if (_prompts != null) foreach (var prompt in _prompts) Locator.GetPromptManager().RemoveScreenPrompt(prompt, PromptPosition.UpperRight);

		_prompts = new ScreenPrompt[]
		{
			_raycastPrompt = AddPrompt("Mark via raycast", PromptPosition.UpperRight, GreenScreenController.RaycastKey),
			_atPositionPrompt = AddPrompt("Mark current position", PromptPosition.UpperRight, GreenScreenController.CurrentPositionKey),
			_clearMarkersPrompt = AddPrompt("Clear markers", PromptPosition.UpperRight, GreenScreenController.ClearPointsKey),
			_clearScreensPrompt = AddPrompt("Clear screens", PromptPosition.UpperRight, GreenScreenController.ClearScreensKey)
		};
	}

	public void Update()
	{
		var visible = !OWTime.IsPaused() && !GUIMode.IsHiddenMode();

		// Top right
		_raycastPrompt.SetVisibility(visible);
		_atPositionPrompt.SetVisibility(visible);

		_clearMarkersPrompt.SetVisibility(visible && GreenScreenController.Instance.Markers.Count > 0);
		_clearScreensPrompt.SetVisibility(visible && GreenScreenController.Instance.Screens.Count > 0);
	}

	private static ScreenPrompt AddPrompt(string text, PromptPosition position, Key key)
	{
		Enum.TryParse(key.ToString().Replace("Digit", "Alpha"), out KeyCode keyCode);

		return AddPrompt(text, position, keyCode);
	}

	private static ScreenPrompt AddPrompt(string text, PromptPosition position, KeyCode keyCode)
	{
		var texture = ButtonPromptLibrary.SharedInstance.GetButtonTexture(keyCode);
		var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
		sprite.name = texture.name;

		var prompt = new ScreenPrompt(text, sprite);
		Locator.GetPromptManager().AddScreenPrompt(prompt, position, false);

		return prompt;
	}
}

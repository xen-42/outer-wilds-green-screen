using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GreenScreenMod;

internal class GreenScreenController : MonoBehaviour
{
	public static GreenScreenController Instance { get; private set; }

	public static int LayerMask => OWLayerMask.physicalMask;

	public List<GameObject> Markers { get; private set; } = new();
	public List<PointInfo> Points { get; private set; } = new();
	public List<GameObject> Screens { get; private set; } = new();

	public static Key RaycastKey => Key.O;
	public static Key CurrentPositionKey => Key.I;
	public static Key ClearPointsKey => Key.U;
	public static Key ClearScreensKey => Key.K;

	public void Awake() => Instance = this;

	public void Start()
	{
		// Bloom makes green screen put green everywhere
		Locator.GetPlayerCamera().postProcessingSettings.bloomEnabled = false;
	}

	public void Update()
	{
		if (Keyboard.current[RaycastKey].wasReleasedThisFrame) TrackPoint(Raycast());
		if (Keyboard.current[CurrentPositionKey].wasReleasedThisFrame) TrackPoint(CurrentPosition());
		if (Keyboard.current[ClearPointsKey].wasReleasedThisFrame) ClearPoints();
		if (Keyboard.current[ClearScreensKey].wasReleasedThisFrame) ClearScreens();

		if (Points.Count >= 4)
		{
			GreenScreenMod.Write("Making green screen");
			Screens.Add(ShapeCreator.CreateGreenScreen(Points[0].Body.gameObject, Points[0].Position, Points[1].Position, Points[2].Position, Points[3].Position));
			ClearPoints();
		}
	}

	private void ClearPoints()
	{
		foreach (var marker in Markers) GameObject.Destroy(marker);
		Markers.Clear();
		Points.Clear();

		GreenScreenMod.Write("Cleared tracked points");
	}

	private void ClearScreens()
	{
		foreach (var screen in Screens) GameObject.Destroy(screen);
		Screens.Clear();
	}

	private void TrackPoint(PointInfo point)
	{
		if (Points.Count > 0 && point.Body != Points.First().Body)
		{
			GreenScreenMod.WriteError("You can't extend a green screen between two rigidbodies!");
			ClearPoints();
		}

		Points.Add(point);
		Markers.Add(ShapeCreator.AddSphere(point.Body.gameObject, point.Position, 0.1f, Color.green));

		GreenScreenMod.Write($"Tracking {Points.Count} points");
	}

	private static PointInfo Raycast()
	{
		var origin = Locator.GetActiveCamera().transform.position;
		var direction = Locator.GetActiveCamera().transform.forward;

		if (Physics.Raycast(origin, direction, out var hitInfo, 100f, LayerMask))
		{
			var pos = hitInfo.rigidbody.transform.InverseTransformPoint(hitInfo.point);
			return new PointInfo(pos, hitInfo.rigidbody.GetAttachedOWRigidbody());
		}
		else
		{
			return null;
		}
	}

	private static PointInfo CurrentPosition()
	{
		OWRigidbody relativeBody = null;

		if (Locator.GetPlayerController().IsGrounded())
		{
			relativeBody = Locator.GetPlayerController()._groundBody;
		}
		if (relativeBody == null && (Time.time - Locator.GetPlayerController()._lastGroundedTime < 2f))
		{
			relativeBody = Locator.GetPlayerController()._lastGroundBody;
		}
		if (relativeBody == null)
		{
			relativeBody = Locator.GetReferenceFrame(true)?.GetOWRigidBody();
		}
		if (relativeBody == null)
		{
			relativeBody = Locator.GetReferenceFrame(false)?.GetOWRigidBody();
		}
		if (relativeBody == null)
		{
			GreenScreenMod.WriteError("Couldn't find something to position relative to.");
			Locator.GetPlayerAudioController().PlayNegativeUISound();
			return null;
		}

		var pos = relativeBody.transform.InverseTransformPoint(Locator.GetPlayerCamera().transform.position);
		return new PointInfo(pos, relativeBody);
	}
}

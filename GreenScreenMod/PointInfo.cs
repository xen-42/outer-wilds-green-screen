using UnityEngine;

namespace GreenScreenMod;

public class PointInfo
{
	public Vector3 Position { get; private set; }
	public OWRigidbody Body { get; private set; }

	public PointInfo(Vector3 position, OWRigidbody body)
	{
		Position = position;
		Body = body;
	}	
}

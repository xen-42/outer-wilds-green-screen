using UnityEngine;

namespace GreenScreenMod;

public static class ShapeCreator
{
	public static GameObject AddSphere(GameObject parent, Vector3 localPos, float radius)
	{
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.name = "DebugSphere";

		try
		{
			sphere.GetComponent<SphereCollider>().enabled = false;
			sphere.transform.parent = parent.transform;
			sphere.transform.localPosition = localPos;
			sphere.transform.localScale = new Vector3(radius, radius, radius);

			sphere.GetComponent<MeshRenderer>().sharedMaterial = GreenScreenMod.Instance.SharedMaterial;
		}
		catch
		{
			// Something went wrong so make sure the sphere is deleted
			GameObject.Destroy(sphere);
		}

		return sphere.gameObject;
	}

	public static GameObject CreateGreenScreen(GameObject parent, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
	{
		var greenScreen = new GameObject();
		greenScreen.transform.name = "GreenScreen";

		try
		{
			// Global pos?
			var p1 = parent.transform.TransformPoint(pos1);
			var p2 = parent.transform.TransformPoint(pos2);
			var p3 = parent.transform.TransformPoint(pos3);
			var p4 = parent.transform.TransformPoint(pos4);

			var globalCenter = (p1 + p2 + p3 + p4) / 4f;

			var plane1 = OneSidedScreen(globalCenter, p1, p2, p3, p4);
			var plane2 = OneSidedScreen(globalCenter, p4, p3, p2, p1);

			plane1.transform.parent = greenScreen.transform;
			plane1.transform.localPosition = Vector3.zero;

			plane2.transform.parent = greenScreen.transform;
			plane2.transform.localPosition = Vector3.zero;

			greenScreen.transform.parent = parent.transform;
			greenScreen.transform.position = globalCenter;
			greenScreen.transform.rotation = Quaternion.identity;

			// Don't ask
			greenScreen.transform.localScale = Vector3.one * -1f;
		}
		catch
		{
			GameObject.Destroy(greenScreen);
		}
		

		return greenScreen;
	}

	private static GameObject OneSidedScreen(Vector3 globalCenter, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	{
		var mesh = new Mesh();

		mesh.vertices = new Vector3[]
		{
			globalCenter - p1,
			globalCenter - p2,
			globalCenter - p3,
			globalCenter - p4
		};

		mesh.uv = new Vector2[]
		{
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 0),
			new Vector2(0, 1)
		};

		mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

		mesh.RecalculateNormals();

		var plane = new GameObject("Plane");

		var mf = plane.AddComponent<MeshFilter>();
		mf.mesh = mesh;

		var mr = plane.AddComponent<MeshRenderer>();
		mr.sharedMaterial = GreenScreenMod.Instance.SharedMaterial;

		return plane;
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

namespace SPACE_OBJECT
{
	public static class Z
	{
		public static float dot(Vector3 a , Vector3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}
	}

	public static class Cam
	{
		public static Camera cam;

		public static void Init()
		{
			GameObject cam_obj = GameObject.Find("cam");
			if (cam_obj == null)
				console.error("obj with name <cam> not found");

			cam = cam_obj.GetComponent<Camera>();
		}

		public static Vector2 pos2D
		{
			get
			{
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);

				Vector3 a = ray.origin;
				Vector3 n = ray.direction;

				Vector3 o = Vector3.zero;
				Vector3 up = Vector3.forward;

				// p = a + n * L, (p - o).up = 0
				float L = -Z.dot(a - o, up) / Z.dot(n, up);
				return a + n * L;
			}
		}
		//
	}
}
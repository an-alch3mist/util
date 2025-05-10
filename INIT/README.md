```INIT.cs```
```cs
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

namespace SPACE_0
{
	[DefaultExecutionOrder(-1000)] // first script to run, after UnityEngine Initialization
	public class INITManager : MonoBehaviour
	{
		[SerializeField] Camera MainCam;
		[SerializeField] RectTransform CanvasRectTransform;
		private void Awake()
		{
			Debug.Log("Awake(): " + this);
			INPUT.INITIALIZE(
				MainCam: MainCam,
				CanvasRectTransform: this.CanvasRectTransform
			);

			LOG.INITIALIZE();
			GameData.LoadGame();
		}

		// check >>

		// << check

		[SerializeField] GameData gameData = null;
		private void OnApplicationQuit()
		{
			GameData.SaveGame(gameData);
		}
	}
}
```

```/UTIL/UTIL.cs```
```cs
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SPACE_UTIL
{
	// v2 a = new v2(0, 0)
	// v2 b = (1, 2)
	[System.Serializable]
	public struct v2
	{
		public int x, y;

		public v2(int x, int y) { this.x = x; this.y = y; }
		public override string ToString()
		{
			return $"[{x}, {y}]";
			//return base.ToString();
		}

		public static v2 operator +(v2 a, v2 b) { return new v2(a.x + b.x, a.y + b.y); }
		public static v2 operator -(v2 a, v2 b) { return new v2(a.x - b.x, a.y - b.y); }
		public static bool operator ==(v2 a, v2 b) { return a.x == b.x && a.y == b.y; }
		public static bool operator !=(v2 a, v2 b) { return a.x != b.x || a.y != b.y; }
		public static float dot(v2 a, v2 b) { return a.x * b.x + a.y * b.y; }
		public static float area(v2 a, v2 b) { return a.x * b.y - a.y * b.x; }

		// Allow implicit conversion from tuple
		public static implicit operator v2((int, int) tuple) => new v2(tuple.Item1, tuple.Item2);
	}

	public static class Z
	{
		#region dot
		public static float dot(Vector3 a, Vector3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}
		#endregion

		#region lerp
		public static float lerp(float a, float b, float t)
		{
			float n = b - a;
			return a + n * t;
		}
		public static Vector3 lerp(Vector3 a, Vector3 b, float t)
		{
			Vector3 n = b - a;
			return a + n * t; ;
		}
		#endregion
	}

	public class INPUT
	{
		public static void INITIALIZE(Camera MainCam, RectTransform CanvasRectTransform)
		{
			M.MainCam = MainCam;
			UI.CanvasRectTransform = CanvasRectTransform;
		}

		#region MOUSE
		public static class M
		{
			public static Camera MainCam;
			// up = new vec3(0, 0, +1)
			public static Vector3 up = new Vector3(0, 0, +1);
			public static Vector3 getPos3D
			{
				get
				{
					// plane: (r - o).up = 0
					Vector3 up = INPUT.M.up;
					Vector3 o = Vector3.zero;

					Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);

					// line: a + n * L
					Vector3 a = ray.origin;
					Vector3 n = ray.direction;

					float L = -Z.dot(a - o, up) / Z.dot(n, up);
					return a + n * L;
				}
			}
			public static bool InstantDown(int mouse_btn_type = 0)
			{
				return Input.GetMouseButtonDown(mouse_btn_type);
			}
			public static bool HeldDown(int mouse_btn_type = 0)
			{
				return Input.GetMouseButtonDown(mouse_btn_type);
			}
			public static bool InstantUp(int mouse_btn_type = 0)
			{
				return Input.GetMouseButtonUp(mouse_btn_type);
			}
		}
		#endregion

		#region K
		public static class K
		{
			public static bool InstantDown(KeyCode keyCode)
			{
				return Input.GetKeyDown(keyCode);
			}
			public static bool Down(KeyCode keyCode)
			{
				return Input.GetKey(keyCode);
			}
			public static bool InstantUp(KeyCode keyCode)
			{
				return Input.GetKeyUp(keyCode);
			}

			public static KeyCode KeyCodeInstantDown
			{
				get
				{
					if (K.InstantDown(KeyCode.W)) return KeyCode.W;
					if (K.InstantDown(KeyCode.A)) return KeyCode.A;
					if (K.InstantDown(KeyCode.S)) return KeyCode.S;
					if (K.InstantDown(KeyCode.D)) return KeyCode.D;
					if (K.InstantDown(KeyCode.Tab)) return KeyCode.Tab;
					if (K.InstantDown(KeyCode.Escape)) return KeyCode.Escape;

					if (K.InstantDown(KeyCode.LeftArrow)) return KeyCode.LeftArrow;
					if (K.InstantDown(KeyCode.RightArrow)) return KeyCode.RightArrow;
					if (K.InstantDown(KeyCode.UpArrow)) return KeyCode.UpArrow;
					if (K.InstantDown(KeyCode.DownArrow)) return KeyCode.DownArrow;

					return KeyCode.Backslash;
				}
			}
		}
		#endregion

		#region UI
		public static class UI
		{
			// is move pointer over (any UI gameobject) / (UI EventSystem) ?
			public static bool Hover
			{
				get { return EventSystem.current.IsPointerOverGameObject(); }
			}

			public static RectTransform CanvasRectTransform;
			public static Vector2 pos
			{
				// return 1280, 720 regardless of canvas scale provided same ratio
				get { return Input.mousePosition / CanvasRectTransform.localScale.x; }
			}
			public static Vector2 size
			{
				// return 1280, 720 regardless of canvas scale provided same ratio
				get { return new Vector2(Screen.width, Screen.height) / CanvasRectTransform.localScale.x; }
			}
		}
		#endregion
	}

	public static class C
	{
		public static float clamp(float x, float min, float max)
		{
			if (x > max) return max;
			if (x < min) return min;
			return x;
		}
		public static int round(float x)
		{
			if (x > 0f)
			{
				int x_I = (int)x;
				float frac = x - x_I;
				if (frac > 0.5f) return x_I + 1;
				else return x_I;

			}
			else if (x < 0f)
			{
				int x_I = (int)x;
				float frac = x - x_I;
				if (frac < -0.5f) return x_I - 1;
				else return x_I;
			}
			return 0;
		}
		public static int floor(float x)
		{
			return (int)x;
		}
	}

	public static class U
	{
		// converted to lowercase before check
		#region .NameStartsWith
		public static Transform NameStartsWith(this Transform transform, string name)
		{
			for (int i0 = 0; i0 < transform.childCount; i0 += 1)
				if (transform.GetChild(i0).name.ToLower().StartsWith(name.ToLower()))
					return transform.GetChild(i0);
			Debug.LogError($"found no leaf starting with that name: {name.ToLower()}, under transform: {transform.name}");
			return null;
		}
		public static GameObject NameStartsWith(this GameObject gameObject, string name)
		{
			Transform transform = gameObject.transform;
			for (int i0 = 0; i0 < transform.childCount; i0 += 1)
				if (transform.GetChild(i0).name.ToLower().StartsWith(name.ToLower()))
					return transform.GetChild(i0).gameObject;
			Debug.LogError($"found no leaf starting with that name: {name.ToLower()}, under transform: {transform.name}");
			return null;
		}
		#endregion

		// get Leaves under a Transform/GameObject
		#region .getLeaves
		public static List<Transform> GetLeaves(this Transform transform)
		{
			List<Transform> T = new List<Transform>();
			for (int i0 = 0; i0 < transform.childCount; i0 += 1)
				if (transform.GetChild(i0))
					T.Add(transform.GetChild(i0));

			if (T.Count == 0)
				Debug.LogError($"found no leaves under: {transform.name}");
			return T;
		}
		public static List<GameObject> GetLeaves(this GameObject gameObject)
		{
			List<GameObject> G = new List<GameObject>();
			Transform transform = gameObject.transform;
			for (int i0 = 0; i0 < transform.childCount; i0 += 1)
				if (transform.GetChild(i0))
					G.Add(transform.GetChild(i0).gameObject);

			if (G.Count == 0)
				Debug.LogError($"found no leaves under: {transform.name}");
			return G;
		}
		#endregion

		// CanPlaceBuilding.... pos2D, gameObject with a collider2D 
		#region CanPlaceObject2D(Vector2 pos2D, GameObject gameObject)
		public static bool CanPlaceObject2D(Vector2 pos2D, GameObject gameObject)
		{
			Collider2D collider = gameObject.GetComponent<Collider2D>();

			if (collider is BoxCollider2D)
			{
				BoxCollider2D boxCollider2D = (BoxCollider2D)collider;
				Collider2D[] COLLIDER = Physics2D.OverlapBoxAll(pos2D + boxCollider2D.offset, boxCollider2D.size, angle: 0f);
				return COLLIDER.Length == 0;
			}
			else if (collider is CircleCollider2D)
			{
				CircleCollider2D circleCollider2D = (CircleCollider2D)collider;
				Collider2D[] COLLIDER = Physics2D.OverlapCircleAll(pos2D + circleCollider2D.offset, circleCollider2D.radius);
				return COLLIDER.Length == 0;
			}
			else if (collider is CapsuleCollider2D)
			{
				CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)collider;
				Collider2D[] COLLIDER = Physics2D.OverlapCapsuleAll(pos2D + capsuleCollider2D.offset, capsuleCollider2D.size, capsuleCollider2D.direction, angle: 0f);
				return COLLIDER.Length == 0;
			}
			//
			Debug.LogError($"no collider attached to {gameObject.name} at {gameObject.transform.position}");
			return true;
		}

		#endregion
		// CanPlaceBuilding.... pos3D, gameObject with a collider
		#region CanPlaceObject3D(Vector3 pos3D,GameObject _prefab,int rotationY = 0)
		/// <summary>
		/// Determines whether a prefab (with potentially multiple BoxColliders) can be placed
		/// at the given world position and euler rotation, without overlapping any existing colliders.
		/// </summary>
		/// <param name="_prefab">A GameObject template that has one or more BoxCollider components.</param>
		/// <param name="pos3D">Desired world‑space position for the prefab’s root.</param>
		/// <param name="eulerRotation">Desired world‑space rotation (in degrees) for the prefab’s root.</param>
		/// <param name="layerMask">Which layers to include in the overlap test (defaults to all layers).</param>
		/// <returns>True if no overlaps occur; false if any collider would intersect something else.</returns>
		public static bool CanPlaceObject3D(
			Vector3 pos3D,
			GameObject _prefab,
			int rotationY = 0)
		{
			// Make sure the physics engine is up to date
			Physics.SyncTransforms();
			LayerMask layerMask = Physics.AllLayers;

			// Parent orientation from the requested Euler angles
			var parentOrientation = Quaternion.Euler(new Vector3(0f, 90 * rotationY, 0f));

			// Gather all BoxColliders (including on children)
			var boxes = _prefab.GetComponents<BoxCollider>();

			foreach (var box in boxes)
			{
				// 1. Calculate world‐space half extents:
				//    half = local size * 0.5, then scale by the collider's lossyScale
				float e = 1f / 1000;
				Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale) - Vector3.one * e;

				// 2. Calculate world‐space center:
				//    take local center offset, scale it, rotate by parentOrientation, then translate
				Vector3 scaledCenterOffset = Vector3.Scale(box.center, box.transform.lossyScale);
				Vector3 worldCenter = pos3D + parentOrientation * scaledCenterOffset;

				// 3. Calculate world‐space orientation of this collider:
				//    combine parentRotation with the collider's local rotation
				Quaternion worldRot = parentOrientation * box.transform.localRotation;

				// 4. Perform the overlap test, ignoring trigger-only colliders
				Collider[] hits = Physics.OverlapBox(
					worldCenter,
					halfExtents,
					worldRot,
					layerMask,
					QueryTriggerInteraction.Ignore);

				// 5. If any hit is **not** part of our prefab, placement is invalid
				if (hits.Length != 0)
					return false;
			}

			// All colliders were clear
			return true;
		}
		#endregion

		// ad
		#region ad
		public static string AbrrevatedNumber(int value)
		{
			// Define scales
			Dictionary<long, string> scales = new Dictionary<long, string>
		{
			{1_000_000_000_000, "T"},
			{1_000_000_000,     "B"},
			{1_000_000,         "M"},
			{1_000,             "K"}
		};

			// Numbers below the smallest scale are unchanged
			if (value < 1_000)
				return value.ToString();

			// Find the largest applicable scale
			foreach (long threshold in scales.Keys.OrderByDescending(k => k))
			{
				if (value >= threshold)
				{
					double scaled = (double)value / threshold;
					double truncated = Math.Floor(scaled * 10) / 10;  // one decimal, always down :contentReference[oaicite:7]{index=7}

					// If the number part is 20 or more, drop decimals
					if (truncated >= 10)
						return $"{(int)truncated}{scales[threshold]}";

					// Otherwise, show one decimal (e.g. 1.1k, 19.9k)
					return $"{truncated:0.#}{scales[threshold]}";
				}
			}
			// default
			return value.ToString();
		}

		public static string RoundDecimal(float val, int digits = 2)
		{
			float new_val = (int)(val * Mathf.Pow(10, digits)) / (Mathf.Pow(10, digits));
			return new_val.ToString();
		}
		#endregion

		// Extension
		#region minMax(func, splice), find(func), findIndex(func)
		public static T minMax<T>(this T[] T_1D, Func<T, T, float> cmp_func)
		{
			T min = T_1D[0];
			for (int i0 = 1; i0 < T_1D.Length; i0 += 1)
				if (cmp_func(T_1D[i0], min) < 0f) // if ( b - a ) < 0f, than a < b, so swap
					min = T_1D[i0];
			return min;
		}
		public static T minMax<T>(this List<T> T_1D, Func<T, T, float> cmp_func, bool splice = false)
		{
			T min = minMax(T_1D.ToArray(), cmp_func);
			if (splice)
				T_1D.Remove(min);
			return min;
		}
		public static T find<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			// collection.MoveNext(), or a foreach loop
			foreach (var item in collection)
				if (predicate(item))
					return item;
			Debug.Log("found none with collection name provided");
			return default(T); // Returns null for reference types, default value for value types
		}
		public static int findIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			int index = 0;
			// collection.MoveNext(), or a foreach loop
			foreach (var item in collection)
			{
				if (predicate(item))
					return index;
				index += 1;
			}
			return -1; // Returns -1 if found none
		}

		#endregion
	}

	/*
		.SaveLog(str)
		.SaveGame(str)
		.LoadGame
		.ToTable("name")
	*/
	// file LOG.INITIALIZE() befdore
	public static class LOG
	{
		static string LocFolder = Application.dataPath + "/LOG";
		static string LocFile_LOG = Application.dataPath + "/LOG/LOG.txt";
		static string LocFile_GameData = Application.dataPath + "/LOG/GameData.txt";
		public static void INITIALIZE()
		{
			if (System.IO.Directory.Exists(LocFolder) == false) System.IO.Directory.CreateDirectory(LocFolder);
			if (System.IO.File.Exists(LocFile_LOG) == false) System.IO.File.Create(LocFile_LOG);
			if (System.IO.File.Exists(LocFile_GameData) == false) System.IO.File.Create(LocFile_GameData);
		}

		public static void SaveLog(params object[] args)
		{
			string str = string.Join("\n\n", args);
			//string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			//string logEntry = $"[{timestamp}] {str}";
			Debug.Log($"logged into file LOG.txt: {str}");
			// File logging

			try { System.IO.File.AppendAllText(LocFile_LOG, str + Environment.NewLine); }
			catch (Exception e) { Debug.LogError($"Failed to write to log file: {e.Message}"); }
		}
		public static void SaveGame(string str)
		{
			Debug.Log($"logged into file GameData.txt: {str}");
			System.IO.File.WriteAllText(LocFile_GameData, str);
		}
		public static string LoadGame
		{
			get
			{
				try
				{
					Debug.Log("Loaded GameData.txt");
					return System.IO.File.ReadAllText(LocFile_GameData);
				}
				catch (Exception)
				{
					Debug.LogError("no file found at: " + LocFile_GameData);
					throw;
				}
			}
		}

		/// <summary>
		/// make sure element class got ovverriden ToString() method
		/// does to ToString() for each of attribute, with thier name in column
		/// Renders a list of T into a plain-text table. 
		/// Columns are sized to fit the widest cell in each column.
		/// </summary>
		// LIST<>
		public static string ToTable<T>(this IEnumerable<T> list, string name = "LIST<>")
		{
			if (list == null)
				return "list is null";
			var items = list.ToList();
			if (items.Count == 0)
				return "list got no elem";

			var sb = new StringBuilder();
			var type = typeof(T);
			var fields = type.GetFields(
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic
			);

			// Calculate column widths
			var columnWidths = new int[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				columnWidths[i] = fields[i].Name.Length;
				foreach (var item in items)
				{
					object val = fields[i].GetValue(item);
					columnWidths[i] = Math.Max(columnWidths[i], (val?.ToString() ?? "null").Length);
				}
				columnWidths[i] += 2; // Add a little padding
			}

			// Header
			sb.AppendLine(string.Join(" | ", fields.Select((f, i) =>
			{
				string fieldName = f.Name;
				if (!f.IsPublic)
					fieldName = "-" + fieldName; // prefix - for private field
				return fieldName.PadRight(columnWidths[i]);
			})));

			// Separator line(dashes + +-separators),before: sb.AppendLine(new string('-', columnWidths.Sum() + (fields.Length - 1) * 3));
			for (int i0 = 0; i0 < fields.Length; i0 += 1)
			{
				sb.Append(new string('-', columnWidths[i0]));
				if (i0 < fields.Length - 1)
					sb.Append("-+-"); // seperator
			}
			sb.AppendLine();

			// Rows
			foreach (var item in items)
			{
				var values = fields.Select((f, i) =>
				{
					var val = f.GetValue(item);
					return (val?.ToString() ?? "null").PadRight(columnWidths[i]);
				});
				sb.AppendLine(string.Join(" | ", values));
			}

			return $"{name}:\n" + sb.ToString();
		}

		// MAP<>
		public static string ToTable<TKey, TValue>(this Dictionary<TKey, TValue> dict, string name = "MAP<>")
		{
			if (dict == null)
				return "dictionary is null";
			if (dict.Count == 0)
				return "dictionary got no elem";

			var sb = new StringBuilder();
			var keys = dict.Keys.ToList();
			var values = dict.Values.ToList();

			// Calculate column widths
			int keyWidth = Math.Max("key".Length, keys.Max(k => k?.ToString().Length ?? 0)) + 2;
			int valueWidth = Math.Max("VAL".Length, values.Max(v => v?.ToString().Length ?? 0)) + 2;

			// Header
			sb.AppendLine($"key".PadRight(keyWidth) + " | " + $"VAL".PadRight(valueWidth));
			sb.AppendLine(new string('-', keyWidth) + "-+-" + new string('-', valueWidth));


			// Rows
			for (int i = 0; i < keys.Count; i++)
			{
				string key = keys[i]?.ToString() ?? "null";
				string value = values[i]?.ToString() ?? "null";
				sb.AppendLine(key.PadRight(keyWidth) + " | " + value.PadRight(valueWidth));
			}

			return $"{name}:\n" + sb.ToString();
		}
	}
}
```

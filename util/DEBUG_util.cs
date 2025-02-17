using System.Collections;
using System.Collections.Generic;
using System.Linq;
using util; 
using UnityEngine;
using System.Threading.Tasks;

public class DEBUG_util : MonoBehaviour
{
	private void Update()
	{

		if(Input.GetMouseButtonDown(0))
		{
			console.log(dt);
			//dt = U.floor(Time.deltaTime * 1000);
			StopAllCoroutines();
			StartCoroutine(STIMULATE());

			await_delay_check();
		}
	}

	IEnumerator STIMULATE()
	{
		#region check
		/*
		string str_0 = "somthng";
		string str_1 = "somthng";
		str_1.PadLeft(10, '0');

		//Debug.Log(str_0 == str_1, str_0, str_1);
		//console.log_mode = "txt";
		console.log(str_0 == str_1, null , str_1);

		List<int> A = new List<int>();
		for (int i0 = 0; i0 < 10; i0 += 1)
			A.Add(i0 + 1);


		var pr = new U.xoro128(121);
		ITER.iter = 0;
		while(true)
		{
			if (ITER.iter_inc(1))
				break;

			console.log(pr.get_pr(-5, 100));
			console.log(console.time);
			yield return U.wait(10);
			console.log(console.time);
		}

		while (true)
		{
			if (ITER.iter_inc(2))
				break;

			console.log(pr.get_pr(-5, 100));
			console.log(console.time);
			yield return U.wait(20);
			console.log(console.time);
		}
		*/
		#endregion

		for (float t = 0f; t < 1f; t += 1f * dt * 0.001f)
		{
			routine.position = new Vector3()
			{
				x = t * 2,
				y = routine.transform.position.y,
				z = 0f,
			};
			yield return U.wait(dt);
		}
		console.log("=done= routine", console.time);

		//console.LOG_txt();
		//yield return null;
	}

	public int dt = 16;
	public Transform routine;
	public Transform _await;

	async void await_delay_check()
	{
		#region check
		/*
		console.log("somthng a");

		console.log(console.time);
		await U.delay(10);
		console.log(console.time);

		console.log("somthng b");

		console.log(console.time);
		await U.delay(20);
		console.log(console.time);

		console.log("somthng c");
		*/ 
		#endregion

		for(float t = 0f; t < 1f; t += 1f * dt * 0.001f)
		{
			_await.position = new Vector3()
			{
				x = t * 2,
				y = _await.transform.position.y,
				z = 0f,
			};
			await U.delay(dt);
		}

		console.log("=done= await", console.time);
	}
}





using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void CallbackDelegate(GameObject obj);
public class CTargetMovableObject : MonoBehaviour {

	public void fly(Vector3 begin, Vector3 to, Sprite sprite, CallbackDelegate finished_callback)
	{
		StartCoroutine(run_fly(begin, to, sprite, finished_callback));
	}


	IEnumerator run_fly(Vector3 begin, Vector3 to, Sprite sprite, CallbackDelegate finished_callback)
	{
		gameObject.GetComponentInChildren<Image>().sprite = sprite;

		float duration = CEffectManager.FLY_DURATION;
		float begin_time = Time.time;
		while (Time.time - begin_time <= duration)
		{
			float elapsed_time = (Time.time - begin_time) / duration;
			Vector3 pos = begin;
			pos.x = EasingUtil.easeOutExpo(begin.x, to.x, elapsed_time);
			pos.y = EasingUtil.easeOutExpo(begin.y, to.y, elapsed_time);

			gameObject.transform.position = pos;
			yield return 0;
		}

		finished_callback(gameObject);
	}
}

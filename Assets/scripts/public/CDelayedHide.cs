using UnityEngine;
using System.Collections;

public class CDelayedHide : MonoBehaviour {

	public void hide(float delay, CallbackDelegate callback)
	{
		StartCoroutine(hide_after_delay(delay, callback));
	}


	IEnumerator hide_after_delay(float delay, CallbackDelegate callback)
	{
		yield return new WaitForSeconds(delay);
		callback(gameObject);
		gameObject.SetActive(false);
	}
}

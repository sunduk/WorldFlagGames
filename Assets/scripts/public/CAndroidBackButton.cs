using UnityEngine;
using System.Collections;

public class CAndroidBackButton : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.WindowsEditor)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				gameObject.SendMessage("on_back_button");
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CLinkedButton : MonoBehaviour {

	void Awake()
	{
		GetComponent<Button>().onClick.AddListener(this.on_touch);
	}


	void on_touch()
	{
		string url = transform.FindChild("link_text").GetComponent<Text>().text;
		Application.OpenURL(url);
	}
}

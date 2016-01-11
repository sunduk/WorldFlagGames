using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class COpenSourceInfo : MonoBehaviour {

	void Awake()
	{
		transform.FindChild("button_back").GetComponent<Button>().onClick.AddListener(this.on_back);
	}


	void on_back()
	{
		CUIManager.Instance.show_fade_out_in(UI_PAGE.SOURCE_INFO, UI_PAGE.GAME_ABOUT);
	}


	void on_back_button()
	{
		on_back();
	}
}

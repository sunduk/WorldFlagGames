using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CGameAbout : MonoBehaviour {

	void Awake()
	{
		transform.FindChild("button_review").GetComponent<Button>().onClick.AddListener(this.on_review);
		transform.FindChild("button_source_info").GetComponent<Button>().onClick.AddListener(this.on_source_info);
		transform.FindChild("button_email").GetComponent<Button>().onClick.AddListener(this.on_email);
		transform.FindChild("button_back").GetComponent<Button>().onClick.AddListener(this.on_back);
	}


	void on_review()
	{
		Application.OpenURL("market://details?id=com.happyhouse.worldflags");
	}


	void on_source_info()
	{
		CUIManager.Instance.show_fade_out_in(UI_PAGE.GAME_ABOUT, UI_PAGE.SOURCE_INFO);
	}


	void on_email()
	{
		mail_to("[WorldFlagGames]");
	}


	void mail_to(string subject)
	{
		Application.OpenURL(string.Format("mailto:{0}?subject={1}", "lee.seokhyun@gmail.com", subject));
	}


	void on_back()
	{
		CUIManager.Instance.show_fade_out_in(UI_PAGE.GAME_ABOUT, UI_PAGE.AREA_SELECT);
	}


	void on_back_button()
	{
		on_back();
	}
}

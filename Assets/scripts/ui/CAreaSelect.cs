using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public enum AREA : byte
{
	asia,
	africa,
	europe,
	america,
	oceania
}

public class CAreaSelect : MonoBehaviour {

	List<UnityAction> callback_area;

	void Awake()
	{
		this.callback_area = new List<UnityAction>();
		this.callback_area.Add(this.on_asia);
		this.callback_area.Add(this.on_africa);
		this.callback_area.Add(this.on_europe);
		this.callback_area.Add(this.on_america);
		this.callback_area.Add(this.on_oceania);

		int index = 1;
		foreach (AREA e in Enum.GetValues(typeof(AREA)))
		{
			string name = string.Format("button_{0:D2}", index);
			transform.FindChild(name).GetComponent<Button>().onClick.AddListener(this.callback_area[index - 1]);
			++index;
		}

		transform.FindChild("button_help").GetComponent<Button>().onClick.AddListener(this.on_help);
		transform.FindChild("toggle_sound").GetComponent<Toggle>().onValueChanged.AddListener(this.on_sound);
		transform.FindChild("button_quit").GetComponent<Button>().onClick.AddListener(this.on_quit);

		CUserManager.Instance.owner.load();
	}


	void OnEnable()
	{
		CSoundManager.Instance.update_main_bgm_clip();
		CSoundManager.Instance.play_main_bgm();
		refresh_sound_state();
	}


	void on_asia()
	{
		on_play(AREA.asia);
	}


	void on_africa()
	{
		on_play(AREA.africa);
	}


	void on_europe()
	{
		on_play(AREA.europe);
	}


	void on_america()
	{
		on_play(AREA.america);
	}


	void on_oceania()
	{
		on_play(AREA.oceania);
	}


	void on_mix()
	{
		on_play_mix();
	}


	void on_play(AREA area)
	{
		CSoundManager.Instance.stop_bgm();
		CSoundManager.Instance.update_area_bgm_clip(area);
		CSoundManager.Instance.play_bgm(area);

		CGameLogic.Instance.update_area(area);

		CUIManager.Instance.show_fade_out_in(UI_PAGE.AREA_SELECT, UI_PAGE.STAGE_SELECT, () =>
		{
			CUIManager.Instance.get_uipage(UI_PAGE.STAGE_SELECT).GetComponent<CStageSelect>().enter(area);
		});
	}


	void on_play_mix()
	{
		//todo:준비중...
	}


	void on_help()
	{
		CUIManager.Instance.show_fade_out_in(UI_PAGE.AREA_SELECT, UI_PAGE.GAME_ABOUT);
	}


	void on_sound(bool flag)
	{
		Debug.Log(flag);
		CSoundManager.Instance.toggle_sound(flag);
		CUserManager.Instance.owner.save();
	}


	void refresh_sound_state()
	{
		transform.FindChild("toggle_sound").GetComponent<Toggle>().isOn = CSoundManager.Instance.sound_on_flag;
	}


	void on_quit()
	{
#if UNITY_EDITOR
		Debug.Log("quit");
		return;
#endif

		Application.Quit();
	}


	void on_back_button()
	{
		on_quit();
	}
}

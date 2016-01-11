using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum UI_PAGE
{
	FADE_OUT_IN,
	PLAY_ROOM,
	MAIN_MENU,
	AREA_SELECT,
	STAGE_SELECT,
	GAME_RESULT,
	GAME_ABOUT,
	SOURCE_INFO
}

public delegate void DefaultDelegate();
public class CUIManager : CSingletonMonobehaviour<CUIManager>
{
	Dictionary<UI_PAGE, GameObject> ui_objects;

	void Awake()
	{
		this.ui_objects = new Dictionary<UI_PAGE, GameObject>();
		this.ui_objects.Add(UI_PAGE.FADE_OUT_IN, transform.FindChild("fade_out_in").gameObject);
		this.ui_objects.Add(UI_PAGE.MAIN_MENU, transform.FindChild("main_menu").gameObject);
		this.ui_objects.Add(UI_PAGE.PLAY_ROOM, transform.FindChild("playroom").gameObject);
		this.ui_objects.Add(UI_PAGE.AREA_SELECT, transform.FindChild("area_select").gameObject);
		this.ui_objects.Add(UI_PAGE.STAGE_SELECT, transform.FindChild("stage_select").gameObject);
		this.ui_objects.Add(UI_PAGE.GAME_RESULT, transform.FindChild("game_result").gameObject);
		this.ui_objects.Add(UI_PAGE.GAME_ABOUT, transform.FindChild("game_about").gameObject);
		this.ui_objects.Add(UI_PAGE.SOURCE_INFO, transform.FindChild("source_info").gameObject);

		CFlagSpriteManager.Instance.load_all_flags();
		CEffectManager.Instance.load_all();
		CSoundManager.Instance.load_all();
	}


	public GameObject get_uipage(UI_PAGE page)
	{
		return this.ui_objects[page];
	}


	public void show(UI_PAGE page)
	{
		this.ui_objects[page].SetActive(true);
	}


	public void hide(UI_PAGE page)
	{
		this.ui_objects[page].SetActive(false);
	}


	public void show_fade_out_in(UI_PAGE prev_page, UI_PAGE next_page, DefaultDelegate finished_callback = null)
	{
		get_uipage(UI_PAGE.FADE_OUT_IN).GetComponent<CFadeOutIn>().update_ui_page(
			prev_page, next_page, finished_callback);
		CUIManager.Instance.show(UI_PAGE.FADE_OUT_IN);
	}
}

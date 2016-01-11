using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CStageSelect : MonoBehaviour {

	// ui 컨트롤.
	Text stage_title;
	Text stage_text;
	List<GameObject> star_gray;
	List<GameObject> star_gold;
	GameObject button_challenge;
	List<Image> preview_slots;

	short current_stage_index;
	List<CTableStageAttribute> stage_attrs;


	void Awake()
	{
		// test.
		CTableDataManager.Instance.load_all();

		this.stage_title = transform.FindChild("stage_title").GetComponent<Text>();
		this.stage_text = transform.FindChild("stage_text").GetComponent<Text>();
		this.star_gray = new List<GameObject>();
		this.star_gold = new List<GameObject>();
		for (int i = 0; i < 3; ++i)
		{
			this.star_gray.Add(transform.FindChild(string.Format("star_gray_{0:D2}", (i + 1))).gameObject);
			this.star_gold.Add(transform.FindChild(string.Format("star_gold_{0:D2}", (i + 1))).gameObject);
		}

		this.button_challenge = transform.FindChild("button_challenge").gameObject;
		this.button_challenge.GetComponent<Button>().onClick.AddListener(this.on_challenge);

		transform.FindChild("button_prev").GetComponent<Button>().onClick.AddListener(this.on_prev);
		transform.FindChild("button_next").GetComponent<Button>().onClick.AddListener(this.on_next);
		transform.FindChild("button_back").GetComponent<Button>().onClick.AddListener(this.on_back);

		// 미리보기 슬롯 컨트롤.
		Transform preview = transform.FindChild("preview");
		this.preview_slots = new List<Image>();
		for (int i = 0; i < CPlayRoom.MAX_PLAYER_SLOTS; ++i)
		{
			Image image = 
				preview.FindChild(string.Format("slot{0:D2}", (i + 1))).GetComponent<Image>();
			this.preview_slots.Add(image);
		}

		refresh(0);
	}


	void on_back()
	{
		CSoundManager.Instance.stop_bgm();

		CUIManager.Instance.hide(UI_PAGE.STAGE_SELECT);
		CUIManager.Instance.show(UI_PAGE.AREA_SELECT);
		CUIManager.Instance.show(UI_PAGE.MAIN_MENU);
	}


	void on_challenge()
	{
		CUIManager.Instance.show_fade_out_in(UI_PAGE.STAGE_SELECT, UI_PAGE.PLAY_ROOM, () =>
		{
			CUIManager.Instance.hide(UI_PAGE.MAIN_MENU);
			CGameLogic.Instance.restart(this.current_stage_index);
		});
	}


	void on_prev()
	{
		if (this.current_stage_index > 0)
		{
			--this.current_stage_index;
		}

		refresh(this.current_stage_index);
	}


	void on_next()
	{
		if (this.current_stage_index < this.stage_attrs.Count - 1)
		{
			++this.current_stage_index;
		}

		refresh(this.current_stage_index);
	}


	public void enter(AREA area)
	{
		refresh(0);
	}


	public CTableStageAttribute get_current_stage_attr()
	{
		return this.stage_attrs[this.current_stage_index];
	}


	public void refresh(short stage_list_index)
	{
		CSoundManager.Instance.volume_up();

		this.current_stage_index = stage_list_index;
		this.stage_attrs = CTableStageAttribute.find_stage_attr(CGameLogic.Instance.selected_area);
		this.stage_title.text = this.stage_attrs[stage_list_index].title;
		this.stage_text.text = this.stage_attrs[stage_list_index].text;

		hide_all_goldstars();
		short stage_index = get_current_stage_attr().stage_index;
		byte star_count = CUserManager.Instance.owner.get_star_count_of_stage(stage_index);
		for (byte i = 0; i < star_count; ++i)
		{
			this.star_gold[i].SetActive(true);
		}

		show_preview();
	}


	void show_preview()
	{
		for (int i = 0; i < this.preview_slots.Count; ++i)
		{
			this.preview_slots[i].sprite = CFlagSpriteManager.Instance.get_blank_sprite();
			this.preview_slots[i].SetNativeSize();
			this.preview_slots[i].gameObject.SetActive(false);
		}

		bool visible_flag = get_current_stage_attr().visible_flag;
		List<CTableStage> stage_data = CTableStage.find_stage_data(get_current_stage_attr().stage_pair);
		for (int i = 0; i < stage_data.Count; ++i)
		{
			CTableFlags flag = CTableDataManager.Instance.flags.Find(obj => obj.flag_index == stage_data[i].flag_index);
			this.preview_slots[i].sprite = CFlagSpriteManager.Instance.find_sprite(flag.resource);
			if (visible_flag)
			{
				Color color = this.preview_slots[i].color;
				color.a = 1.0f;
				this.preview_slots[i].color = color;
			}
			else
			{
				Color color = this.preview_slots[i].color;
				color.a = 0.3f;
				this.preview_slots[i].color = color;
			}

			this.preview_slots[i].SetNativeSize();
			this.preview_slots[i].gameObject.SetActive(true);
		}
	}


	void hide_all_goldstars()
	{
		for (int i = 0; i < this.star_gold.Count; ++i)
		{
			this.star_gold[i].SetActive(false);
		}
	}


	void on_back_button()
	{
		on_back();
	}
}

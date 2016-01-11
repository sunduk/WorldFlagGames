using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CPlayRoom : MonoBehaviour {

	public static readonly int MAX_PLAYER_SLOTS = 8;

	List<CFlagSlot> player_flags;

	Text text_level;
	RectTransform hp_bar;

	public Transform enemy_parent { get; private set; }
	public Transform effect_parent { get; private set; }


    void Awake()
    {
		this.enemy_parent = transform.FindChild("enemy_parent");
		this.effect_parent = transform.FindChild("effect_parent");

		this.hp_bar = transform.FindChild("red_line").GetComponent<RectTransform>();

		// 플레이어가 터치할 영역의 슬롯을 초기화 한다.
		Transform flag_list = transform.FindChild("flag_list");
		this.player_flags = new List<CFlagSlot>();
		for (int i = 0; i < MAX_PLAYER_SLOTS; ++i)
		{
			CFlagSlot slot =
				flag_list.FindChild(string.Format("button_flag_{0:D2}", (i + 1))).gameObject.AddComponent<CFlagSlot>();
			this.player_flags.Add(slot);
		}


		this.text_level = transform.FindChild("level").GetComponent<Text>();
		transform.FindChild("button_back").GetComponent<Button>().onClick.AddListener(this.on_back);

		reset();
    }


	public void restart()
	{
		refresh_player_flags();
		refresh_level_text();
	}



	void refresh_player_flags()
	{
		for (int i = 0; i < this.player_flags.Count; ++i)
		{
			this.player_flags[i].update_flag(null, CFlagSpriteManager.Instance.get_blank_sprite());
		}

		List<CTableStage> stage_data = CGameLogic.Instance.get_current_stage_data();
		if (CGameLogic.Instance.current_stage_attribute.suffle_flag)
		{
			CHelper.Shuffle<CTableStage>(stage_data);
		}
		for (int i = 0; i < stage_data.Count; ++i)
		{
			CTableFlags flag = CTableDataManager.Instance.flags.Find(obj => obj.flag_index == stage_data[i].flag_index);
			this.player_flags[i].update_flag(flag, CFlagSpriteManager.Instance.find_sprite(flag.resource));
		}
	}


    public void reset()
	{
    }


	void refresh_level_text()
	{
		this.text_level.text = CGameLogic.Instance.current_stage_attribute.title;
	}


    public void on_touch_flag(CTableFlags flag)
    {
		if (flag == null)
		{
			return;
		}

		CGameLogic.Instance.on_touch_flag(flag.flag_index);
    }


	bool is_matched()
	{
		return false;
	}


	public void refresh_hp_bar(int hp)
	{
		Vector3 scale = this.hp_bar.localScale;
		scale.x = hp * 96;
		this.hp_bar.localScale = scale;
	}


	public Vector3 get_slot_position(short flag_index)
	{
		CFlagSlot slot = this.player_flags.Find(obj => obj.is_same(flag_index));
		if (slot == null)
		{
			return Vector3.zero;
		}

		return slot.transform.position;
	}


	void on_back()
	{
		CGameLogic.Instance.force_stop();
		CUIManager.Instance.hide(UI_PAGE.PLAY_ROOM);
		CUIManager.Instance.show(UI_PAGE.STAGE_SELECT);
		CUIManager.Instance.show(UI_PAGE.MAIN_MENU);
		CStageSelect stage = CUIManager.Instance.get_uipage(UI_PAGE.STAGE_SELECT).GetComponent<CStageSelect>();
		stage.refresh(CGameLogic.Instance.current_stage);
	}


	void on_back_button()
	{
		on_back();
	}
}

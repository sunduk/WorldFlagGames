using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GAME_MODE : byte
{
	// 국기가 화면에 보여지는 모드.
	VISIBLITY = 0,

	// 숨겨진 국기를 맞추는 모드.
	HIDDEN
}

/// <summary>
/// 게임의 로직을 담당하는 클래스. 시작과 종료를 체크한다.
/// </summary>
public class CGameLogic : CSingletonMonobehaviour<CGameLogic>
{
	public GAME_MODE current_game_mode { get; private set; }
	public short current_stage { get; private set; }
	public int success_count { get; private set; }
	int passed_count;
	List<CTableStage> stage_data;
	public CTableStageAttribute current_stage_attribute { get; private set; }
	int hp;
	CEnemyGenerator enemy_generator;
	public AREA selected_area { get; private set; }


	void Awake()
	{
		this.enemy_generator = gameObject.AddComponent<CEnemyGenerator>();
		this.stage_data = new List<CTableStage>();
		this.current_stage = 0;
		reset();
	}


	/// <summary>
	/// 한판 시작 직전에 초기화 해줘야 할 것들.
	/// </summary>
	public void reset()
	{
		this.success_count = 0;
		this.passed_count = 0;
		this.hp = 5;
	}


	public void update_area(AREA area)
	{
		this.selected_area = area;
	}


	public void restart(short stage_index)
	{
		this.current_stage = stage_index;
		reset();
		CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().refresh_hp_bar(this.hp);
		this.current_game_mode = GAME_MODE.VISIBLITY;
		this.current_stage_attribute = CTableStageAttribute.find_stage_attr(this.selected_area, this.current_stage);
		refresh_stage_data();

		CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().restart();
		this.enemy_generator.restart(CGameLogic.Instance.current_stage_attribute);

		CSoundManager.Instance.volume_up();
	}


	public void on_touch_flag(short flag_index)
	{
		this.enemy_generator.on_attacked(flag_index);
	}


	/// <summary>
	/// 한개의 국기 맞추기를 성공 했을 때 호출된다.
	/// </summary>
	public void succeeded()
	{
		++this.success_count;
		check_finished();
	}


	/// <summary>
	/// 국기를 못맞추고 지나쳤을 때 호출된다.
	/// </summary>
	public void passed()
	{
		--this.hp;
		CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().refresh_hp_bar(this.hp);

		++this.passed_count;
		if (is_die())
		{
			game_over();
			return;
		}

		check_finished();
	}


	/// <summary>
	/// 게임이 끝난 뒤 점수를 계산한다.
	/// 별 개수(3, 2, 1)로 점수를 표시한다.
	/// </summary>
	/// <returns></returns>
	public byte calc_star_count()
	{
		byte star_count = 0;
		int perfect_count = this.current_stage_attribute.flag_count;
		if (this.success_count >= perfect_count)
		{
			star_count = 3;
		}
		else
		{
			if (this.success_count >= perfect_count * 0.5)
			{
				star_count = 2;
			}
			else if (this.success_count >= perfect_count * 0.2)
			{
				star_count = 1;
			}
		}

		return star_count;
	}


	void check_finished()
	{
		Debug.Log(string.Format("succeeded {0},  passed {1}", this.success_count, this.passed_count));
		if (this.current_stage_attribute.flag_count <= this.success_count + this.passed_count)
		{
			game_over();
		}
	}


	void game_over()
	{
		// 게임 중지.
		this.enemy_generator.stop();
		CSoundManager.Instance.volume_down();

		// 점수 계산.
		byte star_count = calc_star_count();

		if (star_count >= 2)
		{
			CSoundManager.Instance.play(EFFECT_SOUND.HIGH_SCORE);
		}
		else
		{
			CSoundManager.Instance.play(EFFECT_SOUND.LOW_SCORE);
		}


		CUserManager.Instance.owner.stage_clear(this.current_stage_attribute.stage_index, star_count);
		CUserManager.Instance.owner.save();

		CUIManager.Instance.show(UI_PAGE.GAME_RESULT);
		CUIManager.Instance.get_uipage(UI_PAGE.GAME_RESULT).GetComponent<CGameResult>().refresh(this.success_count, star_count);
	}


	public void force_stop()
	{
		this.enemy_generator.stop();
		CSoundManager.Instance.volume_down();
	}


	bool is_die()
	{
		return this.hp <= 0;
	}


	/// <summary>
	/// 현재 스테이지를 재시작 한다.
	/// </summary>
	public void restart_current()
	{
		restart(this.current_stage);
	}


	public void move_to_next_stage()
	{
		int count = CTableStageAttribute.find_stage_attr(this.selected_area).Count;
		if (this.current_stage < count - 1)
		{
			++this.current_stage;
			restart(this.current_stage);
		}
		else
		{
			CUIManager.Instance.get_uipage(UI_PAGE.GAME_RESULT).GetComponent<CGameResult>().on_back();
		}
	}


	void refresh_stage_data()
	{
		this.stage_data = CTableDataManager.Instance.stages.FindAll(obj =>
			obj.stage_pair.is_same(
				this.current_stage_attribute.stage_pair.area,
				this.current_stage_attribute.stage_pair.stage_index));
	}


	public List<CTableStage> get_current_stage_data()
	{
		return this.stage_data;
	}
}

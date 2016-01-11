using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CEnemyGenerator : MonoBehaviour {

	enum PLAY_STATE : byte
	{
		PLAYING = 0,
		PAUSE
	}


	CGameObjectPool<GameObject> pool_enemy;
	List<GameObject> live_objects;
	CTableStageAttribute current_stage_attribute;
	CSkillManager skill_manager;

	// 생성될 x좌표 리스트.
	// 겹쳐서 나오는것을 방지하기 위하여 x좌표 리스트를 갖고 있는다.
	List<float> x_positions;

	PLAY_STATE current_play_state;
	Queue<GameObject> success_targets;

	float prev_generated_time;


	void Awake()
	{
		GameObject original = Resources.Load<GameObject>("character/enemy_ui");
		this.pool_enemy = new CGameObjectPool<GameObject>(5, original, (GameObject obj) =>
		{
			GameObject clone = GameObject.Instantiate(obj);
			clone.SetActive(false);
			return clone;
		});

		this.live_objects = new List<GameObject>();
		this.skill_manager = gameObject.AddComponent<CSkillManager>();
		this.success_targets = new Queue<GameObject>();

		this.x_positions = new List<float>();
		this.x_positions.Add(-180.0f);
		this.x_positions.Add(-60.0f);
		this.x_positions.Add(60.0f);
		this.x_positions.Add(180.0f);
	}


	public void restart(CTableStageAttribute stage_attribute)
	{
		this.current_play_state = PLAY_STATE.PLAYING;
		destroy_all();
		this.current_stage_attribute = stage_attribute;

		this.prev_generated_time = Time.time;
		StopAllCoroutines();
		StartCoroutine("run");

		this.success_targets.Clear();
		this.death_list.Clear();
		StartCoroutine(update_success_scene());
	}


	IEnumerator run()
	{
		int count = 0;
		while (count < this.current_stage_attribute.flag_count)
		{
			if (this.current_play_state == PLAY_STATE.PAUSE)
			{
				yield return 0;
				continue;
			}

			if (Time.time - this.prev_generated_time >= this.current_stage_attribute.interval)
			{
				// 생성 위치를 랜덤하게 하기 위해서 셔플로 섞어준다.
				CHelper.Shuffle<float>(this.x_positions);
				for (int i = 0; i < this.current_stage_attribute.concurrent_count; ++i)
				{
					generate(this.x_positions[i]);
					++count;
				}

				this.prev_generated_time = Time.time;
			}

			yield return 0;
		}
	}


	void Update()
	{
		recycle_enemies();
		//speed_down_near_enemies();
	}


	void recycle_enemies()
	{
		List<GameObject> targets = new List<GameObject>();
		for (int i = 0; i < this.live_objects.Count; ++i)
		{
			if (this.live_objects[i].transform.localPosition.y <= -170.0f)
			{
				targets.Add(this.live_objects[i]);
			}
		}

		for (int i = 0; i < targets.Count; ++i)
		{
			destroy_enemy(targets[i]);
			CGameLogic.Instance.passed();
		}
	}


	void speed_down_near_enemies()
	{
		for (int i = 0; i < this.live_objects.Count; ++i)
		{
			if (this.live_objects[i].transform.localPosition.y <= -20.0f)
			{
				this.live_objects[i].GetComponent<CMovableObject>().update_speed_ratio(0.5f);
			}
		}
	}


	void destroy_enemy(GameObject obj)
	{
		obj.transform.SetParent(null);
		obj.SetActive(false);
		this.pool_enemy.push(obj);
		this.live_objects.Remove(obj);
	}


	public void destroy_all()
	{
		while (this.live_objects.Count > 0)
		{
			destroy_enemy(this.live_objects[0]);
		}
	}


	public void stop()
	{
		destroy_all();
		StopAllCoroutines();
	}


	/// <summary>
	/// 스테이지에 맞는 적군 하나를 랜덤으로 생성한다.
	/// </summary>
	void generate(float x)
	{
		List<CTableStage> stage_data = CGameLogic.Instance.get_current_stage_data();
		int index = UnityEngine.Random.Range(0, stage_data.Count);

		// 계층 구조 설정.
		GameObject enemy = this.pool_enemy.pop();
		Transform enemy_parent =
			CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().enemy_parent;
		enemy.transform.SetParent(enemy_parent);
		enemy.SetActive(true);

		enemy.GetComponent<RectTransform>().localPosition = new Vector3(x, 500, 0);
		enemy.transform.localScale = Vector3.one;

		// 기본 속성 설정.
		enemy.GetComponent<CMovableObject>().update_speed(this.current_stage_attribute.speed);
		enemy.GetComponent<CMovableObject>().update_speed_ratio(this.skill_manager.speed_ratio);

		// 국기 이미지 설정.
		CTableStage stage = stage_data[index];
		Sprite sprite = CFlagSpriteManager.Instance.find_sprite(stage.flag.resource);
		enemy.GetComponent<CEnemy>().update_flag(stage.flag, sprite,
			this.current_stage_attribute.visible_flag);

		this.live_objects.Add(enemy);
	}


	void pause()
	{
		this.current_play_state = PLAY_STATE.PAUSE;
		for (int i = 0; i < this.live_objects.Count; ++i)
		{
			this.live_objects[i].GetComponent<CMovableObject>().update_speed_ratio(0.0f);
		}
	}


	void resume()
	{
		for (int i = 0; i < this.live_objects.Count; ++i)
		{
			this.live_objects[i].GetComponent<CMovableObject>().update_speed_ratio(this.skill_manager.speed_ratio);
		}
		this.current_play_state = PLAY_STATE.PLAYING;

		this.prev_generated_time += CEffectManager.FLY_DURATION;
	}


	public void on_attacked(short flag_index)
	{
		List<GameObject> targets = 
			this.live_objects.FindAll(obj => obj.GetComponent<CEnemy>().is_same(flag_index));

		if (targets.Count <= 0)
		{
			on_missed();
			return;
		}


		// 이전에 맞춘 국기가 화면에서 완전히 사라지기 전까지는 중복해서 처리하지 않는다.
		bool already_exist_in_deathlist = 
			this.death_list.Exists(obj => obj.GetComponent<CEnemy>().is_same(flag_index));
		if (already_exist_in_deathlist)
		{
			return;
		}

		// 맞춘 국기 리스트를 보관해 놓고
		// 국기 소멸시 이펙트 처리와 중복 체크에 사용한다.
		for (int i = 0; i < targets.Count; ++i)
		{
			this.death_list.Add(targets[i]);
		}

		// 게임 모드에 따라 소멸되는 방식을 다르게 처리한다.
		// visible모드라면 즉시 없애주고,
		// hidden모드라면 국기가 목표지점까지 날아간 뒤 없애주도록 연출을 만들어 준다.
		if (this.current_stage_attribute.visible_flag)
		{
			destroy_correct_flags();
		}
		else
		{
			// 게임을 좀 더 쉽게 하기 위해서 국기 하나를 맞출 때 마다 게임 진행을 잠시 멈춰 준다.
			pause();

			// 맞춘 국기들을 success_targets큐에 넣어놓으면
			// update_success_scene매소드에서 꺼내와 연출을 처리한다.
			// 여러개의 국기들을 한꺼번에 맞추는 경우가 있을 수 있는데,
			// 국기들을 한꺼번에 모아서 처리하는 연출을 구현하기 위해
			// 즉시 처리하지 않고 큐에 넣어서 처리하게끔 구현 하였다.
			for (int i = 0; i < targets.Count; ++i)
			{
				this.success_targets.Enqueue(targets[i]);
			}
		}
	}


	List<GameObject> death_list = new List<GameObject>();
	IEnumerator update_success_scene()
	{
		while (true)
		{
			if (this.success_targets.Count <= 0)
			{
				yield return 0;
				continue;
			}

			GameObject target = this.success_targets.Dequeue();

			short flag_index = target.GetComponent<CEnemy>().flag.flag_index;
			Vector3 slot_position =
				CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().get_slot_position(flag_index);
			Sprite sprite = get_flag_sprite(target);

			CEffectManager.Instance.fly(
				slot_position,
				target.transform.position,
				sprite);

			pause();
			StopCoroutine("resume_delay");
			StartCoroutine("resume_delay");

			yield return 0;
		}
	}


	IEnumerator resume_delay()
	{
		yield return new WaitForSeconds(CEffectManager.FLY_DURATION);

		destroy_correct_flags();
		resume();
	}


	void destroy_correct_flags()
	{
		CEffectManager.Instance.hide_all_fly_effects();

		for (int i = 0; i < this.death_list.Count; ++i)
		{
			Sprite sprite = get_flag_sprite(this.death_list[i]);
			CEffectManager.Instance.show_destroy_effect(this.death_list[i].transform.position, sprite);

			destroy_enemy(this.death_list[i]);
			CGameLogic.Instance.succeeded();

			CSoundManager.Instance.play(EFFECT_SOUND.CORRECT);
		}
		death_list.Clear();
	}


	void on_missed()
	{
		Handheld.Vibrate();
		CSoundManager.Instance.play(EFFECT_SOUND.WRONG);

		this.skill_manager.skill_speed_up(() =>
		{
			// 스킬 적용이 끝난 후 모든 적들에 대한 스피드를 원래 값으로 돌려 놓음.
			for (int i = 0; i < this.live_objects.Count; ++i)
			{
				this.live_objects[i].GetComponent<CMovableObject>().update_speed_ratio(this.skill_manager.speed_ratio);
			}
		});

		// 화면에 존재하는 모든 적들에 대해서 스피드 증가.
		for (int i = 0; i < this.live_objects.Count; ++i)
		{
			this.live_objects[i].GetComponent<CMovableObject>().update_speed_ratio(this.skill_manager.speed_ratio);
		}
	}


	Sprite get_flag_sprite(GameObject enemy)
	{
		short flag_index = enemy.GetComponent<CEnemy>().flag.flag_index;
		string flag_name = CTableDataManager.Instance.flags[flag_index - 1].resource;
		return CFlagSpriteManager.Instance.find_sprite(flag_name);
	}
}

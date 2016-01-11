using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CSkillManager : MonoBehaviour {

	public delegate void Func();

	enum SKILL_TYPE : byte
	{
		SPEED_UP
	}


	public float speed_ratio { get; private set; }
	Dictionary<SKILL_TYPE, bool> running_skills;


	void Awake()
	{
		this.speed_ratio = 1.0f;
		this.running_skills = new Dictionary<SKILL_TYPE, bool>();
		foreach (SKILL_TYPE e in Enum.GetValues(typeof(SKILL_TYPE)))
		{
			this.running_skills.Add(e, false);
		}
	}


	bool is_running_now(SKILL_TYPE skill_type)
	{
		return this.running_skills[SKILL_TYPE.SPEED_UP];
	}


	public void skill_speed_up(Func finished_callback)
	{
		if (is_running_now(SKILL_TYPE.SPEED_UP))
		{
			return;
		}

		StartCoroutine(run_speed_ratio(3.0f, 0.3f, finished_callback));
	}


	IEnumerator run_speed_ratio(float ratio, float duration, Func finished_callback)
	{
		this.running_skills[SKILL_TYPE.SPEED_UP] = true;
		this.speed_ratio = ratio;

		yield return new WaitForSeconds(duration);
		this.speed_ratio = 1.0f;
		this.running_skills[SKILL_TYPE.SPEED_UP] = false;

		finished_callback();
	}
}

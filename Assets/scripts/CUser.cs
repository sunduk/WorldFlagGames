using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CUser
{
	Dictionary<short, byte> star_of_stage;


	public CUser()
	{
		this.star_of_stage = new Dictionary<short, byte>();
	}


	public void save()
	{
		Hashtable[] datas = new Hashtable[this.star_of_stage.Count];
		int i = 0;
		foreach (KeyValuePair<short, byte> kvp in this.star_of_stage)
		{
			datas[i] = new Hashtable();
			datas[i].Add("stage_index", kvp.Key);
			datas[i].Add("star", kvp.Value);
			++i;
		}

		Hashtable userinfo = new Hashtable();
		userinfo.Add("stage_history", datas);
		userinfo.Add("sound", CSoundManager.Instance.get_sound_state());
		string json_data = XUtil.MiniJSON.jsonEncode(userinfo);

		//Debug.Log("save : " + json_data);
		PlayerPrefs.SetString("userinfo_v2", json_data);
		PlayerPrefs.Save();
	}


	public void load()
	{
		if (!PlayerPrefs.HasKey("userinfo_v2"))
		{
			return;
		}

		string json_data = PlayerPrefs.GetString("userinfo_v2");
		Debug.Log("load : " + json_data);
		Hashtable userinfo = XUtil.MiniJSON.jsonDecode(json_data) as Hashtable;

		int sound = int.Parse(userinfo["sound"].ToString());
		CSoundManager.Instance.set_sound_state(sound);

		ArrayList stage_history = userinfo["stage_history"] as ArrayList;

		this.star_of_stage.Clear();
		for (int i = 0; i < stage_history.Count; ++i)
		{
			Hashtable stage_info = stage_history[i] as Hashtable;
			short stage_index = short.Parse(stage_info["stage_index"].ToString());
			byte star_count = byte.Parse(stage_info["star"].ToString());

			this.star_of_stage.Add(stage_index, star_count);
		}
	}


	public void stage_clear(short stage_index, byte star_count)
	{
		if (!this.star_of_stage.ContainsKey(stage_index))
		{
			this.star_of_stage.Add(stage_index, star_count);
			return;
		}


		// 이전 점수보다 클 때만 기록한다.
		if (this.star_of_stage[stage_index] < star_count)
		{
			this.star_of_stage[stage_index] = star_count;
		}
	}


	public byte get_star_count_of_stage(short stage_index)
	{
		if (!this.star_of_stage.ContainsKey(stage_index))
		{
			return 0;
		}

		return this.star_of_stage[stage_index];
	}
}

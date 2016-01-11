using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CFlagSpriteManager : CSingletonMonobehaviour<CFlagSpriteManager>
{
	Sprite[] total_flags;
	Dictionary<string, Sprite> flag_by_name;
	Sprite gray_box;
	Sprite question_box;


	public void load_all_flags()
	{
		this.total_flags = Resources.LoadAll<Sprite>("atlas/flags_download");
		this.flag_by_name = new Dictionary<string, Sprite>();
		for (int i = 0; i < this.total_flags.Length; ++i)
		{
			this.flag_by_name.Add(this.total_flags[i].name, this.total_flags[i]);
		}
		this.gray_box = Resources.Load<Sprite>("images/gray_box");
		this.question_box = Resources.Load<Sprite>("images/flag_no");
	}


	public Sprite find_sprite(string name)
	{
		if (!this.flag_by_name.ContainsKey(name))
		{
			return null;
		}

		return this.flag_by_name[name];
	}


	public Sprite get_blank_sprite()
	{
		return this.gray_box;
	}


	public Sprite get_question_sprite()
	{
		return this.question_box;
	}
}

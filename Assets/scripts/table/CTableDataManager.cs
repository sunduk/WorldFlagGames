using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CTableDataManager : CSingleton<CTableDataManager>
{
	public List<CTableFlags> flags { get; private set; }
	public List<CTableStage> stages { get; private set; }
	public List<CTableStageAttribute> stage_attributes { get; private set; }


	public CTableDataManager()
	{
		this.flags = new List<CTableFlags>();
		this.stages = new List<CTableStage>();
		this.stage_attributes = new List<CTableStageAttribute>();
	}


	public void load_all()
	{
		load_flags();
		load_stages();
		load_stage_attributes();
	}


	void load_flags()
	{
		TextAsset data = Resources.Load<TextAsset>("table/flags");
		ArrayList tables = XUtil.MiniJSON.jsonDecode(data.text) as ArrayList;

		for (int i = 0; i < tables.Count; ++i)
		{
			this.flags.Add(new CTableFlags((Hashtable)tables[i]));
		}
	}


	void load_stages()
	{
		TextAsset data = Resources.Load<TextAsset>("table/stages");
		ArrayList tables = XUtil.MiniJSON.jsonDecode(data.text) as ArrayList;

		for (int i = 0; i < tables.Count; ++i)
		{
			this.stages.Add(new CTableStage((Hashtable)tables[i]));
		}
	}


	void load_stage_attributes()
	{
		TextAsset data = Resources.Load<TextAsset>("table/stage_attributes");
		ArrayList tables = XUtil.MiniJSON.jsonDecode(data.text) as ArrayList;

		for (int i = 0; i < tables.Count; ++i)
		{
			this.stage_attributes.Add(new CTableStageAttribute((Hashtable)tables[i]));
		}
	}
}

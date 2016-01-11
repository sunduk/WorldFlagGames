using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CTableStageAttribute
{
	public short stage_index { get; private set; }
	public StagePair stage_pair { get; private set; }
	public string title { get; private set; }
	public string text { get; private set; }
	public float speed { get; private set; }
	public float interval { get; private set; }
	public short flag_count { get; private set; }
	public short concurrent_count { get; private set; }
	public bool visible_flag { get; private set; }
	public bool suffle_flag { get; private set; }

	public CTableStageAttribute(Hashtable table)
	{
		this.stage_index = short.Parse(table["idx"].ToString());
		this.stage_pair = new StagePair(table);
		this.title = table["title"].ToString();
		this.text = table["text"].ToString();
		this.speed = float.Parse(table["speed"].ToString());
		this.interval = float.Parse(table["interval"].ToString());
		this.flag_count = short.Parse(table["flag_count"].ToString());
		this.concurrent_count = short.Parse(table["concurrent_count"].ToString());
		this.visible_flag = byte.Parse(table["visible_flag"].ToString()) == 1 ? true : false;
		this.suffle_flag = byte.Parse(table["position_suffle"].ToString()) == 1 ? true : false;
	}


	public static CTableStageAttribute find_stage_attr(AREA area, short stage_index)
	{
		return find_stage_attr(area)[stage_index];
	}


	public static List<CTableStageAttribute> find_stage_attr(AREA area)
	{
		return CTableDataManager.Instance.stage_attributes.FindAll(obj =>
		{
			return obj.stage_pair.area == area;
		});
	}
}

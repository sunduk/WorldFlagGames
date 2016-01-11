using System;
using System.Collections;
using System.Collections.Generic;

public class StagePair
{
	public AREA area;
	public short stage_index;


	public StagePair(AREA area, short stage_index)
	{
		this.area = area;
		this.stage_index = stage_index;
	}


	public StagePair(string area_text, short stage_index)
	{
		foreach (AREA e in Enum.GetValues(typeof(AREA)))
		{
			bool found = area_text.Equals(e.ToString());
			if (found)
			{
				this.area = e;
				break;
			}
		}
		this.stage_index = stage_index;
	}


	public StagePair(Hashtable table)
	{
		string area_text = table["area"].ToString();
		foreach (AREA e in Enum.GetValues(typeof(AREA)))
		{
			bool found = area_text.Equals(e.ToString());
			if (found)
			{
				this.area = e;
				break;
			}
		}
		this.stage_index = short.Parse(table["stage"].ToString());
	}


	public bool is_same(AREA area, short stage_index)
	{
		if (this.area == area &&
			this.stage_index == stage_index)
		{
			return true;
		}

		return false;
	}


	public override string ToString()
	{
		return string.Format("{0},{1}", this.area.ToString(), this.stage_index);
	}
}

public class CTableStage
{
	public StagePair stage_pair { get; private set; }
	public short flag_index { get; private set; }
	public CTableFlags flag { get; private set; }

	public CTableStage(Hashtable table)
	{
		this.stage_pair = new StagePair(table);
		this.flag_index = short.Parse(table["index"].ToString());

		this.flag = CTableDataManager.Instance.flags.Find(obj => obj.flag_index == this.flag_index);
	}


	public static List<CTableStage> find_stage_data(StagePair pair)
	{
		return CTableDataManager.Instance.stages.FindAll(obj =>
			obj.stage_pair.is_same(pair.area, pair.stage_index));
	}
}

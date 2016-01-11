using System;
using System.Collections;
using System.Collections.Generic;

public class CTableFlags
{
	public short flag_index { get; private set; }
	public AREA area { get; private set; }
	public string country_name { get; private set; }
	public string resource { get; private set; }

	public CTableFlags(Hashtable table)
	{
		this.flag_index = short.Parse(table["index"].ToString());
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
		this.country_name = table["country"].ToString();
		this.resource = table["resource"].ToString();
	}
}

using UnityEngine;
using System.Collections;

public class CUserManager : CSingleton<CUserManager>
{
	public CUser owner { get; private set; }


	public CUserManager()
	{
		this.owner = new CUser();
	}
}

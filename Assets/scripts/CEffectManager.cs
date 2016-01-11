using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CEffectManager : CSingletonMonobehaviour<CEffectManager>
{
	public static float FLY_DURATION = 0.4f;
	CGameObjectPool<GameObject> pool_fly;
	List<GameObject> used_fly_effects;

	CGameObjectPool<GameObject> pool_destroy;
	GameObject ef_heart;


	void Awake()
	{
		this.used_fly_effects = new List<GameObject>();
	}


	public void load_all()
	{
		GameObject original = Resources.Load<GameObject>("effects/ef_fly");
		this.pool_fly = new CGameObjectPool<GameObject>(4,
			original, (GameObject obj) =>
			{
				GameObject clone = GameObject.Instantiate(obj);
				clone.SetActive(false);
				return clone;
			});

		original = Resources.Load<GameObject>("effects/ef_destroy");
		this.pool_destroy = new CGameObjectPool<GameObject>(3,
			original, (GameObject obj) =>
			{
				GameObject clone = GameObject.Instantiate(obj);
				clone.SetActive(false);
				return clone;
			});

		this.ef_heart = GameObject.Instantiate(Resources.Load<GameObject>("effects/ef_heart")) as GameObject;
		this.ef_heart.SetActive(false);
	}


	public void fly(Vector3 begin, Vector3 to, Sprite sprite)
	{
		CTargetMovableObject obj = this.pool_fly.pop().GetComponent<CTargetMovableObject>();
		obj.gameObject.SetActive(true);
		obj.transform.localScale = Vector3.one;

		Transform effect_parent = 
			CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().effect_parent;
		obj.transform.SetParent(effect_parent);

		this.used_fly_effects.Add(obj.gameObject);
		obj.fly(begin, to, sprite, (GameObject clone) => { });
	}


	public void hide_all_fly_effects()
	{
		for (int i = 0; i < this.used_fly_effects.Count; ++i)
		{
			this.used_fly_effects[i].SetActive(false);
			this.pool_fly.push(this.used_fly_effects[i]);
		}

		this.used_fly_effects.Clear();
	}


	public void show_destroy_effect(Vector3 position, Sprite sprite)
	{
		Transform effect_parent =
			CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>().effect_parent;

		GameObject obj = this.pool_destroy.pop();
		obj.SetActive(true);
		obj.transform.SetParent(effect_parent);
		obj.GetComponentInChildren<Image>().sprite = sprite;
		obj.transform.position = position;

		// 일정 시간 후 자동 숨김 처리 하고 오브젝트 풀에 반환시킴.
		obj.GetComponent<CDelayedHide>().hide(0.5f, (GameObject old) =>
		{
			old.SetActive(false);
			this.pool_destroy.push(old);
		});
	}


	public void show_hearts()
	{
		StartCoroutine(run_heart_effects());
	}


	IEnumerator run_heart_effects()
	{
		this.ef_heart.SetActive(true);

		yield return new WaitForSeconds(3.0f);
		this.ef_heart.SetActive(false);
	}
}

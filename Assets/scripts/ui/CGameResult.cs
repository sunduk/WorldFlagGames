using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CGameResult : MonoBehaviour {

	Text label_success_count;
	Text label_total_count;
	GameObject button_next;

	List<GameObject> star_gold;

	void Awake()
	{
		this.label_success_count = transform.FindChild("success_count").GetComponent<Text>();
		this.label_total_count = transform.FindChild("total_count").GetComponent<Text>();

		this.button_next = transform.FindChild("button_challenge").gameObject;
		transform.FindChild("button_retry").GetComponent<Button>().onClick.AddListener(this.on_retry);
		transform.FindChild("button_challenge").GetComponent<Button>().onClick.AddListener(this.on_next);
		transform.FindChild("button_back").GetComponent<Button>().onClick.AddListener(this.on_back);

		this.star_gold = new List<GameObject>();
		for (int i = 0; i < 3; ++i)
		{
			this.star_gold.Add(transform.FindChild(string.Format("star_gold_{0:D2}", (i + 1))).gameObject);
		}
	}


	void on_retry()
	{
		CUIManager.Instance.hide(UI_PAGE.GAME_RESULT);
		CGameLogic.Instance.restart_current();
	}


	void on_next()
	{
		CUIManager.Instance.hide(UI_PAGE.GAME_RESULT);
		CGameLogic.Instance.move_to_next_stage();
	}


	public void on_back()
	{
		CUIManager.Instance.hide(UI_PAGE.GAME_RESULT);
		CUIManager.Instance.show(UI_PAGE.STAGE_SELECT);
		CUIManager.Instance.show(UI_PAGE.MAIN_MENU);
		CStageSelect stage = CUIManager.Instance.get_uipage(UI_PAGE.STAGE_SELECT).GetComponent<CStageSelect>();
		stage.refresh(CGameLogic.Instance.current_stage);
	}


	public void refresh(int success_count, byte star_count)
	{
		//if (star_count < 2)
		//{
		//	this.button_next.SetActive(false);
		//}
		//else
		//{
		//	this.button_next.SetActive(true);
		//}

		this.label_success_count.text = success_count.ToString();
		this.label_total_count.text =
			CGameLogic.Instance.current_stage_attribute.flag_count.ToString();

		StartCoroutine(prize_action());
	}


	IEnumerator prize_action()
	{
		hide_all_goldstars();

		// 별표 연출 장면.
		yield return new WaitForSeconds(0.5f);

		byte star_count = CGameLogic.Instance.calc_star_count();

		if (star_count >= 3)
		{
			CEffectManager.Instance.show_hearts();
		}

		for (byte i = 0; i < star_count; ++i)
		{
			this.star_gold[i].SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
	}


	void hide_all_goldstars()
	{
		for (int i = 0; i < this.star_gold.Count; ++i)
		{
			this.star_gold[i].SetActive(false);
		}
	}


	void on_back_button()
	{
		on_back();
	}
}

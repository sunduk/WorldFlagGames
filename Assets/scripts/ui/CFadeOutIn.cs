using UnityEngine;
using System.Collections;

public class CFadeOutIn : MonoBehaviour {

	CUIImageFadeEffect fade_effect;
	UI_PAGE prev_ui_page;
	UI_PAGE next_ui_page;

	DefaultDelegate finished_callback;


	void Awake()
	{
		this.fade_effect = gameObject.GetComponentInChildren<CUIImageFadeEffect>();
	}


	public void update_ui_page(UI_PAGE prev_page, UI_PAGE next_page, DefaultDelegate finished_callback)
	{
		this.prev_ui_page = prev_page;
		this.next_ui_page = next_page;
		this.finished_callback = finished_callback;
	}


	void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(delay_and_next());
	}


	IEnumerator delay_and_next()
	{
		yield return new WaitForSeconds(this.fade_effect.get_duration());

		CUIManager.Instance.hide(this.prev_ui_page);
		CUIManager.Instance.show(this.next_ui_page);

		if (this.finished_callback != null)
		{
			this.finished_callback();
		}
	}
}

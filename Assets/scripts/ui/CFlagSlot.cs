using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CFlagSlot : MonoBehaviour {

	Image image;
	CTableFlags flag;


	void Awake()
	{
		this.image = GetComponent<Image>();
		GetComponent<Button>().onClick.AddListener(this.on_touch);
	}


	public void update_flag(CTableFlags flag, Sprite sprite)
	{
		this.flag = flag;
		this.image.sprite = sprite;
		this.image.SetNativeSize();
	}


	void on_touch()
	{
		CPlayRoom room = CUIManager.Instance.get_uipage(UI_PAGE.PLAY_ROOM).GetComponent<CPlayRoom>();
		room.on_touch_flag(this.flag);
	}


	public bool is_same(short flag_index)
	{
		return this.flag.flag_index == flag_index;
	}
}

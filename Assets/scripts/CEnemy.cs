using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CEnemy : MonoBehaviour {

	public CTableFlags flag { get; private set; }
	Image image;
	Text text;


	void Awake()
	{
		this.image = transform.FindChild("Image").GetComponent<Image>();
		this.text = transform.FindChild("Text").GetComponent<Text>();
	}


	public void update_flag(CTableFlags flag, Sprite sprite, bool visible_flag)
	{
		this.flag = flag;
		this.image.sprite = sprite;
		this.image.SetNativeSize();
		this.text.text = this.flag.country_name;

		if (!visible_flag)
		{
			this.image.sprite = CFlagSpriteManager.Instance.get_question_sprite();
		}
	}


	public bool is_same(short flag_index)
	{
		return this.flag.flag_index == flag_index;
	}


	public Sprite get_sprite()
	{
		return this.image.sprite;
	}


	public void hide(float duration)
	{
		StartCoroutine(delayed_hide(duration));
	}


	IEnumerator delayed_hide(float duration)
	{
		yield return new WaitForSeconds(duration);
		gameObject.SetActive(false);
	}
}

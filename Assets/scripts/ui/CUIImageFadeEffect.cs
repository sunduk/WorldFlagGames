using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CUIImageFadeEffect : MonoBehaviour {

	[SerializeField]
	float delay;

	[SerializeField]
	float duration;

	[SerializeField]
	bool fade_in_only;

	Image image;


	void Awake()
	{
		this.image = gameObject.GetComponent<Image>();
	}


	void OnEnable()
	{
		StopAllCoroutines();
		if (this.fade_in_only)
		{
			StartCoroutine(fade(1.0f, 0.0f));
		}
		else
		{
			StartCoroutine(fade_out_in());
		}
	}


	IEnumerator fade(float from, float to)
	{
		Color color = this.image.color;
		color.a = from;
		this.image.color = color;

		yield return new WaitForSeconds(this.delay);
		float begin_time = Time.time;

		while (true)
		{
			float t = (Time.time - begin_time) / this.duration;
			float val = EasingUtil.easeOutExpo(from, to, t);
			color.a = val;
			this.image.color = color;

			if (t >= 1.0f)
			{
				break;
			}

			yield return 0;
		}

		color.a = to;
		this.image.color = color;
	}


	IEnumerator fade_out_in()
	{
		yield return StartCoroutine(fade(0.0f, 1.0f));
		yield return StartCoroutine(fade(1.0f, 0.0f));
	}


	public float get_duration()
	{
		return this.duration;
	}
}

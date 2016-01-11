using UnityEngine;
using System.Collections.Generic;

public enum EFFECT_SOUND : byte
{
	HIGH_SCORE,
	LOW_SCORE,
	CORRECT,
	WRONG
}


public class CSoundManager : CSingletonMonobehaviour<CSoundManager>
{
	AudioSource bgm_controller;
	AudioSource effect_controller;

	AudioClip main_bgm;
	Dictionary<EFFECT_SOUND, AudioClip> effect_clips;
	Dictionary<AREA, AudioClip> bgm_clips;

	public bool sound_on_flag { get; private set; }


	void Awake()
	{
		this.bgm_controller = gameObject.AddComponent<AudioSource>();
		this.effect_controller = gameObject.AddComponent<AudioSource>();
		this.sound_on_flag = true;
	}


	public void load_all()
	{
		this.effect_clips = new Dictionary<EFFECT_SOUND, AudioClip>();
		this.effect_clips.Add(EFFECT_SOUND.HIGH_SCORE, Resources.Load<AudioClip>("sounds/Event 13"));
		this.effect_clips.Add(EFFECT_SOUND.LOW_SCORE, Resources.Load<AudioClip>("sounds/Event 5"));
		this.effect_clips.Add(EFFECT_SOUND.CORRECT, Resources.Load<AudioClip>("sounds/item_get"));
		this.effect_clips.Add(EFFECT_SOUND.WRONG, Resources.Load<AudioClip>("sounds/Wrong 1"));

		this.bgm_clips = new Dictionary<AREA, AudioClip>();
		this.bgm_clips.Add(AREA.asia, Resources.Load<AudioClip>("sounds/music/Deal with those Pieces"));
		this.bgm_clips.Add(AREA.africa, Resources.Load<AudioClip>("sounds/music/Enjoy It"));
		this.bgm_clips.Add(AREA.europe, Resources.Load<AudioClip>("sounds/music/Choose the Right One"));
		this.bgm_clips.Add(AREA.america, Resources.Load<AudioClip>("sounds/music/Focus on This"));
		this.bgm_clips.Add(AREA.oceania, Resources.Load<AudioClip>("sounds/music/Jewels Everywhere"));

		this.main_bgm = Resources.Load<AudioClip>("sounds/music/Deep Investigation");

		this.effect_controller.loop = false;
		this.bgm_controller.loop = true;
	}


	public void update_main_bgm_clip()
	{
		if (this.main_bgm != null)
		{
			this.bgm_controller.clip = this.main_bgm;
		}
	}


	public void update_area_bgm_clip(AREA area)
	{
		if (this.bgm_clips[area] != null)
		{
			this.bgm_controller.clip = this.bgm_clips[area];
		}
	}


	public void play_main_bgm()
	{
		if (!this.sound_on_flag)
		{
			return;
		}

		if (this.main_bgm != null)
		{
			this.bgm_controller.Play();
		}
	}


	public void play_bgm(AREA area)
	{
		if (!this.sound_on_flag)
		{
			return;
		}

		stop_bgm();
		if (!this.bgm_clips.ContainsKey(area))
		{
			return;
		}

		if (this.bgm_clips[area] != null)
		{
			this.bgm_controller.Play();
		}
	}


	public void pause_bgm()
	{
		this.bgm_controller.Pause();
	}


	public void stop_bgm()
	{
		this.bgm_controller.Stop();
	}


	public void volume_down()
	{
		this.bgm_controller.volume = 0.3f;
	}


	public void volume_up()
	{
		this.bgm_controller.volume = 1.0f;
	}


	public void play(EFFECT_SOUND sound_type)
	{
		if (!this.sound_on_flag)
		{
			return;
		}

		if (this.effect_clips[sound_type] != null)
		{
			this.effect_controller.PlayOneShot(this.effect_clips[sound_type]);
		}
	}


	public void toggle_sound(bool flag)
	{
		this.sound_on_flag = flag;
		if (this.sound_on_flag)
		{
			this.bgm_controller.Play();
		}
		else
		{
			pause_bgm();
		}
	}


	public int get_sound_state()
	{
		return (this.sound_on_flag == true) ? 1 : 0;
	}


	public void set_sound_state(int state)
	{
		if (state == 1)
		{
			toggle_sound(true);
		}
		else
		{
			toggle_sound(false);
		}
	}
}

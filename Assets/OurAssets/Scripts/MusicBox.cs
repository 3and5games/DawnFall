using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class MusicBox : MonoBehaviour, IInteractable 
{

	public AudioClip[] clips;
	public AudioClip activate;
	public AudioClip deactivate;

	private bool playing = false;
	private AudioSource _source;
	private AudioSource source
	{
		get
		{
			if(!_source)
			{
				_source = GetComponent<AudioSource> ();
			}
			return _source;
		}
	}

	#region IInteractable implementation

	public void Activate ()
	{
		if (playing) 
		{
			source.Stop ();
			source.PlayOneShot (deactivate);
			foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
			{
				var em = ps.emission;
				em.rateOverTime = 0;
				ps.Clear ();
			}
			playing = false;
		} 
		else 
		{
			if (clips.Length > 0) 
			{
				playing = true;
				AudioClip clip = clips [Random.Range (0, clips.Length)];
				source.PlayOneShot (activate);
				source.PlayOneShot (clip);
				foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
				{
					var em = ps.emission;
					em.rateOverTime = 5;
				}
				Invoke ("Stop", clip.length);
			}
		}
	}
	#endregion

	private void Stop()
	{
		foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
		{
			var em = ps.emission;
			em.rateOverTime = 0;
			ps.Clear ();
		}
		playing = false;
	}
}

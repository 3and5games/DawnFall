using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class MusicBox : MonoBehaviour, IInteractable 
{

	public AudioClip[] clips;
	public AudioClip activate;
	public AudioClip deactivate;
    public MeshRenderer glassRenderer;

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

    [ContextMenu("activate")]
	public void Activate ()
	{
        StopAllCoroutines();
		if (playing) 
		{
			source.Stop ();
			source.PlayOneShot (deactivate);
			playing = false;
            StartCoroutine(OffGlass());
            //glassRenderer.material.SetFloat("_ColorMult", -1);

        } 
		else 
		{
			if (clips.Length > 0) 
			{
				playing = true;
				AudioClip clip = clips [Random.Range (0, clips.Length)];
				source.PlayOneShot (activate);
				source.PlayOneShot (clip);
				Invoke ("Stop", clip.length);
                StartCoroutine(OnGlass());
                //glassRenderer.material.SetFloat("_ColorMult", 2);
            }
		}
	}
	#endregion

	private void Stop()
	{
		playing = false;
	}

    IEnumerator OffGlass()
    {
        while (glassRenderer.material.GetFloat("_ColorMult")>-1)
        {
            glassRenderer.material.SetFloat("_ColorMult", glassRenderer.material.GetFloat("_ColorMult")-Time.deltaTime*3);
            yield return null;
        }
       
    }

    IEnumerator OnGlass()
    {
        while (glassRenderer.material.GetFloat("_ColorMult") < 2)
        {
            glassRenderer.material.SetFloat("_ColorMult", glassRenderer.material.GetFloat("_ColorMult") + Time.deltaTime*3);
            yield return null;
        }
    }
}

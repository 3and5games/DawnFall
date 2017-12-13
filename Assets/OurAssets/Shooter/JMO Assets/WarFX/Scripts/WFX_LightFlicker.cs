using UnityEngine;
using System.Collections;

/**
 *	Rapidly sets a light on/off.
 *	
 *	(c) 2015, Jean Moreno
**/

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
	public float time = 0.1f;	
	
	void Start ()
	{
        GetComponent<Light>().enabled = false;
    }

    public void Flick()
    {
        CancelInvoke("Off");
        GetComponent<Light>().enabled = true;
        Invoke("Off", time);
    }

	private void Off()
    {
        GetComponent<Light>().enabled = false;
    }
}

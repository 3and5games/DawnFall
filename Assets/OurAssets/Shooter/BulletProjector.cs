using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjector : MonoBehaviour {

    public float Size;
    public float LifeTime;
    public float HideTime;
    public Material ProjectorMaterial;
    private Material currentMaterial;

	// Use this for initialization
	void Start () {
        Projector projector = GetComponent<Projector>();
        projector.orthographicSize = Random.Range(0.8f, 1.2f)*Size;
        projector.material = new Material(ProjectorMaterial);
        currentMaterial = projector.material;
        transform.Rotate(Vector3.forward, Random.Range(0,360), Space.Self);
        Invoke("Hide", LifeTime);
	}
	
	void Hide()
    {
        StartCoroutine(FadeOut());
        Destroy(gameObject, HideTime);
    }

    private IEnumerator FadeOut()
    {
        float intencity = currentMaterial.GetFloat("_Power");
        while (currentMaterial.GetFloat("_Power") >0)
        {
            intencity -= Time.deltaTime / HideTime;
            currentMaterial.SetFloat("_Power", intencity);
            yield return null;
        }
        yield return null;
    }
}

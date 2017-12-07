using UnityEngine;

public class Spectrator : MonoBehaviour
{
    void Update()
    {
        var lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }
}

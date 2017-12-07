using UnityEngine;
using System.Collections;

public class FootStepTrigger : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<Terrain>() != null)
                transform.root.SendMessage("StepOnTerrain", transform.position, SendMessageOptions.DontRequireReceiver);
            else
            {
                var stepHandle = other.GetComponent<FootStepHandler>();
                var renderer = other.GetComponent<Renderer>();

                if (renderer != null && renderer.material != null)
                {
                    var index = 0;
                    var _name = string.Empty;
                    if (stepHandle != null && stepHandle.material_ID > 0)
                        index = stepHandle.material_ID;
                    if (stepHandle)
                    {
                        switch (stepHandle.stepHandleType)
                        {
                            case FootStepHandler.StepHandleType.materialName:
                                _name = renderer.materials[index].name;
                                break;
                            case FootStepHandler.StepHandleType.textureName:
                                _name = renderer.materials[index].mainTexture.name;
                                break;
                        }
                    }
                    else
                        _name = renderer.materials[index].name;
                    transform.root.SendMessage("StepOnMesh", _name, SendMessageOptions.DontRequireReceiver);
                }
            }           
        }
    }    
}

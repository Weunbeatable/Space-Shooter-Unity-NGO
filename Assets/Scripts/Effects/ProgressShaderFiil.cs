using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProgressShaderFiil : NetworkBehaviour
{
    public Renderer fillRenderer;
    public MeshFilter meshFilter;
    float _FillRateValue;
    float progressBorder;
    float stepSize;
    Material objectMat;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(fillRenderer == null)
        {
            Debug.LogError("Expected a render to be assigned here ");
            return;
        }
        float progressBoarder = meshFilter.mesh.bounds.size.x / 2f; // set orientation accordingly
        fillRenderer.material.SetFloat("_progressBorder", progressBorder);
        // for progress bar
        fillRenderer.material.SetFloat("_FillRtate", _FillRateValue); //initial value
        
    }

    public void ChangeValue(bool increase) // setup vaue change for progress bar
        //TODO: modify this to fit he healing station value. 
    {
        // if increase param is true, the progrss bar pregresses otherwise it deprogresses. 
        if (increase)
        {
            _FillRateValue += stepSize; // increae fill progress
        }
        else
        {
            _FillRateValue -= stepSize; // decrease progress 
        }
        objectMat.SetFloat("_FillRate", _FillRateValue); // update fill value 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;
using UnityEngine.Rendering.Universal;
using System.Linq;
public class LowHealthShader : NetworkBehaviour
{
    [Header("material renderer properties")]
    [Tooltip("This field holds the pass material that will be loaded when a health condition is met")]
    [SerializeField] public Material screenspaceMat;
    public UniversalRendererData rendererData;
    void Start()
    {
       var targetmat =  rendererData.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault();
        targetmat.passMaterial = null;
    }

    // Update is called once per frame
    void Update()
    {

        var targetmat = rendererData.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault();
        if (IsOwner)
        {
            if (this.gameObject.TryGetComponent(out Health health))
            {
                if (health.currenthealth.Value <= 30)
                {
                    // health is critical. 
                    // TODO alter the shader depending on health value over time. 

                    targetmat.passMaterial = screenspaceMat;
                }
                else
                {
                    targetmat.passMaterial = null;
                }
            }
        }
    }
}

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
    [SerializeField] private int criticalHealthValue = 30;

    public UniversalRendererData rendererData;
    private FullScreenPassRendererFeature passRendererFeature;

    private bool isInitalized;
    private bool defaultFeatureState; 
    public override void OnNetworkSpawn()
    {
        // learned the hard way this should be treated like start method. Weird situations could happen like
        // a user spawned into an ongoing game and the start method gets called for everyone. 
        passRendererFeature = rendererData.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault();

        // check if anything is null
        if (passRendererFeature == null)
        {
            Debug.LogError("No FullscreenRenderFeature found in the assigned slot");

            enabled = false; // dont use the pass render feature at that point turn off this component/feature 
            return;
        }

        if(!TryGetComponent(out Health health))
        {
            Debug.LogError("Somehow there is no health component here");
            enabled = false;
            return;
        }

        // storing initial value 
        defaultFeatureState = passRendererFeature.isActive;

        // don't enable low health effect at start
        passRendererFeature.SetActive(false);

        isInitalized = true;
    }
    // Update is called once per frame
    void Update()
    {

        var targetmat = rendererData.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault();
        if (!IsOwner) { return; }
            if (this.gameObject.TryGetComponent(out Health health))
            {
                if (health.currenthealth.Value <= criticalHealthValue)
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

    public override void OnNetworkDespawn()
    {
        // handle cleanup
        RestoreRendererFeature();
    }
    private void OnDisable()
    {
        RestoreRendererFeature();
    }
    private void RestoreRendererFeature()
    {
        if(!isInitalized || passRendererFeature == null)
        {
            return;
        }
        passRendererFeature.SetActive(defaultFeatureState);

        isInitalized=false;
    }
}

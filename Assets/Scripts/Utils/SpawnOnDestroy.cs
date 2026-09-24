using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private void OnDestroy()
    {
       GameObject SmokeParticles =  Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(SmokeParticles,1.5f);
    }
}

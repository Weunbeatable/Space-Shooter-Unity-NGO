using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    //Will be a static list of spawn points to choose from so there is only one instance shared
    // this allows for the spawn point monobehavoiur exists players can be added or removed from list when 
    // spawned or destroyed, then code will randomly choose a place to spawn from in the list. 
   
    private static List<SpawnPoints> spawnPoints = new List<SpawnPoints>();


    private void OnEnable()
    {
        spawnPoints.Add(this);
    }
    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }
    // when other scripts want to reference the spawn positions
    public static Vector3 GetRandomSpawnPos()
    {
        // check if list is empty
        if(spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }
       int randomSpawnPos = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomSpawnPos].gameObject.transform.position;
    }

    private void OnDrawGizmos()
    {
        // object will have an icon or mesh only visible in scene view to make it easily visible 
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 10);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonParticles : MonoBehaviour
{
    // This class simply spawns the specified particle prefabs whenever the object is created or destroyed
    [SerializeField] private GameObject spawnParticles;
    [SerializeField] private GameObject deathParticles;
    private bool isQuitting;
    private void Start()
    {
        if(spawnParticles != null)
        {
            Instantiate(spawnParticles, transform.position, Quaternion.identity);
        }
    }
    private void OnDestroy()
    {
        // Also check that the application is not quitting
        if (deathParticles != null && !isQuitting)
        {
            Instantiate(spawnParticles, transform.position, Quaternion.identity);
        }
    }
    // If the application is quitting then don't spawn OnDestroy particles
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

}

using UnityEngine;

public class PlayerTracker : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
}

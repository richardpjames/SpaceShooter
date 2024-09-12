using UnityEngine;

public class PlayerTracker : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMovement player = Object.FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
}

using UnityEngine;

public class PlayerTracker : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        Player player = Object.FindObjectOfType<Player>();
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
}

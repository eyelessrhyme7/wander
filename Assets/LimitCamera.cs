using UnityEngine;

public class LimitCamera : MonoBehaviour
{
    public GameObject Player;

    private void LateUpdate()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y+10, Player.transform.position.z);
      // Set the camera's rotation based on the player's rotation
        transform.rotation = Quaternion.Euler(90, Player.transform.eulerAngles.y, 0);
    }
}
 
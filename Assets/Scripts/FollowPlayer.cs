using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float Speed = 1;

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            GameManager.Instance.Player.position, 
            Speed * Time.deltaTime);
    }
}

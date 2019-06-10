using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
    }

    void Update()
    {
        rb.AddForce(new Vector2(Input.GetAxis("Horizontal") / 200f, Input.GetAxis("Vertical") / 200f));
        rb.velocity = rb.velocity.normalized;
        KeepInBounds();
    }
}

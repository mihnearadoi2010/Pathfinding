using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private const float acceleration = 40;
    public Vector3 Velocity { get; private set; } = Vector3.zero;
    private const float maxVelocity = 30;

    private Vector3 moveDirection;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private InputActionReference move;

    // Update is called once per frame
    void Update()
    {
        var input = move.action.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x, 0, input.y);
    }

    private void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
        {
            Velocity += moveDirection.normalized * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            Velocity = Vector3.MoveTowards(Velocity, Vector3.zero, acceleration * Time.fixedDeltaTime);
        }

        Velocity = Vector3.ClampMagnitude(Velocity, maxVelocity);
        var finalVelocity = Velocity - rb.linearVelocity;
        rb.AddForce(finalVelocity, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            float dot = Vector3.Dot(Velocity, contact.normal);

            if (dot < 0)
            {
                Velocity -= contact.normal * dot * 1.2f;
            }
        }
    }
}
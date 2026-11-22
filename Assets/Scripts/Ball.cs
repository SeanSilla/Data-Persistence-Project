using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private float m_MaxSpeed = 3.0f;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        if (DataManager.Instance != null)
        {
            m_MaxSpeed = DataManager.Instance.BallSpeed;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        var velocity = m_Rigidbody.linearVelocity;

        // after a collision we accelerate a bit
        velocity += velocity.normalized * 0.01f;

        // check if we are not going totally vertically as this would lead to being stuck
        if (Vector3.Dot(velocity.normalized, Vector3.up) < 0.1f)
        {
            velocity += velocity.y > 0 ? Vector3.up * 0.5f : Vector3.down * 0.5f;
        }

        // max velocity from Settings
        if (velocity.magnitude > m_MaxSpeed)
        {
            velocity = velocity.normalized * m_MaxSpeed;
        }

        m_Rigidbody.linearVelocity = velocity;
    }
}
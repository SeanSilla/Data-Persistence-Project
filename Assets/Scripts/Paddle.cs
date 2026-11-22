using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public float Speed = 2.0f;
    public float MaxMovement = 2.0f;

    private InputAction m_MoveAction;

    void Awake()
    {
        m_MoveAction = new InputAction("Move", InputActionType.Value);
        m_MoveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/d")
            .With("Positive", "<Keyboard>/rightArrow");
    }

    void OnEnable()
    {
        m_MoveAction.Enable();
    }

    void OnDisable()
    {
        m_MoveAction.Disable();
    }

    void Start()
    {
        if (DataManager.Instance != null)
        {
            Speed = DataManager.Instance.PaddleSpeed;
        }
    }

    void Update()
    {
        float input = m_MoveAction.ReadValue<float>();

        Vector3 pos = transform.position;
        pos.x += input * Speed * Time.deltaTime;

        if (pos.x > MaxMovement)
            pos.x = MaxMovement;
        else if (pos.x < -MaxMovement)
            pos.x = -MaxMovement;

        transform.position = pos;
    }
}
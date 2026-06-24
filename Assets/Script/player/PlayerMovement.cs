using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý tất cả các chuyển động của người chơi, bao gồm cả di chuyển bằng lệnh thoại.
/// Yêu cầu component Rigidbody.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("Tốc độ di chuyển tới/lui.")]
    [SerializeField] private float moveSpeed = 2.0f;
    [Tooltip("Tốc độ xoay (độ/giây).")]
    [SerializeField] private float turnSpeed = 90.0f;

    private Rigidbody rb;
    private Vector3 movementInput = Vector3.zero;
    private float turnInput = 0f;
    private Coroutine moveToTargetCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Đóng băng xoay trên các trục không mong muốn để tránh nhân vật bị ngã
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Chỉ xử lý di chuyển hướng nếu không đang trong quá trình di chuyển tự động đến mục tiêu
        if (moveToTargetCoroutine == null)
        {
            MovePlayer();
            TurnPlayer();
        }
    }

    private void MovePlayer()
    {
        if (movementInput != Vector3.zero)
        {
            // Di chuyển dựa trên hướng nhìn hiện tại của người chơi
            Vector3 moveVelocity = transform.TransformDirection(movementInput) * moveSpeed;
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        }
    }

    private void TurnPlayer()
    {
        if (turnInput != 0f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    /// <summary>
    /// Xử lý các lệnh di chuyển hướng liên tục (tiến, lùi, xoay).
    /// </summary>
    public void HandleDirectionalCommand(string command)
    {
        StopMoveToTarget(); // Dừng di chuyển tự động đến mục tiêu nếu có

        switch (command.ToLower())
        {
            case "đi thẳng":
            case "tiến lên":
                movementInput = Vector3.forward;
                turnInput = 0;
                break;
            case "lùi lại":
                movementInput = Vector3.back;
                turnInput = 0;
                break;
            case "quay trái":
                turnInput = -1f;
                movementInput = Vector3.zero;
                break;
            case "quay phải":
                turnInput = 1f;
                movementInput = Vector3.zero;
                break;
            case "dừng lại":
                movementInput = Vector3.zero;
                turnInput = 0f;
                break;
        }
    }

    /// <summary>
    /// Bắt đầu di chuyển người chơi đến một vị trí cụ thể.
    /// </summary>
    public void StartMoveToTarget(Vector3 targetPosition)
    {
        StopMoveToTarget(); // Dừng coroutine cũ nếu đang chạy
        moveToTargetCoroutine = StartCoroutine(MoveToTargetRoutine(targetPosition));
    }

    private void StopMoveToTarget()
    {
        if (moveToTargetCoroutine != null)
        {
            StopCoroutine(moveToTargetCoroutine);
            moveToTargetCoroutine = null;
        }
        HandleDirectionalCommand("dừng lại"); // Đảm bảo người chơi dừng hẳn
    }

    private IEnumerator MoveToTargetRoutine(Vector3 destination)
    {
        Debug.Log($"Người chơi đang di chuyển đến {destination}");
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            // Di chuyển về phía đích
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination; // Đảm bảo đến đúng vị trí
        moveToTargetCoroutine = null; // Hoàn thành
        Debug.Log("Người chơi đã đến nơi.");
    }
}

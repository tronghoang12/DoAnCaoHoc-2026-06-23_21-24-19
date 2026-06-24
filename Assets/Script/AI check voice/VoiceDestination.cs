using UnityEngine;

/// <summary>
/// Gắn script này vào bất kỳ GameObject nào đóng vai trò là đích đến.
/// Đối tượng sẽ tự động đăng ký với VoiceCommandController.
/// </summary>
public class VoiceDestination : MonoBehaviour
{
    private void OnEnable()
    {
        VoiceCommandController.RegisterDestination(this);
    }

    private void OnDisable()
    {
        VoiceCommandController.UnregisterDestination(this);
    }
}
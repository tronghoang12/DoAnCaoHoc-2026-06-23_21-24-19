using UnityEngine;

/// <summary>
/// Gắn script này vào bất kỳ GameObject nào bạn muốn có thể di chuyển bằng giọng nói.
/// Đối tượng sẽ tự động đăng ký với VoiceCommandController.
/// </summary>
public class VoiceControllable : MonoBehaviour
{
    private void OnEnable()
    {
        VoiceCommandController.RegisterControllable(this);
    }

    private void OnDisable()
    {
        VoiceCommandController.UnregisterControllable(this);
    }
}
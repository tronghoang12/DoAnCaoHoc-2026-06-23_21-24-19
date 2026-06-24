using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

public class VoiceCommandController : MonoBehaviour
{
    [Tooltip("Tốc độ di chuyển của các đối tượng.")]
    [SerializeField] private float moveSpeed = 3f;

    // Danh sách tĩnh để lưu các đối tượng đã đăng ký
    private static readonly List<VoiceControllable> _controllables = new List<VoiceControllable>();
    private static readonly List<VoiceDestination> _destinations = new List<VoiceDestination>();

    private DictationRecognizer dictationRecognizer;
    
    // Sử dụng Regex để phân tích lệnh một cách linh hoạt
    // Mẫu lệnh: "(di chuyển|move) [TÊN_ĐỐI_TƯỢNG] (đến|to) [TÊN_ĐÍCH]"
    private const string MoveCommandPattern = @"(di chuyển|move) (.+) (đến|to) (.+)";

    void Start()
    {
        // Sử dụng DictationRecognizer để nhận dạng giọng nói tự do.
        // LƯU Ý: Cần kết nối internet và quyền sử dụng micro trên một số nền tảng.
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Lệnh nhận dạng được: {0}", text);
            ProcessVoiceCommand(text);
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            // Khởi động lại trình nhận dạng nếu nó dừng lại
            if (dictationRecognizer.Status != SpeechSystemStatus.Running)
            {
                dictationRecognizer.Start();
            }
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Lỗi nhận dạng giọng nói: {0}; HResult = {1}.", error, hresult);
        };

        dictationRecognizer.Start();
        Debug.Log("Hệ thống điều khiển giọng nói đã sẵn sàng. Hãy ra lệnh (ví dụ: 'Di chuyển khối hộp đến điểm A').");
    }

    private void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }

    // Các hàm tĩnh công khai để đăng ký/hủy đăng ký
    public static void RegisterControllable(VoiceControllable obj) { if (!_controllables.Contains(obj)) _controllables.Add(obj); }
    public static void UnregisterControllable(VoiceControllable obj) { _controllables.Remove(obj); }
    public static void RegisterDestination(VoiceDestination dest) { if (!_destinations.Contains(dest)) _destinations.Add(dest); }
    public static void UnregisterDestination(VoiceDestination dest) { _destinations.Remove(dest); }

    private void ProcessVoiceCommand(string command)
    {
        Match match = Regex.Match(command, MoveCommandPattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            string objectName = match.Groups[2].Value.Trim();
            string destName = match.Groups[4].Value.Trim();

            Debug.Log($"Phân tích lệnh: Di chuyển '{objectName}' đến '{destName}'");

            // Tìm đối tượng và đích đến theo tên (không phân biệt hoa thường)
            var objectToMove = _controllables.FirstOrDefault(c => c.gameObject.name.Equals(objectName, System.StringComparison.OrdinalIgnoreCase));
            var destination = _destinations.FirstOrDefault(d => d.gameObject.name.Equals(destName, System.StringComparison.OrdinalIgnoreCase));

            if (objectToMove == null)
            {
                Debug.LogError($"Không tìm thấy đối tượng có thể di chuyển tên là '{objectName}'!");
                return;
            }
            if (destination == null)
            {
                Debug.LogError($"Không tìm thấy đích đến tên là '{destName}'!");
                return;
            }

            // Bắt đầu di chuyển
            StartCoroutine(MoveObject(objectToMove.gameObject, destination.transform.position));
        }
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 destination)
    {
        Debug.Log($"Đang di chuyển {obj.name} đến {destination}");
        while (Vector3.Distance(obj.transform.position, destination) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null; // Chờ đến frame tiếp theo
        }
        obj.transform.position = destination; // Gán thẳng đến vị trí cuối cùng để đảm bảo chính xác
        Debug.Log($"{obj.name} đã đến nơi.");
    }
}
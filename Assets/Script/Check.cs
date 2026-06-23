using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class Check : MonoBehaviour
{
    string TxtKeyWord;
    GameObject _mainCamera;
    GameObject _target;
    GameObject _gb2ShowRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CheckingTxtKeyWord(string TxtInit)
    {
        //tạo biểu thực regex để kiểm tra toán tử
        string pattern = $@"\b{Regex.Escape(TxtKeyWord)}\b";
        
        //kiểm tra toán tử (Không phân biệt hoa thường)
        if (Regex.IsMatch(TxtInit, pattern, RegexOptions.IgnoreCase))
        {
            Debug.Log($"Chuỗi có chứa chính xác từ '{TxtKeyWord}'(Không phân biệt hoa thường).");

            if(TxtKeyWord == "Information")
            {
                _mainCamera.SetActive(false);
                _mainCamera.transform.position = new Vector3(_target.transform.position.x, _mainCamera.transform.position.y, _target.transform.position.z);
                _mainCamera.transform.eulerAngles = new Vector3(0,0,0);
                _gb2ShowRoom.SetActive(true);
            }
            return true;
        }

        Debug.Log($"Chuỗi không chứa từ '{TxtKeyWord}'.");
        return false;

    }
}



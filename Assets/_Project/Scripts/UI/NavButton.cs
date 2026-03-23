using UnityEngine;
using UnityEngine.UI;

public class NavButton : MonoBehaviour
{
    [SerializeField] private GameState stateToSwitch;
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        GameManager.Instance.ChangeState(stateToSwitch);
    }
}

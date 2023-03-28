using UnityEngine;

public class GlobalSceneManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
        
    private void Awake()
    {
        GlobalPlayerInput.Instance.GlobalCamera = camera;
    }
}
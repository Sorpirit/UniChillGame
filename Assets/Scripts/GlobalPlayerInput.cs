using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

public class GlobalPlayerInput
{
    public static GlobalPlayerInput Instance => _instance ??= new GlobalPlayerInput();

    public static PlayerInputActions InputInstance => Instance.Input;

    [CanBeNull] private static GlobalPlayerInput _instance;
        
    public PlayerInputActions Input { get; }

    [CanBeNull] public Camera GlobalCamera { get; set; }

    private GlobalPlayerInput()
    {
        Assert.IsNull(_instance);
        _instance = this;
            
        Input = new PlayerInputActions();
        Input.Enable();

    }
}
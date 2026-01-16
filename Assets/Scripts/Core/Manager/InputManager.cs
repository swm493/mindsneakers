using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{   
    private InputSystem_Actions inputActions;
    public InputSystem_Actions.PlayerActions player;
    public InputSystem_Actions.UIActions UI;

    protected override void Awake()
    {
        base.Awake();

        inputActions = new InputSystem_Actions();
        player = inputActions.Player;
        UI = inputActions.UI;

        inputActions.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public void EnableInput()
    {
        inputActions.Enable();
    }
    public void DisableInput()
    {
        inputActions.Disable();
    }
}
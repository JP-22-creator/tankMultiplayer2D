using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controlls;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{   // scriptable Object means it can be created in the menu
    // IPlayerActions comes from Controlls and is the input ActionMap 

    private Controlls controlls;

    public event Action<bool> OnPrimaryFireEvent;
    public event Action<Vector2> OnMoveEvent;   // option 1

    public Vector2 AimPosition { get; private set; }   // option 2


    private void OnEnable()
    {
        if (controlls == null)
        {
            controlls = new Controlls();
            controlls.Player.SetCallbacks(this); // we pass this bcs it need an IPlayerActions
        }

        controlls.Player.Enable(); // this enables the Player inputs
                                   // if we changed to somehting else like animal we can disable this 

    }


    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(
            context.ReadValue<Vector2>()
            );
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed) OnPrimaryFireEvent?.Invoke(true);
        else if (context.canceled) OnPrimaryFireEvent?.Invoke(false);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }
}

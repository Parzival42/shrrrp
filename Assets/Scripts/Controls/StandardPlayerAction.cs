using InControl;

public class StandardPlayerAction : PlayerActionSet
{
    #region Left Stick
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerTwoAxisAction Move;

    public PlayerAction Jump;
    public PlayerAction Dash;

    public PlayerAction Pause;
    #endregion

    #region Right Stick
    public PlayerAction RightStickLeft;
    public PlayerAction RightStickRight;
    public PlayerAction RightStickUp;
    public PlayerAction RightStickDown;
    public PlayerTwoAxisAction RightStick;
    #endregion

    public PlayerAction NullAction = null;

    public StandardPlayerAction()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

        RightStickLeft = CreatePlayerAction("Right Stick Left");
        RightStickRight = CreatePlayerAction("Right Stick Right");
        RightStickUp = CreatePlayerAction("Right Stick Up");
        RightStickDown = CreatePlayerAction("Right Stick Down");
        RightStick = CreateTwoAxisPlayerAction(RightStickLeft, RightStickRight, RightStickDown, RightStickUp);

        Jump = CreatePlayerAction("Jump");
        Dash = CreatePlayerAction("Dash");

        Pause = CreatePlayerAction("Pause");
    }

    public bool IsNullAction()
    {
        return NullAction != null ? true : false;
    }

    public static StandardPlayerAction CreateNullBinding()
    {
        StandardPlayerAction p = new StandardPlayerAction();
        p.NullAction = p.CreatePlayerAction("Null");
        p.NullAction.AddDefaultBinding(InputControlType.None);
        
        return p;
    }

    public static StandardPlayerAction CreateStandardGamePadBinding()
    {
        StandardPlayerAction playerAction = new StandardPlayerAction();

        // Left
        playerAction.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerAction.Left.AddDefaultBinding(InputControlType.DPadLeft);

        // Right Stick Left
        playerAction.RightStickLeft.AddDefaultBinding(InputControlType.RightStickLeft);

        // Right
        playerAction.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerAction.Right.AddDefaultBinding(InputControlType.DPadRight);

        // Right Stick Right
        playerAction.RightStickRight.AddDefaultBinding(InputControlType.RightStickRight);

        // Up
        playerAction.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerAction.Up.AddDefaultBinding(InputControlType.DPadUp);

        // Right Stick Up
        playerAction.RightStickUp.AddDefaultBinding(InputControlType.RightStickUp);

        // Down
        playerAction.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        playerAction.Down.AddDefaultBinding(InputControlType.DPadDown);

        // Right Stick Down
        playerAction.RightStickDown.AddDefaultBinding(InputControlType.RightStickDown);

        // Jump
        playerAction.Jump.AddDefaultBinding(InputControlType.Action1);
        playerAction.Jump.AddDefaultBinding(InputControlType.Action2);

        // Dash
        playerAction.Dash.AddDefaultBinding(InputControlType.Action3);
        playerAction.Dash.AddDefaultBinding(InputControlType.Action4);

        // Xbox one specific
        playerAction.Pause.AddDefaultBinding(InputControlType.Menu);
        // Xbox 360 specific
        playerAction.Pause.AddDefaultBinding(InputControlType.Start);

        return playerAction;
    }

    public static StandardPlayerAction CreateStandardGamePadBinding(InputDevice inputDevice)
    {
        StandardPlayerAction playerAction = CreateStandardGamePadBinding();
        playerAction.Device = inputDevice;
        return playerAction;
    }

    public static StandardPlayerAction CreateStandardKeyboardBinding()
    {
        StandardPlayerAction playerAction = new StandardPlayerAction();

        // Left
        playerAction.Left.AddDefaultBinding(Key.A);
        playerAction.RightStickLeft.AddDefaultBinding(Key.LeftArrow);

        // Right
        playerAction.Right.AddDefaultBinding(Key.D);
        playerAction.RightStickRight.AddDefaultBinding(Key.RightArrow);

        // Up
        playerAction.Up.AddDefaultBinding(Key.W);
        playerAction.RightStickUp.AddDefaultBinding(Key.UpArrow);

        // Down
        playerAction.Down.AddDefaultBinding(Key.S);
        playerAction.RightStickDown.AddDefaultBinding(Key.DownArrow);

        // Jump
        playerAction.Jump.AddDefaultBinding(Key.Space);
        playerAction.Jump.AddDefaultBinding(Key.Return);

        // Dash
        playerAction.Dash.AddDefaultBinding(Key.LeftAlt);
        playerAction.Dash.AddDefaultBinding(Key.LeftControl);

        // Xbox one specific
        playerAction.Pause.AddDefaultBinding(Key.Escape);
        playerAction.Pause.AddDefaultBinding(Key.Backspace);

        return playerAction;
    }

    public static StandardPlayerAction CreateStandardKeyboardBinding(InputDevice inputDevice)
    {
        StandardPlayerAction playerAction = CreateStandardKeyboardBinding();
        playerAction.Device = inputDevice;
        return playerAction;
    }
}
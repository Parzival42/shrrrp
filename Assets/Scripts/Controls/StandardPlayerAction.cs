using InControl;

public class StandardPlayerAction : PlayerActionSet
{
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;

    public PlayerAction Jump;
    public PlayerAction Dash;

    public PlayerAction Pause;

    public PlayerTwoAxisAction Move;

    public PlayerAction NullAction = null;

    public StandardPlayerAction()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        
        Jump = CreatePlayerAction("Jump");
        Dash = CreatePlayerAction("Dash");

        Pause = CreatePlayerAction("Pause");

        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
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

        // Right
        playerAction.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerAction.Right.AddDefaultBinding(InputControlType.DPadRight);

        // Up
        playerAction.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerAction.Up.AddDefaultBinding(InputControlType.DPadUp);

        // Down
        playerAction.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        playerAction.Down.AddDefaultBinding(InputControlType.DPadDown);

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
        playerAction.Left.AddDefaultBinding(Key.LeftArrow);
        playerAction.Left.AddDefaultBinding(Key.A);

        // Right
        playerAction.Right.AddDefaultBinding(Key.RightArrow);
        playerAction.Right.AddDefaultBinding(Key.D);

        // Up
        playerAction.Up.AddDefaultBinding(Key.UpArrow);
        playerAction.Up.AddDefaultBinding(Key.W);

        // Down
        playerAction.Down.AddDefaultBinding(Key.DownArrow);
        playerAction.Down.AddDefaultBinding(Key.S);

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
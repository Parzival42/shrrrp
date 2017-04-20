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

    public static StandardPlayerAction CreateStandardBinding()
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
}
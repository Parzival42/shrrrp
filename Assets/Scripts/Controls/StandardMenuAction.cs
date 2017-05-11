using InControl;

public class StandardMenuAction : PlayerActionSet {
	public PlayerAction Submit;
        public PlayerAction Cancel;
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;

        // Menu scenes use left stick only
        public PlayerTwoAxisAction Move;

        public StandardMenuAction()
        {
            Submit = CreatePlayerAction( "Submit" );
            Cancel = CreatePlayerAction( "Cancel" );
            Left = CreatePlayerAction( "Left" );
            Right = CreatePlayerAction( "Right" );
            Up = CreatePlayerAction( "Up" );
            Down = CreatePlayerAction( "Down" );
            Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
        }

        public static StandardMenuAction CreateStandardBinding()
    {
        StandardMenuAction playerAction = new StandardMenuAction();

        // ______________ Gamepad Bindings ______________
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

        // Submit
        playerAction.Submit.AddDefaultBinding(InputControlType.Action1);

        // Cancel
        playerAction.Cancel.AddDefaultBinding(InputControlType.Action2);

        // ______________ Keyboard Bindings ______________
         // Left
        playerAction.Left.AddDefaultBinding(Key.A);
        playerAction.Left.AddDefaultBinding(Key.LeftArrow);

        // Right
        playerAction.Right.AddDefaultBinding(Key.D);
        playerAction.Right.AddDefaultBinding(Key.RightArrow);

        // Up
        playerAction.Up.AddDefaultBinding(Key.W);
        playerAction.Up.AddDefaultBinding(Key.UpArrow);

        // Down
        playerAction.Down.AddDefaultBinding(Key.S);
        playerAction.Down.AddDefaultBinding(Key.DownArrow);

        // Submit
        playerAction.Submit.AddDefaultBinding(Key.Space);
        playerAction.Submit.AddDefaultBinding(Key.Return);

        // Cancel
        playerAction.Cancel.AddDefaultBinding(Key.Escape);
        playerAction.Cancel.AddDefaultBinding(Key.Backspace);

        return playerAction;
    }

    public static StandardMenuAction CreateStandardBinding(InputDevice inputDevice)
    {
        StandardMenuAction playerAction = CreateStandardBinding();
        playerAction.Device = inputDevice;
        return playerAction;
    }
}

using InControl;

public class StandardPlayerAction : PlayerActionSet
{
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;

    public PlayerAction Jump;
    public PlayerAction Dash;

    public PlayerTwoAxisAction Move;

    public StandardPlayerAction()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");

        Jump = CreatePlayerAction("Jump");
        Dash = CreatePlayerAction("Dash");

        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
    }
}
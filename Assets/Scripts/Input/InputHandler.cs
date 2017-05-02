public delegate void JumpPerformedHandler();
public delegate void SecondJumpPerformedHandler();
public delegate void LandedOnGroundHandler();
public delegate void SpecialMoveHandler();

public interface InputHandler
{
    #region Events
    event JumpPerformedHandler OnJump;
    event SecondJumpPerformedHandler OnSecondJump;
    event LandedOnGroundHandler OnLandedOnGround;
    event SpecialMoveHandler OnPlayerCut;
    #endregion

    bool IsGrounded { get; }
    float HorizontalInputValue { get; set; }
    float VerticalInputValue { get; set; }

    void HandleInput();
    void OnPlayerCutMove();
}
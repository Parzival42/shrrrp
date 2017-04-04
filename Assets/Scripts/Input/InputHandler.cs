public delegate void JumpPerformedHandler();
public delegate void SecondJumpPerformedHandler();
public delegate void LandedOnGroundHandler();

public interface InputHandler
{
    #region Events
    event JumpPerformedHandler OnJump;
    event SecondJumpPerformedHandler OnSecondJump;
    event LandedOnGroundHandler OnLandedOnGround;
    #endregion

    bool MovementEnabled { get; set; }
    bool IsGrounded { get; }
    float HorizontalInputValue { get; set; }
    float VerticalInputValue { get; set; }

    void HandleInput();
}
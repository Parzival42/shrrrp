public interface InputHandler
{
    bool MovementEnabled { get; set; }
    float HorizontalInputValue { get; set; }
    float VerticalInputValue { get; set; }

    void HandleInput();
}
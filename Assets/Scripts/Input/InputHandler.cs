public interface InputHandler
{
    bool MovementEnabled { get; set; }

    void HandleInput();
}

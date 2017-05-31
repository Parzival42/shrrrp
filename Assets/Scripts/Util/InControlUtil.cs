using InControl;

public class InControlUtil
{
    public static bool IsKeyboardDevice(InputDevice inputDevice)
    {
        return inputDevice.GetType() == typeof(InputDevice) ? true : false;
    }
}
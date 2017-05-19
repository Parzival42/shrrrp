using UnityEngine;
using UnityEngine.Audio;

public class TimeUtil
{
    private static AudioMixer mainMixer;
    private static string PITCH_PARAM = "Pitch";

    static TimeUtil()
    {
        mainMixer = Resources.Load<AudioMixer>("ShrrrpMixer");
        if (mainMixer == null)
            Debug.LogError("Could not find ShrrrpMixer! (Maybe it was moved or renamed in Resources folder?)");
    }

    /// <summary>
    /// Scales the time to the given timescale and then back to the original value.
    /// </summary>
    /// <param name="to">Target time scale</param>
    /// <param name="time">Time of the tween</param>
    /// <param name="easeType">Ease type of the tween</param>
    public static void TimescalePingPong(float to, float time, LeanTweenType easeType)
    {
        float originalTime = Time.timeScale;
        LeanTween.value(originalTime, to, time).setEase(easeType)
            .setOnUpdate((float value) => {
                Time.timeScale = value;
                Time.fixedDeltaTime = 0.02f * value;
                mainMixer.SetFloat(PITCH_PARAM, value);
            })
            .setLoopClamp()
            .setLoopPingPong(1);
    }
}
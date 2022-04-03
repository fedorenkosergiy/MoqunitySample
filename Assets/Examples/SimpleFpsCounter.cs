using UnityEngine;
using Time = Moqunity.Abstract.UnityEngine.Time;

public class SimpleFpsCounter : MonoBehaviour
{
    private const int LastFramesCount = 10;

    private Time Time = Moqunity.Context.Factory.Time;

    private float[] lastFrames = new float[LastFramesCount];
    private float lastSum = 0.0f;

    public float FpsAvg10 { get; private set; }

    private void Update()
    {
        int count = Time.frameCount;
        int slot = count % LastFramesCount;
        lastSum -= lastFrames[slot];
        float delta = Time.unscaledDeltaTime;
        lastSum += delta;
        lastFrames[slot] = delta;

        if (count >= LastFramesCount)
        {
            FpsAvg10 = 1.0f / lastSum * LastFramesCount;
        }
        else
        {
            FpsAvg10 = 1.0f / lastSum * (slot + 1);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(((int)FpsAvg10).ToString());
    }
}

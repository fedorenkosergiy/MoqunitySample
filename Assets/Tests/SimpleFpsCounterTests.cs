using System.Collections;
using UnityEngine.TestTools;
using Moqunity;
using UnityEngine;
using Moqunity.Fake.UnityEngine;
using NUnit.Framework;
using Time = Moqunity.Abstract.UnityEngine.Time;

public class SimpleFpsCounterTests
{
    private GameObject gameObject;

    private class CheckFpsTime : FakeTime
    {
        private int frameIndex;
        public float[] OverridenUnscaledDeltaTime;

        public override float unscaledDeltaTime => OverridenUnscaledDeltaTime[frameIndex];

        public override int frameCount => frameIndex;

        public void NextFrame() => frameIndex++;
    }

    private class FactoryWithOverridenTime : TestingFactory
    {
        private Time overridenTime;

        public FactoryWithOverridenTime(Time time)
        {
            overridenTime = time;
        }

        protected override Time CreateTime() => overridenTime;
    }

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameObject = new GameObject();
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckFpsAfter1Frame()
    {
        var time = new CheckFpsTime();
        time.OverridenUnscaledDeltaTime = new float [] { 0.25f };
        var factory = new FactoryWithOverridenTime(time);
        factory.Init();
        using var context = new TestingContext(factory);
        
        var counter = gameObject.AddComponent<SimpleFpsCounter>();
        yield return null;
        Assert.AreEqual(4.0f, counter.FpsAvg10);
        Object.Destroy(counter);
    }

    [UnityTest]
    public IEnumerator CheckFpsAfterSpike()
    {
        var time = new CheckFpsTime();
        const float ok = 0.032f;
        const float spike = 0.372f;
        time.OverridenUnscaledDeltaTime = new float[] { ok, ok, ok, ok, spike };
        var factory = new FactoryWithOverridenTime(time);
        factory.Init();
        using var context = new TestingContext(factory);

        var counter = gameObject.AddComponent<SimpleFpsCounter>();
        for (int i = 0; i < time.OverridenUnscaledDeltaTime.Length; ++i)
        {
            yield return null;
            time.NextFrame();
        }
        Assert.AreEqual(10.0f, counter.FpsAvg10);
        Object.Destroy(counter);
    }

    [UnityTest]
    public IEnumerator CheckFpsAfterMoreThan15Frames()
    {
        var time = new CheckFpsTime();
        const float ok = 0.032f;
        const float heavy = 0.06f;
        const float spike = 0.372f;
        time.OverridenUnscaledDeltaTime = new float[]
        {
            ok, ok, ok, ok, ok,
            spike, ok, ok, ok, ok,
            heavy, heavy, heavy, heavy, heavy
        };
        var factory = new FactoryWithOverridenTime(time);
        factory.Init();
        using var context = new TestingContext(factory);

        var counter = gameObject.AddComponent<SimpleFpsCounter>();
        for (int i = 0; i < time.OverridenUnscaledDeltaTime.Length; ++i)
        {
            yield return null;
            time.NextFrame();
        }
        Assert.AreEqual(12.5f, counter.FpsAvg10);
        Object.Destroy(counter);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(gameObject);
        yield return null;
    }
}

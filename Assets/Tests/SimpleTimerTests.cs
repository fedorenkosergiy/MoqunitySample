using System.Collections;
using UnityEngine.TestTools;
using Moqunity;
using UnityEngine;
using Moqunity.Fake.UnityEngine;
using NUnit.Framework;
using Time = Moqunity.Abstract.UnityEngine.Time;

public class SimpleTimerTests
{
    private GameObject gameObject;

    private class MyTyme : FakeTime
    {
        public float overriddenRealtime;

        public override float realtimeSinceStartup => overriddenRealtime;

        public void SetRealtimeSinceStartup(float value) => overriddenRealtime = value;
    }

    private class MyFactory : TestingFactory
    {
        private readonly Time overriddenTime;

        public MyFactory(Time time) => overriddenTime = time;

        protected override Time CreateTime() => overriddenTime;
    }

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameObject = new GameObject();
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckCallbackWasFiredInTime()
    {
        var time = new MyTyme();
        var factory = new MyFactory(time);
        factory.Init();
        using var context = new TestingContext(factory);
        {
            bool wasFired = false;
            var timer = gameObject.AddComponent<SimpleTimer>();
            yield return null;
            timer.Set(10.0f, () => wasFired = true);
            yield return null;
            time.SetRealtimeSinceStartup(10.0f);
            yield return null;
            Assert.IsTrue(wasFired);
            Object.Destroy(timer);
        }
    }

    [UnityTest]
    public IEnumerator CheckCallbackWasNotFiredBeforeTime()
    {
        var time = new MyTyme();
        var factory = new MyFactory(time);
        factory.Init();
        using var context = new TestingContext(factory);
        {
            bool wasFired = false;
            var timer = gameObject.AddComponent<SimpleTimer>();
            yield return null;
            timer.Set(10.0f, () => wasFired = true);
            yield return null;
            time.SetRealtimeSinceStartup(9.99f);
            yield return null;
            Assert.IsFalse(wasFired);
            Object.Destroy(timer);
        }
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(gameObject);
        yield return null;
    }
}

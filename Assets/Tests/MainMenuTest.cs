using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Tests
{
    public class MainMenuTest
    {
        [UnityTest]
        public IEnumerator DummyTest()
        {
            yield return null;
            Assert.IsTrue(true);
        }
    }
}
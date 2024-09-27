using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Tests
{
    public class MainWindowTests
    {
        private void InitScene()
        {
        }

        [UnityTest]
        public IEnumerator DummyTest()
        {
            yield return null;
            Assert.IsTrue(true);
        }
    }
}
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UnityEditor;
using UnityEngine;
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
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebGate.Azure.TableUtils;
using Newtonsoft.Json;

namespace WebGate.Azure.TableUtils.Test;
[TestClass]
public class SerializerIEnumberableTest
{
    [TestMethod]
    public void TestListObjects()
    {
        PocoWihtListChildren pwlc = PocoWihtListChildren.CreateInitializdedPWLC();
        IDictionary<string, object> allEntities = ObjectSerializer.Serialize(pwlc);
        Assert.AreEqual(2, allEntities.Count);
    }
}

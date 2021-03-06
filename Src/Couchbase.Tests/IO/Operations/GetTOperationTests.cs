﻿using System;
using Couchbase.Core.Transcoders;
using Couchbase.IO.Converters;
using Couchbase.IO.Operations;
using Couchbase.Utils;
using NUnit.Framework;

namespace Couchbase.Tests.IO.Operations
{
    [TestFixture]
    public class GetTOperationTests : OperationTestBase
    {
        [Test]
        public void When_Key_Exists_GetT_Returns_Value()
        {
            var key = "When_Key_Exists_GetT_Returns_Value";

            //delete the value if it exists
            var delete = new Delete(key, GetVBucket(), Transcoder, OperationLifespanTimeout);
            IOService.Execute(delete);

            //Add the key
            var add = new Add<dynamic>(key, new { foo = "foo" }, GetVBucket(), Transcoder, OperationLifespanTimeout);
            Assert.IsTrue(IOService.Execute(add).Success);

            var get = new GetT<dynamic>(key, GetVBucket(), Transcoder, OperationLifespanTimeout)
            {
                Expires = new TimeSpan(0,0,0, 1).ToTtl()
            };

            var result = IOService.Execute(get);
            Assert.IsTrue(result.Success);

            var expected = new { foo = "foo" };
            Assert.AreEqual(result.Value.foo.Value, expected.foo);
        }
    }
}

﻿using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Couchbase.Authentication.SASL;
using Couchbase.IO.Operations;
using Couchbase.Logging;

namespace Couchbase.IO.Services
{
    public class SharedPooledIOService : PooledIOService
    {
        private static readonly ILog Log = LogManager.GetLogger<SharedPooledIOService>();

        public SharedPooledIOService(IConnectionPool connectionPool)
            : base(connectionPool)
        {
        }

        public SharedPooledIOService(IConnectionPool connectionPool, ISaslMechanism saslMechanism)
            : base(connectionPool, saslMechanism)
        {
        }

        public override async Task ExecuteAsync(IOperation operation)
        {
            ExceptionDispatchInfo capturedException = null;
            IConnection connection = null;
            try
            {
                connection = _connectionPool.Acquire();

                Log.Trace("Using conn {0} on {1}", connection.Identity, connection.EndPoint);

                //A new connection will have to check for server features
                CheckEnabledServerFeatures(connection);

                await ExecuteAsync(operation, connection);
            }
            catch (Exception e)
            {
                Log.Debug(e);
                capturedException = ExceptionDispatchInfo.Capture(e);
            }
            finally
            {
                _connectionPool.Release(connection);
            }

            if (capturedException != null)
            {
                await HandleException(capturedException, operation);
            }
        }

        public override async Task ExecuteAsync<T>(IOperation<T> operation)
        {
            ExceptionDispatchInfo capturedException = null;
            IConnection connection = null;
            try
            {
                connection = _connectionPool.Acquire();

                Log.Trace("Using conn {0} on {1}", connection.Identity, connection.EndPoint);

                //A new connection will have to check for server features
                CheckEnabledServerFeatures(connection);

                await ExecuteAsync(operation, connection);
            }
            catch (Exception e)
            {
                Log.Debug(e);
                capturedException = ExceptionDispatchInfo.Capture(e);
            }
            finally
            {
                _connectionPool.Release(connection);
            }

            if (capturedException != null)
            {
                await HandleException(capturedException, operation);
            }
        }
    }
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2015 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion

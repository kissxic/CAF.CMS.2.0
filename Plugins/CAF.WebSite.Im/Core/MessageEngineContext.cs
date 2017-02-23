using CAF.Infrastructure.Core;
using CAF.Message.Distributed.Extensions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace CAF.WebSite.Im.Core
{
    public class MessageEngineContext
    {

        private static IBusControl _bus;

        /// <summary>Initializes a static instance of the CAF factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static MessageEngineContext Initialize(bool forceRecreate)
        {
            if (Singleton<MessageEngineContext>.Instance == null || forceRecreate)
            {
                Singleton<MessageEngineContext>.Instance = CreateEngineInstance();
                Singleton<MessageEngineContext>.Instance.Start();
            }
            return Singleton<MessageEngineContext>.Instance;
        }

        public IBusControl Bus
        {
            get { return _bus; }
        }
        /// <summary>Gets the singleton CAF engine used to access CAF services.</summary>
        public static MessageEngineContext Current
        {
            get
            {
                if (Singleton<MessageEngineContext>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<MessageEngineContext>.Instance;
            }
        }

        /// <summary>
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <returns>A new factory</returns>
        public static MessageEngineContext CreateEngineInstance()
        {
            _bus = BusCreator.CreateBus();
            return new MessageEngineContext();
        }
        public void Start()
        {
            _bus.Start();
        }
        public void Stop()
        {
            _bus.Stop();
        }
    }
}
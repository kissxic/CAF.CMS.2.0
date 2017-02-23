
using IdGen;
using System;
using System.Runtime.CompilerServices;


namespace CAF.Infrastructure.Core.Utilities
{
    public class IdGenEngineContext
    {

        private static IdGenerator _idGenerator;

        /// <summary>Initializes a static instance of the CAF factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IdGenEngineContext Initialize(bool forceRecreate)
        {
            if (Singleton<IdGenEngineContext>.Instance == null || forceRecreate)
            {
                Singleton<IdGenEngineContext>.Instance = CreateEngineInstance();

            }
            return Singleton<IdGenEngineContext>.Instance;
        }

        public IdGenerator IdGenerator
        {
            get { return _idGenerator; }
        }
        /// <summary>Gets the singleton CAF engine used to access CAF services.</summary>
        public static IdGenEngineContext Current
        {
            get
            {
                if (Singleton<IdGenEngineContext>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IdGenEngineContext>.Instance;
            }
        }

        /// <summary>
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <returns>A new factory</returns>
        public static IdGenEngineContext CreateEngineInstance()
        {
            // Let's say we take april 1st 2015 as our epoch
            var epoch = DateTime.UtcNow;
            // Create a mask configuration of 45 bits for timestamp, 2 for generator-id 
            // and 16 for sequence
            var mc = new MaskConfig(45, 2, 16);
            // Create an IdGenerator with it's generator-id set to 0, our custom epoch 
            // and mask configuration
            _idGenerator = new IdGenerator(0, epoch, mc);

            return new IdGenEngineContext();
        }

    }
}
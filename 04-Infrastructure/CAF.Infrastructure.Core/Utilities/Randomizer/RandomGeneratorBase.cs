using System;

namespace CAF.Infrastructure.Core.Utilities.Randomizer
{
    public abstract class RandomGeneratorBase
    {
        protected Random randomizer;

        protected virtual bool IsConditionToReachLimit()
        {
            return DateTime.Now.Ticks % 2016 == 0;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.Infrastructure.Core.Utilities.Randomizer.Interfaces.ValueTypes;
namespace CAF.Infrastructure.Core.Utilities.Randomizer.Types
{
    public class RandomIntegerGenerator : RandomGeneratorBase, IRandomInteger
    {
        public void InitSeed(int seed)
        {
            this.randomizer = new Random(seed);
        }

        public int GenerateValue()
        {
            if (IsConditionToReachLimit())
            {
                return int.MaxValue;
            }

            return randomizer.Next();
        }

        public int GenerateValue(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min cannot be greater than max.");
            }

            if (IsConditionToReachLimit())
            {
                return max;
            }

            return randomizer.Next(min, max);
        }

        public int GeneratePositiveValue()
        {
            if (IsConditionToReachLimit())
            {
                return int.MaxValue;
            }

            return randomizer.Next(0, int.MaxValue);
        }

        public int GenerateNegativeValue()
        {
            if (IsConditionToReachLimit())
            {
                return 0;
            }

            return randomizer.Next(int.MinValue, 0);
        }
    }
}


using System;
using System.Linq;
using NUnit.Framework;
using CAF.Infrastructure.Core.Utilities.Randomizer;

namespace CAF.WebSite.Test
{
    [TestFixture]
    public class RandomTest : PersistenceTest
    {

        [Test]
        public void GetRandowString()
        {
            string randowString = SomeRandom.String(20);
            Console.Write(randowString);
            string randomStringUpper = SomeRandom.StringUpper(20);
            Console.Write(randomStringUpper);
            string randomStringLower = SomeRandom.StringLower(20);
            Console.Write(randomStringLower);
            int randomInteger = SomeRandom.Integer();
            Console.Write(randomInteger);
            int randomIntegerRange = SomeRandom.Integer(1000, 5000);
            Console.Write(randomIntegerRange);
            int positiveInteger = SomeRandom.PositiveInteger();
            Console.Write(positiveInteger);
            int negativeInteger = SomeRandom.NegativeInteger();
            Console.Write(negativeInteger);

            randowString.ShouldNotBeNull();
            randomStringUpper.ShouldNotBeNull();
            randomStringLower.ShouldNotBeNull();
            randomInteger.ShouldNotBeNull();
            randomIntegerRange.ShouldNotBeNull();
            positiveInteger.ShouldNotBeNull();
            negativeInteger.ShouldNotBeNull();
        }
        [Test]
        public void GetRandowRangeInteger()
        {
            for (int i = 0; i < 20; i++)
            {
                int randomIntegerRange = SomeRandom.Integer(1000, 5000);
                Console.Write(randomIntegerRange);

            }

        }
        [Test]
        public void GetRandowRangeString()
        {
            for (int i = 0; i < 20; i++)
            {
                string randomIntegerRange = SomeRandom.String(5);
                Console.Write(randomIntegerRange);

            }

        }
    }
}
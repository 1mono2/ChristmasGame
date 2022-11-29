using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace MoNo.Christmas.Test
{
    public class TestOrganizeQuantity
    {

        [SetUp]
        public void SetUp()
        {
            Debug.Log("SetUp");
            
        }

        // A Test behaves as an ordinary method
        [TestCase(-3, 11)]
        [TestCase(-4, 7)]
        [TestCase(1, -1)]
        public void CalculationTest(int expected, int input)
        {
            OrganizeQuantity _organize = new OrganizeQuantity();
            _organize.SetNum(input);
            _organize.operations = OrganizeQuantity.ArithmeticOperations.division;
            var reValue = _organize.Event(79);
            Debug.Log(reValue);
            Assert.AreEqual(expected , reValue );
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CPMBase.Tests
{
	public class NewNUnit
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			Console.WriteLine("Test1");
			TestContext.WriteLine("Test1");
		}
	}
}
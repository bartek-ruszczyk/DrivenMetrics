using System.Linq;
using NUnit.Framework;
using Mono.Cecil;
using Driven.Metrics;
namespace Driven.Metrics.Tests
{
    [TestFixture]
    public class AssemblySearcherTests
    {
        private AssemblyDefinition _assembly;

        [SetUp]
        public void Setup()
        {
            var assemblyLoader = new AssemblyLoader("DomainTestClasses.dll");
            _assembly = assemblyLoader.Load();
        }
        
        [Test]
        public void ShouldLoadValidMethod()
        {
            var methodFinder = new AssemblySearcher(_assembly);
            var method = methodFinder.FindMethod("First");

            Assert.That(method.Name,Is.EqualTo("First"));
        }

        [Test]
        public void ShouldReturnNullForInvalidMethod()
        {
            var methodFinder = new AssemblySearcher(_assembly);
            var method = methodFinder.FindMethod("NonExisting");

            Assert.That(method, Is.Null);
        }

        [Test]
		[Ignore("Not working on Mono and Mac")]
        public void ShouldGetAllTypes()
        {
            var methodFinder = new AssemblySearcher(_assembly);
            var types = methodFinder.GetAllTypes().ToList();

            CollectionAssert.AreEquivalent(new[] { "Foo", "BaseClass" }, types.Select((typeDef, index) => typeDef.Name));
        }

    }
}

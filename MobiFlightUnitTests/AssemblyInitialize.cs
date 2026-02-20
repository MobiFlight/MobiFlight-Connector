using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;

namespace MobiFlight.Tests
{
    [TestClass]
    public class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // Globally disable schema validation to not exceed 1,000 limit per hour
            // https://www.newtonsoft.com/jsonschema
            JsonBackedObject.SkipSchemaValidation = true;
        }
    }
}
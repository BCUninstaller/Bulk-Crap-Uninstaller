using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using UninstallTools.Factory.Json;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class DynamicStringArrayConverterTests
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            Converters = { new DynamicStringArrayConverter() }
        };

        private static string[] Deserialize(string json)
        {
            return JsonSerializer.Deserialize<string[]>(json, Options);
        }

        // --- 0-dimension (bare string) ---

        [TestMethod]
        public void BareString_ReturnsSingleElementArray()
        {
            var result = Deserialize("\"foo\"");
            Assert.AreSequenceEqual(new[] { "foo" }, result);
        }

        // --- Flat arrays ---

        [TestMethod]
        public void FlatArray_ReturnsAllStrings()
        {
            var result = Deserialize("[\"a\", \"b\", \"c\"]");
            Assert.AreSequenceEqual(new[] { "a", "b", "c" }, result);
        }

        [TestMethod]
        public void EmptyArray_ReturnsEmpty()
        {
            var result = Deserialize("[]");
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void SingleElementArray_ReturnsSingleElement()
        {
            var result = Deserialize("[\"a\"]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        // --- One-level nested arrays ---

        [TestMethod]
        public void NestedArray_TakesFirstStringOnly()
        {
            var result = Deserialize("[[\"a\", \"b\"], \"c\"]");
            Assert.AreSequenceEqual(new[] { "a", "c" }, result);
        }

        [TestMethod]
        public void MultipleNestedArrays_TakesFirstFromEach()
        {
            var result = Deserialize("[[\"a\", \"b\"], [\"c\", \"d\"]]");
            Assert.AreSequenceEqual(new[] { "a", "c" }, result);
        }

        [TestMethod]
        public void EmptyNestedArray_IsSkipped()
        {
            var result = Deserialize("[[], \"c\"]");
            Assert.AreSequenceEqual(new[] { "c" }, result);
        }

        [TestMethod]
        public void AllEmptyNestedArrays_ReturnsEmpty()
        {
            var result = Deserialize("[[], []]");
            Assert.IsEmpty(result);
        }

        // --- Non-string tokens ---

        [TestMethod]
        public void NonStringFirstInNested_SkipsToFirstString()
        {
            var result = Deserialize("[[42, \"a\"], \"c\"]");
            Assert.AreSequenceEqual(new[] { "a", "c" }, result);
        }

        [TestMethod]
        public void NestedWithOnlyNonStrings_IsSkipped()
        {
            var result = Deserialize("[[42, true, null]]");
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void NullAtTopLevel_IsSkipped()
        {
            var result = Deserialize("[null, \"a\"]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        [TestMethod]
        public void NullInNestedArray_SkipsToFirstString()
        {
            var result = Deserialize("[[null, \"a\"]]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        // --- Deep nesting ---

        [TestMethod]
        public void TwoDeepNesting_RecursivelyUnwraps()
        {
            var result = Deserialize("[[[\"a\"]]]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        [TestMethod]
        public void ThreeDeepNesting_RecursivelyUnwraps()
        {
            var result = Deserialize("[[[[\"a\"]]]]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        [TestMethod]
        public void DeeplyNestedEmpty_ReturnsEmpty()
        {
            var result = Deserialize("[[[[]]]]");
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void EmptyThenNonEmptyInNested_FindsString()
        {
            var result = Deserialize("[[[], [\"x\"]], \"y\"]");
            Assert.AreSequenceEqual(new[] { "x", "y" }, result);
        }

        // --- Mixed flat and nested ---

        [TestMethod]
        public void MixedFlatAndNested_CollectsAll()
        {
            var result = Deserialize("[\"a\", [\"b\"], [[\"c\"]], \"d\"]");
            Assert.AreSequenceEqual(new[] { "a", "b", "c", "d" }, result);
        }

        // --- Objects ---

        [TestMethod]
        public void ObjectAtTopLevel_IsSkipped()
        {
            var result = Deserialize("[{\"k\":\"v\"}, \"a\"]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        [TestMethod]
        public void ObjectInNestedArray_IsSkipped()
        {
            var result = Deserialize("[[{\"k\":\"v\"}, \"a\"]]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        // --- Invalid root token ---

        [TestMethod]
        public void InvalidRootToken_ThrowsJsonException()
        {
            Assert.Throws<JsonException>(() => Deserialize("42"));
        }

        [TestMethod]
        public void RootObject_ThrowsJsonException()
        {
            Assert.Throws<JsonException>(() => Deserialize("{\"x\":1}"));
        }

        [TestMethod]
        public void RootNull_ReturnsNull()
        {
            var result = Deserialize("null");
            Assert.IsNull(result);
        }

        // --- Additional adversarial edge cases ---

        [TestMethod]
        public void EmptyStringPreserved_TopLevel()
        {
            var result = Deserialize("[\"\"]");
            Assert.AreSequenceEqual(new[] { "" }, result);
        }

        [TestMethod]
        public void EmptyStringPreserved_InNested()
        {
            var result = Deserialize("[[\"\", \"b\"]]");
            Assert.AreSequenceEqual(new[] { "" }, result);
        }

        [TestMethod]
        public void BooleanAtTopLevel_IsSkipped()
        {
            var result = Deserialize("[true, false, \"a\"]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        [TestMethod]
        public void NumberAtTopLevel_IsSkipped()
        {
            var result = Deserialize("[42, \"a\"]");
            Assert.AreSequenceEqual(new[] { "a" }, result);
        }

        [TestMethod]
        public void ScoopRealisticBinFormat()
        {
            // Scoop manifests: ["app.exe", ["tool.exe", "alias", "--args"]]
            var result = Deserialize("[\"app.exe\", [\"tool.exe\", \"alias\", \"--args\"]]");
            Assert.AreSequenceEqual(new[] { "app.exe", "tool.exe" }, result);
        }

        [TestMethod]
        public void NestedObjectFollowedByString()
        {
            var result = Deserialize("[[{\"x\":1}, \"after\"]]");
            Assert.AreSequenceEqual(new[] { "after" }, result);
        }

        [TestMethod]
        public void RecursiveNullThenNestedString()
        {
            // ReadFirstString must continue past null recursive result
            var result = Deserialize("[[[],  [\"x\"]], \"y\"]");
            Assert.AreSequenceEqual(new[] { "x", "y" }, result);
        }

        [TestMethod]
        public void AdjacentNestedWithEmptyBetween()
        {
            var result = Deserialize("[[\"a\"], [], [\"b\"]]");
            Assert.AreSequenceEqual(new[] { "a", "b" }, result);
        }

        [TestMethod]
        public void NestedWithNonStringsThenDeeperString()
        {
            // ReadFirstString must skip 42, skip empty inner, then find "x" in deeper array
            var result = Deserialize("[[42, [], [\"x\"]], \"y\"]");
            Assert.AreSequenceEqual(new[] { "x", "y" }, result);
        }
    }
}

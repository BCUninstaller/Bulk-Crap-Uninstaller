using System.Collections.Generic;
using BrightIdeasSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class TreeListViewTests
    {
        [TestMethod]
        public void AddObject_UpdatesVirtualListSizeAndAllowsExpansion()
        {
            var child = new TestNode("child");
            var root = new TestNode("root", child);

            using var tree = new TreeListView
            {
                CanExpandGetter = model => ((TestNode)model).Children.Count > 0,
                ChildrenGetter = model => ((TestNode)model).Children
            };

            tree.AddObject(root);

            Assert.AreEqual(1, tree.GetItemCount());

            tree.Expand(root);

            Assert.AreEqual(2, tree.GetItemCount());
        }

        private sealed class TestNode
        {
            public TestNode(string name, params TestNode[] children)
            {
                Name = name;
                Children = children;
            }

            public string Name { get; }
            public IReadOnlyList<TestNode> Children { get; }
        }
    }
}

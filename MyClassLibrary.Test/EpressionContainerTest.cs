using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyClassLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyClassLibrary.Test
{
    [TestClass]
    public class EpressionContainerTest
    {
        [TestMethod]
        public void TestRecursivePropertyAccess()
        {
            Tree tree = MakeTree();

            dynamic treeWrapper = ExpressionContainer.Wrap(tree);

            dynamic nodeToTest = treeWrapper.Root.Left.Left.Left;

            MemberExpression expectedExpression =
                Expression.MakeMemberAccess(
                    Expression.MakeMemberAccess(
                        Expression.MakeMemberAccess(
                            Expression.MakeMemberAccess(Expression.Constant(tree), tree.GetType().GetProperty("Root")),
                            typeof(INode).GetProperty("Left")),
                        typeof(INode).GetProperty("Left")),
                    typeof(INode).GetProperty("Left"));

            Expression actualExpression = (nodeToTest as IExpressionContainer).Expression;

            Assert.IsFalse((nodeToTest as IExpressionContainer).IsStatic);
            Assert.IsTrue(
                Equals(actualExpression, expectedExpression),
                "Comparing expressions, expected: '"
                    + expectedExpression.ToString()
                    + "', actual: '" + actualExpression.ToString() + "'");
        }

        private static Tree MakeTree()
        {
            var treeNodes = new List<INode>();
            for (int i = 0; i < 10; i++)
            {
                treeNodes.Add(new Node("node" + i, (i > 0) ? treeNodes[i - 1] : null, null));
            }

            var tree = new Tree(treeNodes[9]);

            return tree;
        }

        private static bool Equals(Expression x, Expression y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x.GetType() != y.GetType() || x.Type != y.Type || x.NodeType != y.NodeType)
            {
                return false;
            }

            MemberExpression memberExpressionX = x as MemberExpression;
            MemberExpression memberExpressionY = y as MemberExpression;

            if (memberExpressionX != null && memberExpressionY != null)
            {
                bool areMembersEqual = memberExpressionX.Member == memberExpressionY.Member;
                bool areExpressionEquals = Equals(memberExpressionX.Expression, memberExpressionY.Expression);

                return areMembersEqual && areExpressionEquals;
            }

            ConstantExpression constantExpressionX = x as ConstantExpression;
            ConstantExpression constantExpressionY = y as ConstantExpression;

            if (constantExpressionX != null && constantExpressionY != null)
            {
                bool isExpressionValueEqual = constantExpressionX.Value.Equals(constantExpressionY.Value);
                return isExpressionValueEqual;
            }

            return false;
        }
    }

    public interface INode
    {
        string Name { get; }
        INode Left { get; }
        INode Right { get; }
    }

    public class Node : INode
    {
        public string Name { get; private set; }
        public INode Left { get; private set; }
        public INode Right { get; private set; }

        public Node(string name, INode left, INode right)
        {
            Name = name;
            Left = left;
            Right = right;
        }
    }

    class Tree
    {
        public INode Root { get; private set; }

        public Tree(INode root)
        {
            Root = root;
        }
    }
}

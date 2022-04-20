using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using U4DosRandomizer.Algorithms;

namespace U4DosRandomizer.UnitTests
{
    public class TestTile : ITile
    {
        public byte X { get; set; }

        public byte Y => throw new System.NotImplementedException();

        public byte GetTile()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITile> NeighborAndAdjacentCoordinates()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITile> NeighborCoordinates()
        {
            throw new System.NotImplementedException();
        }

        public void SetTile(byte tile)
        {
            throw new System.NotImplementedException();
        }
    }
    [TestClass]
    public class BreadthFirstTraversalTests
    {

        [TestMethod]
        public void  TraversesAll()
        {
            // Arrange
            var visited = new List<bool>() { false, false, false, false, false, false, false, false };

            // Act
            BreadthFirstTraversal.BreadthFirst(new TestTile() { X = 0 },
                ( t => visited[t.X] = true ),
                ( t => (t.X == 7 ? new List<ITile>() : new List<ITile>() { new TestTile() { X = (byte)(t.X + 1) } } )));

            // Assert
            Assert.IsTrue(visited.TrueForAll(b => b), "All entries should be true if visited.");
        }

        [TestMethod]
        public void TraversesOnce()
        {
            // Arrange
            var visited = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };

            // Act
            BreadthFirstTraversal.BreadthFirst(new TestTile() { X = 0 },
                (t => { visited[t.X] = visited[t.X] + 1; return true; }),
                (t => (t.X == 7 ? new List<ITile>() : new List<ITile>() { new TestTile() { X = (byte)(t.X + 1) } }))) ;

            // Assert
            Assert.IsTrue(visited.TrueForAll(b => b == 1), "All entries should be visited once.");
        }

        [TestMethod]
        public void TraversesOnceOnReachable()
        {
            // Arrange
            var visited = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };

            // Act
            BreadthFirstTraversal.BreadthFirst(new TestTile() { X = 0 },
                (t => { visited[t.X] = visited[t.X] + 1; return true; }),
                (t => (t.X == 7 || t.X == 5 ? new List<ITile>() : new List<ITile>() { new TestTile() { X = (byte)(t.X + 1) } })));

            // Assert
            for( int i = 0; i < 6; i++ )
            {
                Assert.IsTrue(visited[i] == 1);
            }
            for (int i = 6; i < 8; i++)
            {
                Assert.IsTrue(visited[i] == 0);
            }
        }

    }
}

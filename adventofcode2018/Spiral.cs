using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace adventofcode2018
{
    [TestClass]
    public class SpiralTest
    {
        [TestMethod]
        public void Walk()
        {
            var result = new Spiral().NextValueOf(368078);
            Assert.AreEqual(369601, result);
        }
    }

    class Spiral
    {
        public IEnumerable<Position> WalkSpiral()
        {
            var position = new Position(0, 0);
            yield return position;

            var directions = new[] { Direction.UP, Direction.LEFT, Direction.DOWN, Direction.RIGHT };
            for (var length = 2;; length += 2)
            {
                position = position.Move(Direction.RIGHT).Move(Direction.DOWN);
                foreach (var direction in directions)
                    for (var i = 0; i < length; i++)
                        yield return position = position.Move(direction);
            }
        }

        public int NextValueOf(int input)
        {
            var grid = new Grid();
            foreach (var position in WalkSpiral())
            {
                int value = grid.AddPosition(position);
                if (value >= input)
                    return value;
            }

            return -1;
        }
    }

    class Grid
    {
        private Dictionary<Position, int> _map = new Dictionary<Position, int>();

        public int AddPosition(Position position)
        {
            return _map[position] = Calculate(position);
        }

        private int Calculate(Position position)
        {
            if (position.Equals(Position.Origin))
                return 1;

            return position.Neighbors.Select(ValueAt).Sum();

        }

        private int ValueAt(Position position)
        {
            return _map.TryGetValue(position, out var value) ? value : 0;
        }
    }
    
    enum Direction
    {
        LEFT, RIGHT, UP, DOWN
    }

    class Position
    {
        public static Position Origin = new Position(0, 0);
        
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT:  return Move(-1, 0);
                case Direction.RIGHT: return Move(1, 0);
                case Direction.UP:    return Move(0, -1);
                case Direction.DOWN:  return Move(0, 1);
                default: throw new Exception("Invalid direction");
            }
        }

        public Position Move(int x, int y)
        {
            return new Position(X + x, Y + y);
        }

        public IEnumerable<Position> Neighbors => new List<Position>
        {
            Move(1, 0),
            Move(1,   1),
            Move(0,   1),
            Move(-1,  1),
            Move(-1,  0),
            Move(-1, -1),
            Move(0,  -1),
            Move(1,  -1)
        };

        protected bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}

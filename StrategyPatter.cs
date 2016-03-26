using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication_test_20151202
{
    class StrategyPatter
    {
    }
    interface IDrawStrategy
    {
        void draw();
    }
    class Grapher
    {
        private IDrawStrategy _drawStrategy = null;
        public Grapher()
        {
        }
        public Grapher(IDrawStrategy drawStrategy)
        {
            _drawStrategy = drawStrategy;
        }
        public void drawShape()
        {
            if (_drawStrategy != null)
            {
                _drawStrategy.draw();
            }
        }
        public void setShape(IDrawStrategy drawStrategy)
        {
            _drawStrategy = drawStrategy;
        }
    }
    class Triangle : IDrawStrategy
    {
        public void draw()
        {
            Console.WriteLine("Draw Triangle");
        }
    }
    class Circle : IDrawStrategy
    {
        public void draw()
        {
            Console.WriteLine("Draw Circle");
        }
    }
    class Square : IDrawStrategy
    {
        public void draw()
        {
            Console.WriteLine("Draw Square");
        }
    }
}

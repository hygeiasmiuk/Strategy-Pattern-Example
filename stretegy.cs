using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy_Pattern
{
    interface Car
    {

    }
    public interface ISpeedUpSet
    {
        void SpeedUp();
    }
    public class LaserAttack : IAttackMode
    {
        void IAttackMode.AttackMode()
        {
            Console.WriteLine("Laser Attack");
        }
    }
    public class MecheteAttack : IAttackMode
    {
        void IAttackMode.AttackMode()
        {
            Console.WriteLine("Mechete Attack");
        }
    }
    public interface ISlowDownSet
    {
        void SlowDown();
    }
    public class swiftlySlowDown : ISlowDownSet
    {
        void ISlowDownSet.SlowDown()
        {
            Console.WriteLine("Slow Down swiftly");
        }
    }
    public class BreakDown : ISlowDownSet
    {
        void ISlowDownSet.SlowDown()
        {
            Console.WriteLine("break down");
        }
    }
    public interface IAttackMode
    {
        void AttackMode();
    }
    public class abruplySpeedUp : ISpeedUpSet
    {
        void ISpeedUpSet.SpeedUp()
        {
            Console.WriteLine("abruply speed up");
        }
    }
    public class slightlySpeedUp : ISpeedUpSet
    {
        void ISpeedUpSet.SpeedUp()
        {
            Console.WriteLine("slightly Speed Up");
        }
    }
    public class PorscheCar : Car
    {
        private IAttackMode _attackmode;
        private ISlowDownSet _slowdownset;
        private ISpeedUpSet _speedupset;
        public PorscheCar(IAttackMode attackmode, ISlowDownSet slowdownset, ISpeedUpSet speedupset)
        {
            _attackmode = attackmode;
            _slowdownset = slowdownset;
            _speedupset = speedupset;
        }
        public void GetAttackMode()
        {
            _attackmode.AttackMode();
        }
        public void GetSppedupMode()
        {
            _speedupset.SpeedUp();

        }
        public void GetSlowDownMode()
        {
            _slowdownset.SlowDown();
        }
        public void SetSlowDownMode(ISlowDownSet slowdownset)
        {
            _slowdownset = slowdownset;
        }
        public void SetSpeedupMode(ISpeedUpSet speedup)
        {
            _speedupset = speedup;
        }
        public void SetAttackMode(IAttackMode attack)
        {
            _attackmode = attack;
        }
        public void GetCurrentState()
        {
            _slowdownset.SlowDown();
            _attackmode.AttackMode();
            _speedupset.SpeedUp();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            PorscheCar pc = new PorscheCar(new LaserAttack(), new BreakDown(), new abruplySpeedUp());
            Console.WriteLine("before");
            pc.GetCurrentState();
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("after");
            pc.SetAttackMode(new MecheteAttack());
            pc.GetCurrentState();
            Console.WriteLine(Environment.NewLine);
            pc.SetSpeedupMode(new slightlySpeedUp());
            pc.GetCurrentState();
            Console.WriteLine(Environment.NewLine);
            pc.SetSlowDownMode(new swiftlySlowDown());
            pc.GetCurrentState();
        }
    }
}

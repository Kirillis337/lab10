using System;
using System.Collections.Generic;

namespace lab10
{
    class Program
    {
        static void Main()
        {
            //Цепочка Обязанностей (Chain of responsibility)
            Receiver receiver = new Receiver(false, true, true);
            ArmyHandler wzrdHandler = new WizzardHandler();
            ArmyHandler archHadler = new ArcherHandler();
            ArmyHandler warrHandler = new WarriorHandler();
            wzrdHandler.Successor = archHadler;
            archHadler.Successor = warrHandler;
            wzrdHandler.Handle(receiver);
            Console.WriteLine("--------------------------");
            //Посредник (Mediator)
            ManagerMediator mediator = new ManagerMediator();
            Colleague warrior = new WarriorColleague(mediator);
            Colleague archer = new ArcherColleague(mediator);
            Colleague wizzard = new WizzardColleague(mediator);
            mediator.Customer = warrior;
            mediator.Programmer = archer;
            mediator.Tester = wizzard;
            warrior.Send("Нужен обстрел по вражеской пехоте");
            archer.Send("Нужен огненный дождь по врагам!");
            wizzard.Send("Заклинание готово, отступайте");
            Console.WriteLine("--------------------------");
            //Наблюдатель (Observer)
            Battle stock = new Battle();
            Archer arch = new Archer("Игнат", stock);
            Warrior warr1 = new Warrior("Леха", stock);
            // имитация боя
            stock.Fight();
            Console.WriteLine("--------------------------");
            // воин умерб(
            warr1.DiesfromCringe();
            // имитация боя
            stock.Fight();

            Console.WriteLine("--------------------------");
            //Посетитель (Visitor)
            var structure = new Restaurant();
            structure.Add(new Knight { Name = "Рыцарь Артём" });
            structure.Add(new Mage { Name = "Маг Генадий" });
            structure.Accept(new PoorAlchemist());
            structure.Accept(new GoodAlchemist());

            Console.ReadLine();
        }
    }
    //Цепочка Обязанностей (Chain of responsibility)
    class Receiver
    {
        // Маги
        public bool WizzardTransfer { get; set; }
        // Лучники
        public bool ArcherTransfer { get; set; }
        // Войны
        public bool WarriorTransfer { get; set; }
        public Receiver(bool wizt, bool archt, bool wart)
        {
            WizzardTransfer = wizt;
            ArcherTransfer = archt;
            WarriorTransfer = wart;
        }
    }
    abstract class ArmyHandler
    {
        public ArmyHandler Successor { get; set; }
        public abstract void Handle(Receiver receiver);
    }

    class WizzardHandler : ArmyHandler
    {
        public override void Handle(Receiver receiver)
        {
            if (receiver.WizzardTransfer == true)
                Console.WriteLine("Отправляем магов");
            else if (Successor != null)
                Successor.Handle(receiver);
        }
    }

    class WarriorHandler : ArmyHandler
    {
        public override void Handle(Receiver receiver)
        {
            if (receiver.WarriorTransfer == true)
                Console.WriteLine("Отправляем войнов");
            else if (Successor != null)
                Successor.Handle(receiver);
        }
    }

    class ArcherHandler : ArmyHandler
    {
        public override void Handle(Receiver receiver)
        {
            if (receiver.ArcherTransfer == true)
                Console.WriteLine("Отправляем лучников");
            else if (Successor != null)
                Successor.Handle(receiver);
        }
    }
    //Посредник (Mediator)

    abstract class Mediator
    {
        public abstract void Send(string msg, Colleague colleague);
    }
    abstract class Colleague
    {
        protected Mediator mediator;

        public Colleague(Mediator mediator)
        {
            this.mediator = mediator;
        }

        public virtual void Send(string message)
        {
            mediator.Send(message, this);
        }
        public abstract void Notify(string message);
    }
    // класс войнов
    class WarriorColleague : Colleague
    {
        public WarriorColleague(Mediator mediator)
            : base(mediator)
        { }

        public override void Notify(string message)
        {
            Console.WriteLine("Сообщение пехоте: " + message);
        }
    }
    // класс лучников
    class ArcherColleague : Colleague
    {
        public ArcherColleague(Mediator mediator)
            : base(mediator)
        { }

        public override void Notify(string message)
        {
            Console.WriteLine("Сообщение лучникам: " + message);
        }
    }
    // класс магов
    class WizzardColleague : Colleague
    {
        public WizzardColleague(Mediator mediator)
            : base(mediator)
        { }

        public override void Notify(string message)
        {
            Console.WriteLine("Сообщение магам: " + message);
        }
    }

    class ManagerMediator : Mediator
    {
        public Colleague Customer { get; set; }
        public Colleague Programmer { get; set; }
        public Colleague Tester { get; set; }
        public override void Send(string msg, Colleague colleague)
        {
            // если отправитель - воин, значит нужна поддержка лучников
            // отправляем сообщение лучникам 
            if (Customer == colleague)
                Programmer.Notify(msg);
            // если отправитель - лучник, то лучники не справляются и нужен огненный дождь! 
            // отправляем сообщение магам
            else if (Programmer == colleague)
                Tester.Notify(msg);
            // если отправитель - Маг, значит сайчас начнется резня
            // отправляем сообщение войнам
            else if (Tester == colleague)
                Customer.Notify(msg);
        }
    }
    //Наблюдатель (Observer)
    interface IObserver
    {
        void Update(Object ob);
    }

    interface IObservable
    {
        void RegisterObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void NotifyObservers();
    }

    class Battle : IObservable
    {
        BattleInfo bInfo; // информация о битве

        List<IObserver> observers;
        public Battle()
        {
            observers = new List<IObserver>();
            bInfo = new BattleInfo();
        }
        public void RegisterObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }

        public void NotifyObservers()
        {
            foreach (IObserver o in observers)
            {
                o.Update(bInfo);
            }
        }

        public void Fight()
        {
            Random rnd = new Random();
            bInfo.fireradius = rnd.Next(1, 20);
            NotifyObservers();
        }
    }

    class BattleInfo
    {
        public int fireradius { get; set; }
    }

    class Warrior : IObserver
    {
        public string Name { get; set; }
        IObservable stock;
        public Warrior(string name, IObservable obs)
        {
            this.Name = name;
            stock = obs;
            stock.RegisterObserver(this);
        }
        public void Update(object ob)
        {
            BattleInfo bInfo = (BattleInfo)ob;

            if (bInfo.fireradius > 3)

                Console.WriteLine("Воин {0} попадает под огненный дождь, радиусом {1}", this.Name, bInfo.fireradius);
            else
                Console.WriteLine("Воин {0} уворачивается от огненного дождя, так как радиус {1} слишком мал", this.Name, bInfo.fireradius);
        }
        public void DiesfromCringe()
        {
            stock.RemoveObserver(this);
            stock = null;
        }
    }

    class Archer : IObserver
    {
        public string Name { get; set; }
        IObservable stock;
        public Archer(string name, IObservable obs)
        {
            this.Name = name;
            stock = obs;
            stock.RegisterObserver(this);
        }
        public void Update(object ob)
        {
            BattleInfo bInfo = (BattleInfo)ob;

            if (bInfo.fireradius > 17)
                Console.WriteLine("Лучник {0} попадает под огненный дождь, радиусом {1}", this.Name, bInfo.fireradius);
            else
                Console.WriteLine("Лучник {0} уворачивается от огненного дождя, так как радиус {1} слишком мал", this.Name, bInfo.fireradius);
        }
    }
    //Посетитель (Visitor)

    interface Visitor
    {
        void VisitKnight(Knight vis);
        void VisitMage(Mage vis);
    }

    class GoodAlchemist : Visitor
    {
        public void VisitKnight(Knight vis)
        {
            Console.WriteLine("Куплено 2 Превосходных зелья лечения");
        }

        public void VisitMage(Mage vis)
        {
            Console.WriteLine("Куплено Превосходное зелье маны");
        }
    }

    class PoorAlchemist : Visitor
    {
        public void VisitKnight(Knight vis)
        {
            Console.WriteLine("Куплена Настойка от паноса");
        }

        public void VisitMage(Mage vis)
        {
            Console.WriteLine("Куплено Варево от головной боли");
        }
    }

    class Restaurant
    {
        List<Customer> сustomers = new List<Customer>();
        public void Add(Customer vis)
        {
            сustomers.Add(vis);
        }
        public void Remove(Customer vis)
        {
            сustomers.Remove(vis);
        }
        public void Accept(Visitor visitor)
        {
            foreach (Customer vis in сustomers)
                vis.Accept(visitor);
        }
    }

    interface Customer
    {
        void Accept(Visitor visitor);
    }

    class Knight : Customer
    {
        public string Name { get; set; }
        public string Age { get; set; }

        public void Accept(Visitor visitor)
        {
            visitor.VisitKnight(this);
        }
    }

    class Mage : Customer
    {
        public string Name { get; set; }

        public void Accept(Visitor visitor)
        {
            visitor.VisitMage(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactivUI_Test
{
    class BarClass : ReactiveObject
    {
        [Reactive] public String Baz { get; set; } = "";
        public BarClass(String b) { Baz = b; }
        public BarClass() { Baz = "!!!"; }
        public override String ToString() { return Baz.ToString(); }
    }


    
    class FooClass : ReactiveObject
    {
        [Reactive] public BarClass Bar { get; set; } = new BarClass();
        public override String ToString() { return Bar.ToString(); }
    }


    class ViewModel: ReactiveObject
    {
        [Reactive] FooClass Foo { get; set; } = new FooClass();

        void PrintList<T> (IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Console.WriteLine(item.ToString());
            }
        }

        SourceList<FooClass> sl2 = new SourceList<FooClass>();

        public ViewModel()
        {
                this.WhenAnyValue(x => x.Foo.Bar.Baz)
                       .Subscribe(x => Console.WriteLine("Hallo " + x?.ToString())); 

                  Console.WriteLine("Example 1");
                  this.Foo.Bar.Baz = null;
                  Console.WriteLine("Example 2a");
                  this.Foo.Bar = new BarClass();
                  Console.WriteLine("Example 2b");
                  this.Foo.Bar = new BarClass();
                  Console.WriteLine("Example 3");
                  this.Foo.Bar = new BarClass() { Baz = "Something" };

                  Console.WriteLine("Example 4");
                  this.Foo = new FooClass() ; 


                  SourceList<String> sl = new SourceList<String>();
                  sl.Add("One");
                  sl.Add("Two");
                  sl.Add("Two");
                  sl.Add("Three");


                  sl.Connect()
                      .Transform(x => x)
                      .Sort(SortExpressionComparer<String>.Ascending(t => t))
                      .DistinctValues(x => x)
                      .Bind(out ReadOnlyObservableCollection<String> sorted)
                      .Subscribe();

                  Console.WriteLine("=== Raw List ===");
                  PrintList<String>(sl.Items);

                  Console.WriteLine("=== Sorted ===");
                  PrintList<String>(sorted);   

            Console.WriteLine("===  ===");

            

            FooClass fo1 = new FooClass() { Bar = new BarClass("Hello ") };
            FooClass fo2 = new FooClass() { Bar = new BarClass("World ") };
            FooClass fo3 = new FooClass() { Bar = new BarClass("Out ") };
            FooClass fo4 = new FooClass() { Bar = new BarClass("There ") };
            FooClass fo5 = new FooClass() { Bar = new BarClass("!!!!!!") };

            sl2.Add(fo1);
            sl2.Add(fo2);
            sl2.Add(fo3);
            sl2.Add(fo4);


            sl2.Connect()
                .AutoRefresh(x => x.Bar.Baz)
                .Transform(x => x.Bar.Baz, true)
         //       .Sort(SortExpressionComparer<String>.Ascending(t => t))
         //       .DistinctValues(x => x)
                .Bind(out ReadOnlyObservableCollection<String> transformed)
                .Subscribe( x=> { Console.WriteLine("CHANGE from Subscribe"); } );

            Console.WriteLine("=== Start ===");

            ((INotifyCollectionChanged)transformed).CollectionChanged += 
                new NotifyCollectionChangedEventHandler(( s,e) => Console.WriteLine("CHANGE from Event Handler"));

     
            Console.WriteLine("sl2: ");
            PrintList<FooClass>(sl2.Items);

            Console.WriteLine("transformed: ");
            PrintList<String>(transformed);


            Console.WriteLine("=== Send to Space ===");
            fo2.Bar.Baz = "Space";
            
            Console.WriteLine("sl2: ");
            PrintList<FooClass>(sl2.Items);

            Console.WriteLine("transformed: ");
            PrintList<String>(transformed);      

            Console.WriteLine("=== Add !!!! ===" );
            
            sl2.Add(fo5);

            Console.WriteLine("sl2: ");
            PrintList<FooClass>(sl2.Items);

            Console.WriteLine("transformed: ");
            PrintList<String>(transformed);

            Console.WriteLine("===  ===");

            Console.WriteLine("Finish");
            Console.ReadLine();

        }
    }


    class Program 
    {
        static void Main(string[] args)
        {
            ViewModel vm = new ViewModel();
        }
    }
}

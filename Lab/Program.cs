using System;
using System.Threading;
using Serialization;
using Writing;


using Tracer;
namespace Lab
{
    class Program
    {
        static void Main(string[] args)
        {   
           TracerClass tracer = new TracerClass();

            FirstClass _first = new FirstClass(tracer);
            SecondClass _second = new SecondClass(tracer);
            ThirdClass _third = new ThirdClass(tracer);

            tracer.StartTrace();
            _third.RecursionMethod();
            _second.InnerMethod();
            tracer.StopTrace();

            Thread secondThread = new Thread(new ThreadStart(_first.MyMethod));
            secondThread.Start();

            Thread thirdThread = new Thread(new ThreadStart(_second.InnerMethod));
            thirdThread.Start();

            secondThread.Join(); //ожидание выполнения секнд тред
            thirdThread.Join();

            TraceResult traceResult = tracer.GetTraceResult();

            ISerializer serializerJson = new JsonSerializer();
            ISerializer serializerXml = new myXmlSerializer();
            IWriter consoleWriter = new ConsoleWriter();
            IWriter fileWriter = new FileWriter(Environment.CurrentDirectory + "\\" + "output.txt");

            string json = serializerJson.Serialize(traceResult);
            string xml = serializerXml.Serialize(traceResult);

            consoleWriter.Write(json);
            consoleWriter.Write("\n\n--------------\n\n");
            consoleWriter.Write(xml);

            fileWriter.Write(json + "\n\n--------------\n\n" + xml);

         /*   string controller = "";
            Console.WriteLine("\nFile output: \n1 - json\n2 - xml\n3 - both");
            while (true)
            {
                controller = Console.ReadLine();
                switch (controller)
                {
                    case "1":
                        fileWriter.Write(json);
                        return;
                    case "2":
                        fileWriter.Write(xml);
                        return;
                    case "3": 
                        fileWriter.Write(json + "\n\n--------------\n\n" + xml);
                        return;
                    default: continue;
                }
            }*/
        }
    }

    public class FirstClass
    {
        private SecondClass _second;
        private ITracer _tracer;

        internal FirstClass(ITracer tracer)
        {
            _tracer = tracer;
            _second = new SecondClass(_tracer);
        }
        public void MyMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _second.InnerMethod();
            _tracer.StopTrace();
        }
    }


    public class SecondClass
    {
        private ITracer _tracer;

        internal SecondClass(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void InnerMethod()
        {
         
            _tracer.StartTrace();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
    }

    public class ThirdClass
    {
        private ITracer _tracer;
        private SecondClass _second;
        private int n = 3;

        internal ThirdClass(ITracer tracer)
        {
            _tracer = tracer;
            _second = new SecondClass(_tracer);
        }

        public void RecursionMethod()
        {
            Console.WriteLine(1);
            _tracer.StartTrace();
            while (n != 0)
            {
                n--;
                RecursionMethod();
                Console.WriteLine(2);
                _second.InnerMethod();
            }
            Thread.Sleep(100);
            Console.WriteLine(3);
            _tracer.StopTrace();
        }
    }
}

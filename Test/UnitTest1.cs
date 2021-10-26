using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;
using System;
using Tracer;

namespace UnitTests
{
    [TestClass]
    public class TracerClassTests
    {

        TraceResult traceResult;

        [TestInitialize]
        public void Setup()
        {
            TracerClass tracer = new TracerClass();

            FirstClass _first = new FirstClass(tracer);
            SecondClass _second = new SecondClass(tracer);
            ThirdClass _third = new ThirdClass(tracer);

            _third.RecursionMethod();

            Thread secondThread = new Thread(new ThreadStart(_first.MyMethod));
            secondThread.Start();

            Thread thirdThread = new Thread(new ThreadStart(_second.InnerMethod));
            thirdThread.Start();

            secondThread.Join();
            thirdThread.Join();

            traceResult = tracer.GetTraceResult();
        }


        [TestMethod]
        public void Test_ThreadCount_3()
        {
            Assert.AreEqual(3, traceResult.Threads.Count);
        }

        [TestMethod]
        public void Test_SameLevelMethods_2()
        {
            Assert.AreEqual(2, traceResult.Threads[0].Methods[0].Methods.Count);
        }

        [TestMethod]
        public void Test_MethodInfo_Second_InnerMethod()
        {
            Assert.AreEqual("InnerMethod", traceResult.Threads[0].Methods[0].Methods[1].Name, "Имя метода не совпадает");
            Assert.AreEqual("SecondClass", traceResult.Threads[0].Methods[0].Methods[1].ClassName, "Имя класса не совпадает");
        }

        [TestMethod]
        public void Test_ExecutionTime()
        {
            Stopwatch stopwatch = new Stopwatch();
            TracerClass tracer = new TracerClass();

            stopwatch.Start();
            tracer.StartTrace();

            Thread.Sleep(1000);

            stopwatch.Stop();
            tracer.StopTrace();

            Assert.IsTrue(Math.Abs(stopwatch.ElapsedMilliseconds - tracer.GetTraceResult().Threads[0].Time) < 10);
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
        private int n = 30;

        internal ThirdClass(ITracer tracer)
        {
            _tracer = tracer;
            _second = new SecondClass(_tracer);
        }

        public void RecursionMethod()
        {
            _tracer.StartTrace();
            while (n != 0)
            {
                n--;
                RecursionMethod();
                _second.InnerMethod();
            }
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RPNCalc
{
    //public delegate double binop(double a, double b);
    //public delegate double monop(double a);

    /// <summary>
    /// A class that wraps a system Stack&lt;double&gt; to produce a stack like object.
    /// Its main feature is that it does not throw exceptions, instead generating double.NaN when inconsistent.
    /// </summary>
    class CalcEngine
    {
        public delegate double binop(double a, double b);
        public delegate double monop(double a);

        private class SafeStack
        {
            private Stack<double> S = new Stack<double>();

            public void push(double d) { S.Push(d); }
            public double pop() { return S.Count > 0 ? S.Pop() : double.NaN; }
            public double peek() { return S.Count > 0 ? S.Peek() : double.NaN; }
            public string tostring() { return S.Dump(); }
            public void clear() { S.Clear(); } 
        }

        private SafeStack stack = new SafeStack();

        public string StackToString() { return stack.tostring(); }

        public void Reduce(monop m) { stack.push(m(stack.pop())); }
        public void Reduce(binop b) { stack.push(b(stack.pop(), stack.pop())); }
        public void Commit(double d) { stack.push(d); }
        public void Uncommit() { stack.pop(); }
        public double ToS { get { return stack.peek(); } }
        public void Reset() { stack.clear(); }
    }

    static class Extensions
    {
        public static string Dump<T>(this Stack<T> s, string format)
        {
            StringBuilder output = new StringBuilder();
            foreach (var v in s.ToArray<T>())
                output.AppendFormat(format, v);
            return output.ToString();
        }

        public static string Dump<T>(this Stack<T> s)
        {
            return Dump<T>(s,"{0}\n");
        }
    }
}

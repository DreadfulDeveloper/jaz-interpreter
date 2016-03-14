using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace jaz
{
    public class JazInterpreter
    {
        public static void main(String[] args)
        {
            if (!(args.Length > 0))
            {
                Console.WriteLine("Must provide a jaz file");
                System.Environment.Exit(1);
            }
            // start parsing program into symbol table
            Parser parser = new Parser(args[0]);
            //evaluate symbol table once all symbols are loaded
            JazEvaluator eval = new JazEvaluator();
        }
    }
    public class SymbolTable
    {
        public static List<JazTuple<string, string>> symbolTable = new List<JazTuple<string, string>>();
    }

    class JazEnv
    {
        private Dictionary<string, int> env = new Dictionary<string, int>();
        private Stack<int> callee = new Stack<int>();

        private bool inProc;
        private bool afterCall;
        private int recursive;
        private bool returning;

        public JazEnv()
        {
            inProc = false;
            afterCall = false;
            recursive = 0;
            returning = false;
        }

        public void startProc(int line)
        {
            callee.Push(line);
        }

        public int endProc()
        {
            return callee.Pop();
        }

        public void putLabel(string name, int line) {
            env.Add(name, line);
        }

        public bool isInProc() {
            return inProc;
        }

        public void setInProc(bool status) {
            inProc = status;
        }

        public bool isAfterCall()
        {
            return afterCall;
        }

        public void setAfterCall(bool b)
        {
            afterCall = b;
        }

        public void resetRecursive()
        {
            recursive--;
        }
        
        public bool stillCalling()
        {
            return this.isRecursive();
        }

        public void resetCallDepth()
        {
            this.resetRecursive();

        }

        public void incCallDepth()
        {
            this.incRecursive();
        }

        public void setReturning(bool b)
        {
            returning = b;
        }

        public bool isReturning()
        {
            return returning;
        }

        public void incRecursive() {
            recursive++;
        }

        public int getRecursive() {
            return recursive;
        }

        public bool isRecursive() {
            return recursive > 0;
        }       
    }

    public class JazEvaluator
    {
        private Stack<Object> executionStack = new Stack<Object>();
        private Dictionary<String, Stack<String>> memory = new Dictionary<String, Stack<String>>();
        private Dictionary<String, Stack<String>> procMemory = new Dictionary<String, Stack<String>>();

        public JazEvaluator()
        {
        }
    }
    
    public class Parser
    {
        static JazEnv env = new JazEnv();

        public Parser(String file)
        {
            this.parse(file);
        }

        private void parse(string file)
        {
            using (System.IO.StreamReader sr = System.IO.File.OpenText(file))
            {
                String t;
                int lineNumber = 0;
                while ((t = sr.ReadLine()) != null)
                {
                    lineNumber = parse(t, lineNumber);
                }
            }
        }

        private int parse(String t, int lineNumber) {
            t = t.Trim();
            string head = t.Split(' ')[0].Trim();
            string rest = t.Substring(t.Split(' ')[0].Length).Trim();

            if(!String.IsNullOrEmpty(head)) {
                //load tuples into symbol table
                if(head == "label")
                {
                    env.putLabel(rest, lineNumber);
                    SymbolTable.symbolTable.Add(new JazTuple<string, string>(head, rest));
                }
                else
                {
                    SymbolTable.symbolTable.Add(new JazTuple<string, string>(head, rest));
                }
                
                lineNumber++;
            }
            return lineNumber;
        }
    }


    public class JazTuple<H, R>
    {
        private H Head;
        private R Rest;

        public JazTuple(H head, R rest)
        {
            Head = head;
            Rest = rest;
        }

        public H gethead() {
            return Head;
        }
        public R getRest()
        {
            return Rest;
        }
        public void setHead(H head)
        {
            Head = head;
        }

        public void setRest(R rest)
        {
            Rest = rest;
        }

        public String toString()
        {
            return "(" + Head + Rest + ")";
        }
    }


}

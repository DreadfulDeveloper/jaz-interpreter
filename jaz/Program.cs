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
        public static void Main(String[] args)
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
            int lineNumber = 0;
            JazTuple<string, string> currentInstruction = SymbolTable.symbolTable[lineNumber];
            while(currentInstruction.getAction() != "halt")
            {
                currentInstruction = SymbolTable.symbolTable[lineNumber];
                switch (currentInstruction.getAction())
                {
                    case "push":
                        push(currentInstruction);
                        break;
                    case "rvalue":
                        rValue(currentInstruction);
                        break;
                    case "lvalue":
                        lValue(currentInstruction);
                        break;
                    case "pop":
                        pop();
                        break;
                    case ":=":
                        colonEqual(currentInstruction);
                        break;
                    case "copy":
                        copy();
                        break;
                    case "label":
                        label(currentInstruction);
                        break;
                    case "goto":
                        goTo(currentInstruction);
                        break;
                    case "gofalse":
                        goFalse(currentInstruction);
                        break;
                    case "gotrue":
                        goTrue(currentInstruction);
                        break;
                    case "+":
                        add(currentInstruction);
                        break;
                    case "-":
                        subtract(currentInstruction);
                        break;
                    case "*":
                        multiply(currentInstruction);
                        break;
                    case "/":
                        divide(currentInstruction);
                        break;
                    case "div":
                        mod(currentInstruction);
                        break;
                    case "&":
                        and(currentInstruction);
                        break;
                    case "!":
                        neg(currentInstruction);
                        break;
                    case "|":
                        or(currentInstruction);
                        break;
                    case "<>":
                        notEqual(currentInstruction);
                        break;
                    case "<=":
                        lessThanEqualTo(currentInstruction);
                        break;
                    case ">=":
                        greaterThanEualTo(currentInstruction);
                        break;
                    case "<":
                        lessThan(currentInstruction);
                        break;
                    case ">":
                        greaterThan(currentInstruction);
                        break;
                    case "=":
                        equal(currentInstruction);
                        break;
                    case "print":
                        print(currentInstruction);
                        break;
                    case "show":
                        show(currentInstruction);
                        break;
                    case "begin":
                        begin(currentInstruction);
                        break;
                    case "end":
                        end(currentInstruction);
                        break;
                    case "return":
                        ret(currentInstruction);
                        break;
                    case "call":
                        call(currentInstruction);
                        break;
                    default:
                        break;
                }
                lineNumber++;
            }
        }

        private void push(JazTuple<string, string> instruction) {
            executionStack.Push(instruction.getArgs());
        }
        private void rValue(JazTuple<string, string> instruction) { }
        private void lValue(JazTuple<string, string> instruction) { }
        private void pop() {
            executionStack.Pop();
        }
        private void colonEqual(JazTuple<string, string> instruction) { }
        private void copy() {
            executionStack.Push(executionStack.Peek());
        }
        private void label(JazTuple<string, string> instruction) { }
        private void goTo(JazTuple<string, string> instruction) { }
        private void goFalse(JazTuple<string, string> instruction) { }
        private void goTrue(JazTuple<string, string> instruction) { }
        private void halt(JazTuple<string, string> instruction) { }
        private void add(JazTuple<string, string> instruction) { }
        private void subtract(JazTuple<string, string> instruction) { }
        private void multiply(JazTuple<string, string> instruction) { }
        private void divide(JazTuple<string, string> instruction) { }
        private void mod(JazTuple<string, string> instruction) { }
        private void and(JazTuple<string, string> instruction) { }
        private void neg(JazTuple<string, string> instruction) { }
        private void or(JazTuple<string, string> instruction) { }
        private void notEqual(JazTuple<string, string> instruction) { }
        private void lessThanEqualTo(JazTuple<string, string> instruction) { }
        private void greaterThanEualTo(JazTuple<string, string> instruction) { }
        private void lessThan(JazTuple<string, string> instruction) { }
        private void greaterThan(JazTuple<string, string> instruction) { }
        private void equal(JazTuple<string, string> instruction) { }
        private void print(JazTuple<string, string> instruction) {
            Console.WriteLine(executionStack.Pop());
        }
        private void show(JazTuple<string, string> instruction)
        {
            Console.WriteLine(instruction.getArgs());
        }
        private void begin(JazTuple<string, string> instruction) { }
        private void end(JazTuple<string, string> instruction) { }
        private void ret(JazTuple<string, string> instruction) { }
        private void call(JazTuple<string, string> instruction) { }
    }
    
    public class Parser
    {
        static JazEnv env = new JazEnv();

        public Parser(String file)
        {
            this.parseFile(file);
        }

        private void parseFile(string file)
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
            string label = t.Split(' ')[0].Trim();
            string args = t.Substring(t.Split(' ')[0].Length).Trim();

            if(!String.IsNullOrEmpty(label)) {
                //load tuples into symbol table
                if(label == "label")
                {
                    env.putLabel(args, lineNumber);
                    SymbolTable.symbolTable.Add(new JazTuple<string, string>(label, args));
                }
                else
                {
                    SymbolTable.symbolTable.Add(new JazTuple<string, string>(label, args));
                }
                
                lineNumber++;
            }
            return lineNumber;
        }
    }
    
    //A tuple is an instruction "label" and the arguments for that instruction
    public class JazTuple<Action, Args>
    {
        private Action A;
        private Args R;

        public JazTuple(Action a, Args r)
        {
            A = a;
            R = r;
        }

        public Action getAction() {
            return A;
        }
        public Args getArgs()
        {
            return R;
        }

        public void setAction(Action a)
        {
            A = a;
        }

        public void setArgs(Args r)
        {
            R = r;
        }

        public String toString()
        {
            return "(" + A + " " + R + ")";
        }
    }
}

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
        public static void Main(string[] args)
        {
            if (!(args.Length > 0))
            {
                Console.WriteLine("Must provide a jaz file");
                System.Environment.Exit(1);
            }
            // start parsing program into symbol table
            Parser parser = new Parser(args[0]);
            //evaluate symbol table once all symbols are loaded
			new JazEvaluator(parser.SymbolTable, parser.Env);
        }
    }

    public class JazEnv
    {
		private Dictionary<string, int> labels;
        private Stack<int> callee;
		private Stack<Stack<string>> scopeStack;

        public JazEnv()
        {
			labels = new Dictionary<string, int>();
			callee = new Stack<int>();
			scopeStack = new Stack<Stack<string>>();
        }

        public void startProc(int line)
        {
            callee.Push(line);
        }

        public int endProc()
        {
            return callee.Pop();
        }

        public void putLabel(string name, int line) 
		{
            labels.Add(name, line);
        }

		public int getLineForLabel(string name)
		{
			return labels [name];
		}     

		public void pushStack(Stack<string> oldStack)
		{
			scopeStack.Push (oldStack);
		}

		public Stack<string> popStack()
		{
			return scopeStack.Pop ();
		}
    }

    public class JazEvaluator
    {
		// Parameters Passed
		private static List<JazTuple<string, string>> symbolTable;
		private static JazEnv env; 

		// Program Execution
		private Stack<string> executionStack = new Stack<string>();
        private Dictionary<string, string> memory = new Dictionary<string, string>();

		private int lineNumber;

		public JazEvaluator(List<JazTuple<string, string>> symbols, JazEnv enviornment)
        {
			symbolTable = symbols;
			env = enviornment;
			lineNumber = 0;

			JazTuple<string, string> currentInstruction = symbolTable[lineNumber];
			while(currentInstruction.getAction() != "halt")
            {
				currentInstruction = symbolTable[lineNumber];
				switch (currentInstruction.getAction ()) {
					case "push":
						push (currentInstruction);
						break;
					case "rvalue":
						rValue (currentInstruction);
						break;
					case "lvalue":
						lValue (currentInstruction);
						break;
					case "pop":
						pop ();
						break;
					case ":=":
						colonEqual (currentInstruction);
						break;
					case "copy":
						copy ();
						break;
					case "label":
						label (currentInstruction);
						break;
					case "goto":
						goTo (currentInstruction);
						break;
					case "gofalse":
						goFalse (currentInstruction);
						break;
					case "gotrue":
						goTrue (currentInstruction);
						break;
					case "+":
						add (currentInstruction);
						break;
					case "-":
						subtract (currentInstruction);
						break;
					case "*":
						multiply (currentInstruction);
						break;
					case "/":
						divide (currentInstruction);
						break;
					case "div":
						mod (currentInstruction);
						break;
					case "&":
						and (currentInstruction);
						break;
					case "!":
						neg (currentInstruction);
						break;
					case "|":
						or (currentInstruction);
						break;
					case "<>":
						notEqual (currentInstruction);
						break;
					case "<=":
						lessThanEqualTo (currentInstruction);
						break;
					case ">=":
						greaterThanEualTo (currentInstruction);
						break;
					case "<":
						lessThan (currentInstruction);
						break;
					case ">":
						greaterThan (currentInstruction);
						break;
					case "=":
						equal (currentInstruction);
						break;
					case "print":
						print (currentInstruction);
						break;
					case "show":
						show (currentInstruction);
						break;
					case "begin":
						begin (currentInstruction);
						break;
					case "end":
						end (currentInstruction);
						break;
					case "return":
						ret (currentInstruction);
						break;
					case "call":
						call (currentInstruction);
						break;
					default:
						break;
				}
				lineNumber++;
            }
        }

		// Value given is pushed onto the stack.
        private void push(JazTuple<string, string> instruction) 
		{
            executionStack.Push(instruction.getArgs());
        }

		// Value at Memory Location given is pushed onto the stack.
        private void rValue(JazTuple<string, string> instruction) 
		{
			executionStack.Push(memory[instruction.getArgs()]);
		}

		// Memory location given is pushed onto the stack.
        private void lValue(JazTuple<string, string> instruction) 
		{
			executionStack.Push (instruction.getArgs ());
		}

		// The top value is popped off of the stack.
        private void pop() 
		{
            executionStack.Pop();
        }

		// Pop two off the stack. Put a value of the first at index of the second in memory.
        private void colonEqual(JazTuple<string, string> instruction) 
		{
			string value = executionStack.Pop ();
			string memoryAddress = executionStack.Pop ();
			if (memory.ContainsKey (memoryAddress)) 
			{
				memory [memoryAddress] = value;
			}
			else 
			{
				memory.Add (memoryAddress, value);
			}
		}

		// Copy the top of the stack to itself.
        private void copy() 
		{
            executionStack.Push(executionStack.Peek());
        }

		// labels are ignored
        private void label(JazTuple<string, string> instruction) 
		{ 
		}

		// Go to the line designated by the label given.
        private void goTo(JazTuple<string, string> instruction) 
		{
			string label = instruction.getArgs ();
			lineNumber = env.getLineForLabel(label);
		}

		// Go to the line designated by the label given if equals is true.
        private void goFalse(JazTuple<string, string> instruction) 
		{
			
		}

		// Go to the line designated by the label given if equals is true.
        private void goTrue(JazTuple<string, string> instruction) 
		{
			
		}

		// Pop the first two items on the stack, add them and add the result to the stack.
        private void add(JazTuple<string, string> instruction) 
		{
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			executionStack.Push (Convert.ToString(first + second));
		}

		// Pop the first two items on the stack, subtract them and add the result to the stack.
        private void subtract(JazTuple<string, string> instruction) 
		{ 
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			executionStack.Push (Convert.ToString(second - first));
		}

		// Pop the first two items on the stack, multiply them and add the result to the stack.
        private void multiply(JazTuple<string, string> instruction) 
		{
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			executionStack.Push (Convert.ToString(first * second));
		}

		// Pop the first two items on the stack, divide them and add the result to the stack.
        private void divide(JazTuple<string, string> instruction) 
		{ 
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			executionStack.Push (Convert.ToString(second / first));
		}
        
		// Pop the first two items on the stack, divide them and add the remainder to the stack.
		private void mod(JazTuple<string, string> instruction) 
		{
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			executionStack.Push (Convert.ToString(second % first));
		}

		// Pop the first two items on the stack, and them and add the result to the stack.
        private void and(JazTuple<string, string> instruction) 
		{
			bool first = Convert.ToBoolean (executionStack.Pop ());
			bool second = Convert.ToBoolean (executionStack.Pop ());
			executionStack.Push (Convert.ToString(first && second));
		}

		// Pop the first item on the stack, negate it and add the result to the stack.
        private void neg(JazTuple<string, string> instruction) 
		{ 
			bool value = Convert.ToBoolean (executionStack.Pop ());
			executionStack.Push (Convert.ToString(!value));
		}

		// Pop the first two items on the stack, or them and add the result to the stack.
        private void or(JazTuple<string, string> instruction) 
		{
			bool first = Convert.ToBoolean (executionStack.Pop ());
			bool second = Convert.ToBoolean (executionStack.Pop ());
			executionStack.Push (Convert.ToString(first || second));
		}


        private void notEqual(JazTuple<string, string> instruction) 
		{ 
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			string result = "0";
			if (first != second) 
			{
				result = "1";
			}
			executionStack.Push (result);
		}

        private void lessThanEqualTo(JazTuple<string, string> instruction) 
		{
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			string result = "0";
			if (first <= second) 
			{
				result = "1";
			}
			executionStack.Push (result);
		}

        private void greaterThanEualTo(JazTuple<string, string> instruction) 
		{
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			string result = "0";
			if (first >= second) 
			{
				result = "1";
			}
			executionStack.Push (result);
		}
			
        private void lessThan(JazTuple<string, string> instruction) 
		{ 
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			string result = "0";
			if (first < second) 
			{
				result = "1";
			}
			executionStack.Push (result);
		}

        private void greaterThan(JazTuple<string, string> instruction) 
		{ 
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			string result = "0";
			if (first > second) 
			{
				result = "1";
			}
			executionStack.Push (result);
		}

        private void equal(JazTuple<string, string> instruction) 
		{ 
			long first = Convert.ToInt64 (executionStack.Pop ());
			long second = Convert.ToInt64 (executionStack.Pop ());
			string result = "0";
			if (first == second) 
			{
				result = "1";
			}
			executionStack.Push (result);
		}

		// Print the top item from the stack and print it.
        private void print(JazTuple<string, string> instruction) 
		{
			Console.WriteLine(executionStack.Peek());
        }

		// Show the value given.
        private void show(JazTuple<string, string> instruction)
        {
            Console.WriteLine(instruction.getArgs());
        }

		// Begin passing arguments. We create a new stack for the subfunction.
        private void begin(JazTuple<string, string> instruction) 
		{
			env.pushStack (executionStack);
			executionStack = new Stack<string>();
		}

		// End passing arguments. We revert to the old stack from before the subfunction.
        private void end(JazTuple<string, string> instruction) 
		{
			executionStack = env.popStack ();
		}

		// Return to the line we called the function from.
        private void ret(JazTuple<string, string> instruction) 
		{
			lineNumber = env.endProc ();
		}

		// Call the line designated by the label given.
        private void call(JazTuple<string, string> instruction) 
		{
			env.startProc (lineNumber);
			goTo (instruction);
		}
    }
    
    public class Parser
    {
		public List<JazTuple<string, string>> SymbolTable 
		{
			get;
			private set;
		}

		public JazEnv Env 
		{
			get;
			private set;
		}

        public Parser(string file)
        {
			Env = new JazEnv();
			SymbolTable = new List<JazTuple<string, string>>();
            this.parseFile(file);
        }

        private void parseFile(string file)
        {
            using (System.IO.StreamReader sr = System.IO.File.OpenText(file))
            {
                string t;
                int lineNumber = 0;
                while ((t = sr.ReadLine()) != null)
                {
                    lineNumber = parse(t, lineNumber);
                }
            }
        }

        private int parse(string t, int lineNumber) {
            t = t.Trim();
            string label = t.Split(' ')[0].Trim();
            string args = t.Substring(t.Split(' ')[0].Length).Trim();

            if(!string.IsNullOrEmpty(label)) {
                //load tuples into symbol table
                if(label == "label")
                {
                    Env.putLabel(args, lineNumber);
                    SymbolTable.Add(new JazTuple<string, string>(label, args));
                }
                else
                {
                    SymbolTable.Add(new JazTuple<string, string>(label, args));
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

        public string toString()
        {
            return "(" + A + " " + R + ")";
        }
    }
}

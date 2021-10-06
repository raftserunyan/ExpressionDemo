using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ExpressionDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();

			//PrintExpression(k => k < 2);

			//Compiling for the first time
			sw.Reset();
			sw.Start();
			var method = IsLengthGreaterThanFourExpression();
			sw.Stop();
			Console.WriteLine($"Elapsed on compiling the expression for the first time: {sw.Elapsed} sec.");

			//Compiling second time
			sw.Reset();
			sw.Start();
			var method1 = IsLengthGreaterThanFourExpression();
			sw.Stop();
			Console.WriteLine($"Elapsed on compiling the expression for second time: {sw.Elapsed} sec.");


			Console.WriteLine("\n---------------------------------------------");
			Console.WriteLine("---------------------------------------------\n\n");

			for (int i = 0; i < 3; i++)
			{
				sw.Start();
				var result1 = IsLengthGreaterThanFour("Rafo");
				sw.Stop();
				Console.WriteLine("Calling the ordinary method");
				Console.WriteLine($"Elapsed time in milliseconds: {sw.Elapsed}");
				Console.WriteLine("---------------------------------------------");

				sw.Reset();
				sw.Start();
				var result2 = method("Rafo");
				sw.Stop();
				Console.WriteLine("Calling the already compiled expression method");
				Console.WriteLine($"Elapsed time in milliseconds: {sw.Elapsed}");

				Console.WriteLine("\n\n\n");
			}
		}

		static int IsLengthGreaterThanFour(string s)
		{
			int result = 0;

			if (s.Length == 4)
				result = 1 + 1;
			else
				result = int.Parse(String.Concat("1", "1"));

			return result;
		}

		static Func<string, int> IsLengthGreaterThanFourExpression()
		{
			var sw = new Stopwatch();

			sw.Reset();
			sw.Start();
			var param = Expression.Parameter(typeof(string), "input");

			var paramLength = Expression.Property(param, nameof(string.Length));
			var minLength = Expression.Constant(4);

			var isGreaterThanFour = Expression.Equal(paramLength, minLength);

			var intParseMethod = typeof(int).GetMethod(nameof(int.Parse), new[] { typeof(string) });
			var stringConcatMethod = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(String), typeof(String) });
			var intEqualsMethod = typeof(int).GetMethod(nameof(int.Equals), new[] { typeof(int) });

			var condition = Expression.Call(paramLength, intEqualsMethod, minLength);
			var ifTrue = Expression.Add(Expression.Constant(1), Expression.Constant(1));
			var ifFalse = Expression.Call(intParseMethod,
											Expression.Call(stringConcatMethod,
															Expression.Constant("1"),
															Expression.Constant("1")));

			var conditional = Expression.Condition(isGreaterThanFour, ifTrue, ifFalse);

			var lambda = Expression.Lambda<Func<string, int>>(conditional, param);
			//sw.Stop();
			//Console.WriteLine($"Elapsed on preparing the expression: {sw.Elapsed}\n\n\n");

			//sw.Reset();
			//sw.Start();
			var valod = lambda.Compile();
			//sw.Stop();
			//Console.WriteLine($"Elapsed on compiling the expression: {sw.Elapsed}\n\n\n");

			return valod;
		}

		static void PrintExpression(Expression<Func<int, bool>> expression)
		{
			ParameterExpression param = (ParameterExpression)expression.Parameters[0];
			BinaryExpression operation = (BinaryExpression)expression.Body;
			ParameterExpression left = (ParameterExpression)operation.Left;
			ConstantExpression right = (ConstantExpression)operation.Right;

			Console.WriteLine("Decomposed expression: {0} => {1} {2} {3}",
				  param.Name, left.Name, operation.NodeType, right.Value);
		}
	}
}

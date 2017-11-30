using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BloxEngine
{
	[ExcludeFromBlox]
	public static class BloxMathsUtil
	{
		private class Opt
		{
			public Type ta;

			public Type tb;

			public Delegate comm;
		}

		private static List<Opt> addCache = new List<Opt>();

		private static List<Opt> subCache = new List<Opt>();

		private static List<Opt> mulCache = new List<Opt>();

		private static List<Opt> divCache = new List<Opt>();

		private static List<Opt> modCache = new List<Opt>();

		private static List<Opt> andCache = new List<Opt>();

		private static List<Opt> orCache = new List<Opt>();

		private static List<Opt> xorCache = new List<Opt>();

		private static List<Opt> lsCache = new List<Opt>();

		private static List<Opt> rsCache = new List<Opt>();

		public static object Add(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.Add, BloxMathsUtil.addCache, a, b);
		}

		public static object Subtract(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.Subtract, BloxMathsUtil.subCache, a, b);
		}

		public static object Multiply(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.Multiply, BloxMathsUtil.mulCache, a, b);
		}

		public static object Divide(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.Divide, BloxMathsUtil.divCache, a, b);
		}

		public static object Modulo(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.Modulo, BloxMathsUtil.modCache, a, b);
		}

		public static object BitwiseAnd(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.And, BloxMathsUtil.andCache, a, b);
		}

		public static object BitwiseOr(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.Or, BloxMathsUtil.orCache, a, b);
		}

		public static object BitwiseXor(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.ExclusiveOr, BloxMathsUtil.xorCache, a, b);
		}

		public static object LeftShift(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.LeftShift, BloxMathsUtil.lsCache, a, b);
		}

		public static object RightShift(object a, object b)
		{
			return BloxMathsUtil._DoOperation(Expression.RightShift, BloxMathsUtil.rsCache, a, b);
		}

		private static Opt FindOpt(List<Opt> lst, Type ta, Type tb)
		{
			if (lst == null)
			{
				lst = new List<Opt>();
			}
			for (int i = 0; i < lst.Count; i++)
			{
				if (lst[i].ta == ta && lst[i].tb == tb)
				{
					return lst[i];
				}
			}
			return null;
		}

		private static object _DoOperation(Func<Expression, Expression, BinaryExpression> exp, List<Opt> cache, object a, object b)
		{
			if (a != null && b != null)
			{
				Type type = a.GetType();
				Type type2 = b.GetType();
				try
				{
					if (exp == new Func<Expression, Expression, BinaryExpression>(Expression.Add) && (type == typeof(string) || type2 == typeof(string)))
					{
						return a.ToString() + b.ToString();
					}
					Opt opt = BloxMathsUtil.FindOpt(cache, type, type2);
					if (opt == null)
					{
						ParameterExpression parameterExpression = Expression.Parameter(type, "a");
						ParameterExpression parameterExpression2 = Expression.Parameter(type2, "b");
						Delegate @delegate = null;
						try
						{
							@delegate = Expression.Lambda(exp(parameterExpression, parameterExpression2), parameterExpression, parameterExpression2).Compile();
						}
						catch (InvalidOperationException)
						{
							if (type != type2)
							{
								Expression arg = parameterExpression;
								Expression arg2 = Expression.Convert(parameterExpression2, type);
								@delegate = Expression.Lambda(exp(arg, arg2), parameterExpression, parameterExpression2).Compile();
								goto end_IL_00b8;
							}
							throw;
							end_IL_00b8:;
						}
						opt = new Opt
						{
							ta = type,
							tb = type2,
							comm = @delegate
						};
						cache.Add(opt);
					}
					if (opt != null)
					{
						return opt.comm.DynamicInvoke(a, b);
					}
				}
				catch
				{
					throw new Exception(string.Format("The operation can't be completed with [{0}] and [{1}].", type, type2));
				}
				return null;
			}
			throw new Exception("The operation can't be completed since one or both values are null.");
		}
	}
}

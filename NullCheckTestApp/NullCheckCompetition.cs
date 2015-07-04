using System;
using System.Linq.Expressions;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace NullCheckTestApp
{
    [Task(
            platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V40
    )]
    [Task(
            platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V40
    )]
    [Task(
            platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V45
    )]
    [Task(
            platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V45
    )]
    public class NullCheckCompetition
    {
        private object m_nullObjectToCheck = default(object);
        private object m_nonNullObjectToCheck = new object();

        [Benchmark()]
        public void FailAnonymousClass()
        {
            NullCheckThroughAnonymousClass(m_nullObjectToCheck, new { m_nullObjectToCheck });
        }

        [Benchmark()]
        public void FailLazyAnonymousClass()
        {
            NullCheckThroughLazyAnonymousClass(m_nullObjectToCheck, () => new { m_nullObjectToCheck });
        }

        [Benchmark()]
        public void FailRawCheck()
        {
            NullCheckThroughRawCheck(m_nullObjectToCheck, "m_nullObjectToCheck");
        }

        [Benchmark()]
        public void FailSingleLambda()
        {
            NullCheckThroughSingleLambda(m_nullObjectToCheck, () => m_nullObjectToCheck);
        }

        [Benchmark()]
        public void FailDoubleLambda()
        {
            NullCheckThroughDoubleLambda(m_nullObjectToCheck, () => () => m_nullObjectToCheck);
        }


        [Benchmark()]
        public void SuccessAnonymousClass()
        {
            NullCheckThroughAnonymousClass(m_nonNullObjectToCheck, new { m_objectToCheck = m_nonNullObjectToCheck });
        }

        [Benchmark()]
        public void SuccessLazyAnonymousClass()
        {
            NullCheckThroughLazyAnonymousClass(m_nonNullObjectToCheck, () => new { m_objectToCheck = m_nonNullObjectToCheck });
        }

        [Benchmark()]
        public void SuccessRawCheck()
        {
            NullCheckThroughRawCheck(m_nonNullObjectToCheck, "m_nonNullObjectToCheck");
        }

        [Benchmark()]
        public void SuccessSingleLambda()
        {
            NullCheckThroughSingleLambda(m_nonNullObjectToCheck, () => m_nonNullObjectToCheck);
        }

        [Benchmark()]
        public void SuccessDoubleLambda()
        {
            NullCheckThroughDoubleLambda(m_nonNullObjectToCheck, () => () => m_nonNullObjectToCheck);
        }

        public static string NullCheckThroughLazyAnonymousClass<T>(object _objectToValidate, Func<T> _containerProvider) where T : class
        {
            if (_objectToValidate != null)
            {
                return string.Empty;
            }

            return NullCheckThroughAnonymousClass(_objectToValidate, _containerProvider());
        }

        private static string NullCheckThroughAnonymousClass<T>(object _objectToValidate, T _container) where T : class
        {
            if (_objectToValidate != null || _container == null)
            {
                return string.Empty;
            }

            var properties = typeof(T).GetProperties();
            if (properties.Length != 1)
            {
                throw new ArgumentException(string.Format("Expected anonymous class with exactly one property, got {0}", typeof(T)));
            }
            return properties[0].Name;
        }


        private static string NullCheckThroughRawCheck(object _objectToValidate, string _objectName)
        {
            if (_objectToValidate != null || _objectName == null)
            {
                return string.Empty;
            }

            return _objectName;
        }

        private static string NullCheckThroughSingleLambda(object _objectToValidate, Expression<Func<object>> _expression)
        {
            if (_objectToValidate != null || _expression == null)
            {
                return string.Empty;
            }
            return ExtractNameFromExpression(_expression);
        }

        private static string NullCheckThroughDoubleLambda(object _objectToValidate, Func<Expression<Func<object>>> _expression)
        {
            if (_objectToValidate != null || _expression == null)
            {
                return string.Empty;
            }
            return ExtractNameFromExpression(_expression());
        }

        private static string ExtractNameFromExpression(Expression<Func<object>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body == null)
            {
                return string.Empty;
            }
            return body.Member.Name;
        }
    }
}
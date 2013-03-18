using System;
using System.Collections.Generic;

namespace NugetSolutionValidator// $rootnamespace$.NuGet
{
    /// <summary>
    /// Equality comparer that accepts a function delegate.
    /// </summary>
    public class PredicateEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _predicate;

        public PredicateEqualityComparer(Func<T, T, bool> predicate)
            : base()
        {
            this._predicate = predicate;
        }

        public bool Equals(T x, T y)
        {
            if (x != null)
            {
                return ((y != null) && this._predicate(x, y));
            }

            if (y != null)
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
﻿using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal representing a variable clause
    /// </summary>
    public class Variable : ILiteral, IEquatable<Variable>
    {
        /// <summary>
        /// The identifier for this variable
        /// </summary>
        private readonly long _identifier;

        /// <summary>
        /// The identifier to assign to the next variable created
        /// </summary>
        private static long _nextIdentifier = 0;

        public Variable()
        {
            _identifier = Interlocked.Increment(ref _nextIdentifier);
        }

        public IEnumerable<IUnificationState> Unify(ILiteral unifyWith, IUnificationState state)
        {
            ILiteral currentValue;

            if (Equals(this, unifyWith))
            {
                // Variable unifies with itself
                yield return state;
            }
            else if (state.TryGetBindingForVariable(this, out currentValue))
            {
                // If there's an existing value, result is the unification of the two
                foreach (var unifiedValue in currentValue.Unify(unifyWith, state))
                {
                    yield return unifiedValue;
                }
            }
            else
            {
                // Otherwise, bind the variable to the value it's being unified with
                yield return state.StateWithBinding(this, unifyWith);
            }
        }

        public ILiteral Bind(IUnificationState state)
        {
            ILiteral value;
            if (state.TryGetBindingForVariable(this, out value))
            {
                return value;
            }
            else
            {
                return this;
            }
        }

        public bool Equals(Variable other)
        {
            if (other == null) return false;

            return other._identifier == _identifier;
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as Variable);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Variable);
        }

        public override int GetHashCode()
        {
            return _identifier.GetHashCode();
        }

        public void UnifyQuery(IQueryUnifier unifier)
        {
            if (unifier.HasVariable(this))
            {
                unifier.SetValue(this);
            }
            else
            {
                unifier.SetVariable(this);
            }
        }

        public void UnifyProgram(IProgramUnifier unifier)
        {
            if (unifier.HasVariable(this))
            {
                unifier.UnifyVariable(this);
            }
            else
            {
                unifier.UnifyVariable(this);
            }
        }
    }
}

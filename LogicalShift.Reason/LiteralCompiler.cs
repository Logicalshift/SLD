﻿using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason
{
    /// <summary>
    /// Compiles literals for unification
    /// </summary>
    public static class LiteralCompiler
    {
        /// <summary>
        /// Binds the variables in the unifier for a set of assignments
        /// </summary>
        /// <returns>
        /// The list of free variables in the assignments
        /// </returns>
        public static IEnumerable<ILiteral> Bind(this IBaseUnifier unifier, IEnumerable<IAssignmentLiteral> assignments)
        {
            var freeVariables = new List<ILiteral>();

            foreach (var assign in assignments)
            {
                if (assign.Value.UnificationKey == null)
                {
                    // Values without a unification key are free variables (they can have any value)
                    freeVariables.Add(assign.Value);
                    unifier.BindVariable(assign.Variable, assign.Value);
                }
                else
                {
                    // Values with a unification key are not free
                    unifier.BindVariable(assign.Variable, assign.Value.UnificationKey);
                }
            }

            return freeVariables;
        }

        /// <summary>
        /// Compiles a literal for unification using a query unifier
        /// </summary>
        /// <returns>
        /// The list of variables in the literal
        /// </returns>
        public static IEnumerable<ILiteral> Compile(this IQueryUnifier unifier, ILiteral literal)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (literal == null) throw new ArgumentNullException("literal");

            // Flatten the literal
            var assignments = literal.Flatten().ToList();

            // Bind the variables in order
            var freeVariables = unifier.Bind(assignments);

            // Compile subterms to terms
            foreach (var assign in assignments.OrderTermsAfterSubterms())
            {
                assign.CompileQuery(unifier);
            }

            return freeVariables;
        }

        /// <summary>
        /// Compiles a literal for unification using a program unifier
        /// </summary>
        /// <returns>
        /// The list of variables in this literal
        /// </returns>
        public static IEnumerable<ILiteral> Compile(this IProgramUnifier unifier, ILiteral literal)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (literal == null) throw new ArgumentNullException("literal");

            // Flatten the literal
            var assignments = literal.Flatten().ToList();

            // Bind the variables in order
            var freeVariables = unifier.Bind(assignments);

            foreach (var assign in assignments.OrderSubtermsAfterTerms())
            {
                assign.CompileProgram(unifier);
            }

            return freeVariables;
        }
    }
}

﻿using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Methods for ordering assignments so that permanent variables are ordered first
    /// </summary>
    /// <remarks>
    /// The ordering is permanent variables, assignments, then the rest. Permanent variables can thus be
    /// allocated starting at 0 (displacing arguments, which can be moved up the list)
    /// </remarks>
    public static class PermanentVariableAssignments
    {
        /// <summary>
        /// Orders assignments so that permanent variables are first, followed by arguments, followed by any other assignments
        /// </summary>
        public static IEnumerable<IAssignmentLiteral> OrderPermanentVariablesFirst(this IEnumerable<IAssignmentLiteral> assignments, int numArguments, IEnumerable<ILiteral> permanentVariables)
        {
            // Convert the list of permanent variables into a hash set
            var isPermanent = permanentVariables as HashSet<ILiteral> ?? new HashSet<ILiteral>(permanentVariables);

            // ICollections can be iterated over multiple times
            var assignmentCollection = assignments as ICollection<IAssignmentLiteral> ?? assignments.ToArray();

            // Perform the ordering
            var arguments = assignmentCollection.Take(numArguments);
            var permanent = assignmentCollection.Skip(numArguments).Where(assign => isPermanent.Contains(assign.Value));
            var remainder = assignmentCollection.Skip(numArguments).Where(assign => !isPermanent.Contains(assign.Value));

            // Result is arguments followed by the list with the permanent variables first
            return permanent.Concat(arguments).Concat(remainder);
        }

        /// <summary>
        /// Returns the names of the variables that get assigned in a list of assignments
        /// </summary>
        public static IEnumerable<ILiteral> VariablesAssigned(this IEnumerable<IAssignmentLiteral> assignments)
        {
            return assignments
                .Where(assign => assign.Value.UnificationKey == null)
                .Select(assign => assign.Value);
        }

        /// <summary>
        /// Given a list of predicate assignment lists (representing the assignments for the initial predicate and
        /// the clauses, in order), finds the names of the 'permanent' variables: that is, the list of variables
        /// that are used in more than one clause.
        /// </summary>
        public static HashSet<ILiteral> PermanentVariables(this IEnumerable<PredicateAssignmentList> assignmentLists)
        {
            // The predicate and the first clause don't have a call between them, so variables used in both places 
            // aren't considered to be permanent
            var usedVariables = new HashSet<ILiteral>();
            var permanentVariabels = new HashSet<ILiteral>();
            int pos = 0;

            foreach (var assignmentList in assignmentLists)
            {
                var assignmentVariables = VariablesAssigned(assignmentList.Assignments).ToArray();

                // After the predicate (pos = 0) and first clause (pos = 1), re-used variables need to be marked as permanent
                if (pos > 1)
                {
                    permanentVariabels.UnionWith(assignmentVariables.Where(variable => usedVariables.Contains(variable)));
                }

                // Mark this set of variables as used
                usedVariables.UnionWith(assignmentVariables);

                ++pos;
            }

            return permanentVariabels;
        }
    }
}

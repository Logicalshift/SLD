﻿namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Low-level operations for a program unifier
    /// </summary>
    public interface IProgramUnifier : IBaseUnifier
    {
        /// <summary>
        /// Retrieves a structure into a variable and sets read or write mode
        /// </summary>
        void GetStructure(ILiteral termName, int termLength, ILiteral variable);

        /// <summary>
        /// Retrieves the value of a variable that hasn't been read before
        /// </summary>
        void UnifyVariable(ILiteral variable);

        /// <summary>
        /// Unifies a variable that has been previously read
        /// </summary>
        void UnifyValue(ILiteral variable);

        /// <summary>
        /// Assigns variable2 to variable1
        /// </summary>
        void GetVariable(ILiteral variable1, ILiteral variable2);

        /// <summary>
        /// Unifies variable1 and variable2
        /// </summary>
        void GetValue(ILiteral variable1, ILiteral variable2);
    }
}

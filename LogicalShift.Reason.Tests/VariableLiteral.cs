﻿using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class VariableLiteral
    {
        [Test]
        public void VariableEqualsSelf()
        {
            var variable = Literal.NewVariable();
            Assert.IsTrue(Equals(variable, variable));
        }

        [Test]
        public void VariableUnificationCreatesBinding()
        {
            var variable = Literal.NewVariable();
            var atom = Literal.NewAtom();

            var states = variable.Unify(atom).ToList();

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(atom, states[0]);
        }

        [Test]
        public void AtomUnificationCreatesBinding()
        {
            var variable = Literal.NewVariable();
            var atom = Literal.NewAtom();

            var states = atom.Unify(variable).ToList();

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(atom, states[0]);
        }

        [Test]
        public void VariableIsFreeVariable()
        {
            var variable = Literal.NewVariable();
            var unifier = new SimpleUnifier();

            var freeVars = unifier.QueryUnifier.Compile(variable).ToList();
            Assert.AreEqual(1, freeVars.Count);
            Assert.AreEqual(variable, freeVars[0]);
        }
    }
}

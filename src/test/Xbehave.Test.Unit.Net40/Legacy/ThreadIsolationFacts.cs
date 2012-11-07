﻿// <copyright file="ThreadIsolationFacts.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Legacy
{
    using System.Threading;
    using Xunit;

    public static class ThreadIsolationFacts
    {
        [Fact]
        public static void CanSetUpSpecificationConcurrently()
        {
            VerifyConcurrentExecution(SetUpSpecification);
        }

        [Fact]
        public static void DoEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Do(() => { }));
        }

        [Fact]
        public static void AssertEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Assert(() => { }));
        }

        [Fact]
        public static void ObservationEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Observation(() => { }));
        }

        [Fact]
        public static void TodoEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Todo(() => { }));
        }

        private static void VerifyConcurrentExecution(ThreadStart action)
        {
            Assert.DoesNotThrow(() =>
            {
                // We could use the Task API here but we want to be explicit about getting two concurrent, physical threads
                var a = new Thread(action);
                var b = new Thread(action);

                a.Start();
                b.Start();

                a.Join();
                b.Join();
            });
        }

        private static void SetUpSpecification()
        {
            "foo".Context(() => { });
            "foo".Do(() => { });
            "foo".Assert(() => { });
        }
    }
}
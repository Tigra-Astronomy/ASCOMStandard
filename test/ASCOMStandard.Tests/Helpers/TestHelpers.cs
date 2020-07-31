// This file is part of the ASCOMStandard project
//
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so. The Software comes with no warranty of any kind.
// You make use of the Software entirely at your own risk and assume all liability arising from your use thereof.
//
// File: AssertExtensions.cs  Last modified: 2020-07-31@09:27 by Tim Long

using System;
using System.Globalization;
using System.Threading;
using Xunit.Sdk;

namespace ASCOM.Alpaca.Tests.Helpers
    {
    internal static class TestHelpers
        {
        public static void AssertIsCloseTo(this double actual, double expected, double tolerance = 1e-9)
            {
            var difference = Math.Abs(actual - expected);
            if ((difference > tolerance))
                throw new AssertActualExpectedException(expected, actual,
                    $"Floating point values should have differed by less than {tolerance} but the difference was {difference}"
                );
            }

        /// <summary>Runs an action in a new thread with the specified culture.</summary>
        /// <param name="culture">The culture to be used in the thread.</param>
        /// <param name="action">The action to run.</param>
        public static void InCulturedThread(CultureInfo culture, Action action)
            {
            Exception threadException = null;
            var thread = new Thread(() =>
                {
                try
                    {
                    Console.WriteLine($"Running in culture {culture.Name}");
                    CultureInfo.CurrentCulture = culture;
                    action();
                    }
                catch (Exception ex)
                    {
                    // Capture any exception thrown inside the thread.
                    threadException = ex;
                    }
                });
            thread.Start();
            thread.Join();
            // If the thread threw an exception, re-throw it outside the thread.
            if (threadException != null) throw threadException;
            }
        }
    }
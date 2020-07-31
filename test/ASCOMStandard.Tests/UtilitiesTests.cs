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
// File: UtilitiesTests.cs  Last modified: 2020-07-31@10:52 by Tim Long

using System.Globalization;
using ASCOM.Alpaca.Tests.Helpers;
using ASCOM.Standard.Utilities;
using Xunit;

namespace ASCOM.Alpaca.Tests
    {
    public class DMSToDegreesTests
        {
        private readonly Utilities utils = new Utilities();

        [Fact]
        public void NegativeDegreesShouldProduceNegativeValue()
            {
            var actual = utils.DMSToDegrees("-10:0:0");
            actual.AssertIsCloseTo(-10.0);
            }

        [Fact]
        public void MinutesAreNegativeWhenDegreesAreNegative()
            {
            var actual = utils.DMSToDegrees("-10:30:0");
            actual.AssertIsCloseTo(-10.5);
            }

        [Fact]
        public void SecondsAreNegativeWhenDegreesAreNegative()
            {
            var actual = utils.DMSToDegrees("-10:0:30");
            actual.AssertIsCloseTo(-10.00833, 0.00001);
            }

        [Fact]
        public void MinutesCannotBeGreaterThan59()
            {
            Assert.Throws<InvalidValueException>(() => utils.DMSToDegrees("00:60:00"));
            }

        [Fact]
        public void SecondsCannotBeGreaterThan59()
            {
            Assert.Throws<InvalidValueException>(() => utils.DMSToDegrees("00:00:60"));
            }

        [Fact]
        public void ExtraFieldsAreIgnored()
            {
            var actual = utils.DMSToDegrees("10:20:30:40:50:60");
            actual.AssertIsCloseTo(10.341667, 0.000001);
            }

        [Fact]
        public void DecimalsAreAllowedInAllFields()
            {
            var actual = utils.DMSToDegrees("10.1:20.2:30.3");
            actual.AssertIsCloseTo(10.445083, 0.000001);
            }

        /*Bug: unsafe cultural assumption
         * While the code works as documented, it makes an unsafe assumption that will likely lead to bugs.
         * As a library write, we have no idea what culture the supplied string is in, so it is unsafe
         * to assume that it is in the current culture.
         * This is probably the number one source of bugs in astronomy software, and there is no
         * one-size-fits-all correct assumption. If the DMS string was user input then it will most
         * likely be in the current culture, but if it came from a device or was retrieved from storage,
         * then it will most likely be in the invariant culture or some device-specific culture.
         * There is no safe correct assumption!
         * In order to "nudge the user into the pit of success" we should require them to be explicit
         * about what they intended. Therefore, a better approach would be to require a CultureInfo argument
         * to be passed to the method. If there is a backward-compatibility concern, then a compatible
         * version of the method can be provided but marked
         * [Obsolete("Use the overload that takes a CultureInfo argument")]
         */
        [Fact]
        public void DecimalSeparatorIsCultureSensitive()
            {
            double actual = double.NaN;
            TestHelpers.InCulturedThread(CultureInfo.CreateSpecificCulture("DE"),
                () => { actual = utils.DMSToDegrees("10,1:20,2:30,3"); });
            actual.AssertIsCloseTo(10.445083, 0.000001);
            }
        }
    }
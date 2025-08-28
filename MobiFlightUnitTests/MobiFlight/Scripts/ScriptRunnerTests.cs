using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Scripts;
using System;
using System.Collections.Generic;

namespace MobiFlightUnitTests.MobiFlight.Scripts
{
    [TestClass]
    public class ScriptRunnerTests
    {
        public static IEnumerable<object[]> InstalledPackagesTestData
        {
            get
            {
                return new[]
                {
                    new object[] // No packages detected
                    {
                        false,
                        new Dictionary<string, Version>(), 
                    },
                    new object[]
                    {
                        false,
                        new Dictionary<string, Version> // Missing package
                        {
                            { "websockets", new Version(14,0) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        false,
                        new Dictionary<string, Version> // Missing package
                        {
                            { "websockets", new Version(14,0) },
                            { "gql", new Version(3,5) },
                        },
                    },
                    new object[]
                    {
                        false,
                        new Dictionary<string, Version> // Missing package
                        {
                            { "gql", new Version(3,5) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        false,
                        new Dictionary<string, Version> // Smaller major version installed
                        {
                            { "websockets", new Version(13,0) },
                            { "gql", new Version(3,5) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        false,
                        new Dictionary<string, Version> // Smaller minor version installed
                        {
                            { "websockets", new Version(14,0) },
                            { "gql", new Version(3,4) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        true,
                        new Dictionary<string, Version> // Minimum required versions installed
                        {
                            { "websockets", new Version(14,0) },
                            { "gql", new Version(3,5) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        true,
                        new Dictionary<string, Version> // Newer major version installed
                        {
                            { "websockets", new Version(14,0) },
                            { "gql", new Version(4,0) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        true,
                        new Dictionary<string, Version> // Newer minor version installed
                        {
                            { "websockets", new Version(14,2) },
                            { "gql", new Version(3,5) },
                            { "SimConnect", new Version(0,4) },
                        },
                    },
                    new object[]
                    {
                        true,
                        new Dictionary<string, Version> // Newer revision version installed
                        {
                            { "websockets", new Version(14,0) },
                            { "gql", new Version(3,5) },
                            { "SimConnect", new Version(0,4,2) },
                        },
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(InstalledPackagesTestData))]
        public void ValidateAreNecessaryPackagesInstalled_ReturnsExpected(
            bool expectedResult,
            Dictionary<string, Version> installedPackages)
        {
            var actualResult = ScriptRunner.ValidateNecessaryPackagesInstalled(installedPackages);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}

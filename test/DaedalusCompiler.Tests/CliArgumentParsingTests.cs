using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaedalusCompiler;
using Xunit;

namespace DaedalusCompiler.Tests
{
    /// <summary>
    /// Tests for CLI argument parsing using the real ProcessCliArguments method
    /// </summary>
    public class CliArgumentParsingTests
    {
        [Fact]
        public void TestBasicArguments()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: false,
                zenPaths: null,
                verbose: false);

            // Assert
            Assert.Equal("test.src", result.SrcFilePath);
            Assert.Equal(string.Empty, result.RuntimePath);
            Assert.Equal(Path.Combine("output", "test.dat"), result.OutputPathDat);
            Assert.Equal("output", result.OutputPathOu);
            Assert.False(result.GenerateOutputUnits);
            Assert.False(result.Strict);
            Assert.False(result.Verbose);
            
            // Should have default suppress codes for non-case-sensitive and non-detect-unused
            Assert.Contains("W2", result.SuppressCodes); // NamesNotMatchingCaseWiseWarning.WCode
            Assert.Contains("W3", result.SuppressCodes); // UnusedSymbolWarning.WCode
        }

        [Fact]
        public void TestAllArgumentsProvided()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "gothic.src",
                runtime: "runtime.d",
                outputDat: "custom/gothic.dat",
                genOu: true,
                outputOu: "customou",
                strict: true,
                caseSensitive: true,
                suppress: "W1:W2:W3",
                detectUnused: true,
                zenPaths: "zen1.zen:zen2.zen",
                verbose: true);

            // Assert
            Assert.Equal("gothic.src", result.SrcFilePath);
            Assert.Equal("runtime.d", result.RuntimePath);
            Assert.Equal("custom/gothic.dat", result.OutputPathDat);
            Assert.Equal("customou", result.OutputPathOu);
            Assert.True(result.GenerateOutputUnits);
            Assert.True(result.Strict);
            Assert.True(result.Verbose);

            // Should have explicitly provided suppress codes
            Assert.Contains("W1", result.SuppressCodes);
            Assert.Contains("W2", result.SuppressCodes);
            Assert.Contains("W3", result.SuppressCodes);
            
            // Should have zen paths processed
            Assert.Equal(2, result.ZenPaths.Count);
            Assert.Contains("zen1.zen", result.ZenPaths);
            Assert.Contains("zen2.zen", result.ZenPaths);
        }

        [Fact]
        public void TestCaseSensitiveRemovesWarningSupression()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: true, // This should NOT add W2
                suppress: null,
                detectUnused: false,
                zenPaths: null,
                verbose: false);

            // Assert
            Assert.DoesNotContain("W2", result.SuppressCodes); // NamesNotMatchingCaseWiseWarning should not be suppressed
            Assert.Contains("W3", result.SuppressCodes); // UnusedSymbolWarning should still be suppressed
        }

        [Fact]
        public void TestDetectUnusedRemovesWarningSupression()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: true, // This should NOT add W3
                zenPaths: null,
                verbose: false);

            // Assert
            Assert.Contains("W2", result.SuppressCodes); // NamesNotMatchingCaseWiseWarning should still be suppressed
            Assert.DoesNotContain("W3", result.SuppressCodes); // UnusedSymbolWarning should not be suppressed
        }

        [Fact]
        public void TestZenPathsAutoEnablesDetectUnused()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: false, // This will be overridden by zenPaths
                zenPaths: "path1.zen:path2.zen",
                verbose: false);

            // Assert
            Assert.Equal(2, result.ZenPaths.Count);
            Assert.Contains("path1.zen", result.ZenPaths);
            Assert.Contains("path2.zen", result.ZenPaths);
            
            // Since zen paths auto-enables detectUnused, W3 should NOT be in suppress codes
            Assert.DoesNotContain("W3", result.SuppressCodes);
            Assert.Contains("W2", result.SuppressCodes); // W2 should still be there (caseSensitive=false)
        }

        [Fact]
        public void TestSuppressCodesAreMergedCorrectly()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: true,  // This should NOT add W2
                suppress: "W1:W4",    // Explicitly provided codes
                detectUnused: true,   // This should NOT add W3
                zenPaths: null,
                verbose: false);

            // Assert
            Assert.Contains("W1", result.SuppressCodes);
            Assert.Contains("W4", result.SuppressCodes);
            
            // When caseSensitive=true and detectUnused=true, default suppressions should NOT be added
            Assert.DoesNotContain("W2", result.SuppressCodes);
            Assert.DoesNotContain("W3", result.SuppressCodes);
        }

        [Theory]
        [InlineData("Gothic.src", "gothic.dat")]
        [InlineData("FIGHT.SRC", "fight.dat")]
        [InlineData("Menu.SRC", "menu.dat")]
        [InlineData("/full/path/Camera.src", "camera.dat")]
        public void TestOutputPathGenerationVariations(string srcFile, string expectedFileName)
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: srcFile,
                runtime: null,
                outputDat: null, // Force default generation
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: false,
                zenPaths: null,
                verbose: false);

            // Assert - Use Path.Combine for cross-platform compatibility
            var expectedOutput = Path.Combine("output", expectedFileName);
            Assert.Equal(expectedOutput, result.OutputPathDat);
        }

        [Fact]
        public void TestCustomOutputPathPreserved()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: "custom/path/myfile.dat",
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: false,
                zenPaths: null,
                verbose: false);

            // Assert
            // Custom output paths should be preserved exactly as provided by the user
            Assert.Equal("custom/path/myfile.dat", result.OutputPathDat);
        }

        [Fact]
        public void TestEmptyZenPathsHandling()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: false,
                zenPaths: "", // Empty string
                verbose: false);

            // Assert
            Assert.Empty(result.ZenPaths);
            // detectUnused should still be false since zenPaths is empty
            Assert.Contains("W3", result.SuppressCodes);
        }

        [Fact]
        public void TestEmptySuppressCodesHandling()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: true,
                suppress: "", // Empty string
                detectUnused: true,
                zenPaths: null,
                verbose: false);

            // Assert
            // Should only have defaults based on caseSensitive and detectUnused flags
            Assert.DoesNotContain("W2", result.SuppressCodes);
            Assert.DoesNotContain("W3", result.SuppressCodes);
        }

        [Fact]
        public void TestSingleZenPath()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: false,
                suppress: null,
                detectUnused: false,
                zenPaths: "single.zen", // Single path without colon
                verbose: false);

            // Assert
            Assert.Single(result.ZenPaths);
            Assert.Contains("single.zen", result.ZenPaths);
        }

        [Fact]
        public void TestSingleSuppressCode()
        {
            // Act
            var result = Program.ProcessCliArguments(
                srcFile: "test.src",
                runtime: null,
                outputDat: null,
                genOu: false,
                outputOu: "output",
                strict: false,
                caseSensitive: true,
                suppress: "W5", // Single code without colon
                detectUnused: true,
                zenPaths: null,
                verbose: false);

            // Assert
            Assert.Single(result.SuppressCodes);
            Assert.Contains("W5", result.SuppressCodes);
        }
    }
}
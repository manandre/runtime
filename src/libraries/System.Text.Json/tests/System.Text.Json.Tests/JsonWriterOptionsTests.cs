// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class JsonWriterOptionsTests
    {
        [Fact]
        public static void JsonWriterOptionsDefaultCtor()
        {
            JsonWriterOptions options = default;

            var expectedOption = new JsonWriterOptions
            {
                Indented = false,
                IndentCharacter = ' ',
                IndentSize = 2,
                NewLine = Environment.NewLine,
                SkipValidation = false,
                MaxDepth = 0,
            };
            Assert.Equal(expectedOption, options);
        }

        [Fact]
        public static void JsonWriterOptionsCtor()
        {
            var options = new JsonWriterOptions();

            var expectedOption = new JsonWriterOptions
            {
                Indented = false,
                IndentCharacter = ' ',
                IndentSize = 2,
                NewLine = Environment.NewLine,
                SkipValidation = false,
                MaxDepth = 0,
            };
            Assert.Equal(expectedOption, options);
        }

        [Theory]
        [InlineData(true, '\t', 1, "\r\n", true, 0)]
        [InlineData(true, ' ', 127, "\n", false, 1)]
        [InlineData(false, ' ', 0, "\n", true, 1024)]
        [InlineData(false, ' ', 4, "\n", false, 1024 * 1024)]
        public static void JsonWriterOptions(bool indented, char indentCharacter, int indentSize, string newLine, bool skipValidation, int maxDepth)
        {
            var options = new JsonWriterOptions();
            options.Indented = indented;
            options.IndentCharacter = indentCharacter;
            options.IndentSize = indentSize;
            options.NewLine = newLine;
            options.SkipValidation = skipValidation;
            options.MaxDepth = maxDepth;

            var expectedOption = new JsonWriterOptions
            {
                Indented = indented,
                IndentCharacter = indentCharacter,
                IndentSize = indentSize,
                NewLine = newLine,
                SkipValidation = skipValidation,
                MaxDepth = maxDepth,
            };
            Assert.Equal(expectedOption, options);
        }

        [Theory]
        [InlineData(true, '\t', 1, "\r\n", true, 0)]
        [InlineData(true, ' ', 127, "\n", false, 1)]
        [InlineData(false, ' ', 0, "\n", true, 1024)]
        [InlineData(false, ' ', 4, "\n", false, 1024 * 1024)]
        public static void JsonWriterOptions_Properties(bool indented, char indentCharacter, int indentSize, string newLine, bool skipValidation, int maxDepth)
        {
            var options = new JsonWriterOptions();
            options.Indented = indented;
            options.IndentCharacter = indentCharacter;
            options.IndentSize = indentSize;
            options.NewLine = newLine;
            options.SkipValidation = skipValidation;
            options.MaxDepth = maxDepth;

            Assert.Equal(indented, options.Indented);
            Assert.Equal(indentCharacter, options.IndentCharacter);
            Assert.Equal(indentSize, options.IndentSize);
            Assert.Equal(newLine, options.NewLine);
            Assert.Equal(skipValidation, options.SkipValidation);
            Assert.Equal(maxDepth, options.MaxDepth);
        }

        [Fact]
        public static void JsonWriterOptions_DefaultValues()
        {
            JsonWriterOptions options = default;

            Assert.False(options.Indented);
            Assert.Equal(' ', options.IndentCharacter);
            Assert.Equal(2, options.IndentSize);
            Assert.Equal(Environment.NewLine, options.NewLine);
            Assert.False(options.SkipValidation);
            Assert.Equal(0, options.MaxDepth);
        }

        [Fact]
        public static void JsonWriterOptions_MultipleValues()
        {
            JsonWriterOptions defaultOptions = default;
            var options = new JsonWriterOptions();

            options.Indented = true;
            options.Indented = defaultOptions.Indented;
            Assert.Equal(defaultOptions.Indented, options.Indented);

            options.IndentCharacter = '\t';
            options.IndentCharacter = defaultOptions.IndentCharacter;
            Assert.Equal(defaultOptions.IndentCharacter, options.IndentCharacter);

            options.IndentSize = 127;
            options.IndentSize = defaultOptions.IndentSize;
            Assert.Equal(defaultOptions.IndentSize, options.IndentSize);

            options.NewLine = Environment.NewLine is "\n" ? "\r\n" : "\n";
            options.NewLine = defaultOptions.NewLine;
            Assert.Equal(defaultOptions.NewLine, options.NewLine);

            options.SkipValidation = true;
            options.SkipValidation = defaultOptions.SkipValidation;
            Assert.Equal(defaultOptions.SkipValidation, options.SkipValidation);

            options.MaxDepth = 1024 * 1024;
            options.MaxDepth = defaultOptions.MaxDepth;
            Assert.Equal(defaultOptions.MaxDepth, options.MaxDepth);

            Assert.Equal(defaultOptions, options);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public static void JsonWriterOptions_MaxDepth_InvalidParameters(int maxDepth)
        {
            var options = new JsonWriterOptions();
            Assert.Throws<ArgumentOutOfRangeException>(() => options.MaxDepth = maxDepth);
        }

        [Theory]
        [InlineData('\f')]
        [InlineData('\n')]
        [InlineData('\r')]
        [InlineData('\0')]
        [InlineData('a')]
        public static void JsonWriterOptions_IndentCharacter_InvalidCharacter(char character)
        {
            var options = new JsonWriterOptions();
            Assert.Throws<ArgumentOutOfRangeException>(() => options.IndentCharacter = character);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(128)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public static void JsonWriterOptions_IndentSize_OutOfRange(int size)
        {
            var options = new JsonWriterOptions();
            Assert.Throws<ArgumentOutOfRangeException>(() => options.IndentSize = size);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\f")]
        [InlineData("\r")]
        [InlineData("\t")]
        [InlineData("\r\r")]
        [InlineData("\n\r")]
        [InlineData("\n\n")]
        public static void JsonWriterOptions_NewLine_InvalidValue(string value)
        {
            var options = new JsonWriterOptions();
            Assert.Throws<ArgumentOutOfRangeException>(() => options.NewLine = value);
        }
    }
}

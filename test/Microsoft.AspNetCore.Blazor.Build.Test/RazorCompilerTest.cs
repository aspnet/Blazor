﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Blazor.Build.Core.RazorCompilation;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Layouts;
using Microsoft.AspNetCore.Blazor.Razor;
using Microsoft.AspNetCore.Blazor.Rendering;
using Microsoft.AspNetCore.Blazor.RenderTree;
using Microsoft.AspNetCore.Blazor.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Microsoft.AspNetCore.Blazor.Build.Test
{
    public class RazorCompilerTest
    {
        [Fact]
        public void RejectsInvalidClassName()
        {
            // Arrange/Act
            var result = CompileToCSharp(
                GetArbitraryPlatformValidDirectoryPath(),
                "Filename with spaces.cshtml",
                "ignored code",
                "ignored namespace");

            // Assert
            Assert.Collection(result.Diagnostics,
                item =>
                {
                    Assert.Equal(RazorCompilerDiagnostic.DiagnosticType.Error, item.Type);
                    Assert.StartsWith($"Invalid name 'Filename with spaces'", item.Message);
                });
        }

        [Theory]
        [InlineData("\\unrelated.cs")]
        [InlineData("..\\outsideroot.cs")]
        public void RejectsFilenameOutsideRoot(string filename)
        {
            // Arrange/Act
            filename = filename.Replace('\\', Path.DirectorySeparatorChar);
            var result = CompileToCSharp(
                GetArbitraryPlatformValidDirectoryPath(),
                filename,
                "ignored code",
                "ignored namespace");

            // Assert
            Assert.Collection(result.Diagnostics,
                item =>
                {
                    Assert.Equal(RazorCompilerDiagnostic.DiagnosticType.Error, item.Type);
                    Assert.StartsWith($"File is not within source root directory: '{filename}'", item.Message);
                });
        }

        [Theory]
        [InlineData("ItemAtRoot.cs", "Test.Base", "ItemAtRoot")]
        [InlineData(".\\ItemAtRoot.cs", "Test.Base", "ItemAtRoot")]
        [InlineData("x:\\dir\\subdir\\ItemAtRoot.cs", "Test.Base", "ItemAtRoot")]
        [InlineData("Dir1\\MyFile.cs", "Test.Base.Dir1", "MyFile")]
        [InlineData("Dir1\\Dir2\\MyFile.cs", "Test.Base.Dir1.Dir2", "MyFile")]
        public void CreatesClassWithCorrectNameAndNamespace(string relativePath, string expectedNamespace, string expectedClassName)
        {
            // Arrange/Act
            relativePath = relativePath.Replace(ArbitraryWindowsPath, GetArbitraryPlatformValidDirectoryPath());
            relativePath = relativePath.Replace('\\', Path.DirectorySeparatorChar);
            var result = CompileToAssembly(
                GetArbitraryPlatformValidDirectoryPath(),
                relativePath,
                "{* No code *}",
                "Test.Base");

            // Assert
            Assert.Empty(result.Diagnostics);
            Assert.Collection(result.Assembly.GetTypes(),
                type =>
                {
                    Assert.Equal(expectedNamespace, type.Namespace);
                    Assert.Equal(expectedClassName, type.Name);
                });
        }

        [Fact]
        public void SupportsPlainText()
        {
            // Arrange/Act
            var component = CompileToComponent("Some plain text");
            var frames = GetRenderTree(component);

            // Assert
            Assert.Collection(frames,
                frame => AssertFrame.Text(frame, "Some plain text", 0));
        }

        [Fact]
        public void SupportsCSharpExpressions()
        {
            // Arrange/Act
            var component = CompileToComponent(@"
                @(""Hello"")
                @((object)null)
                @(123)
                @(new object())
            ");

            // Assert
            var frames = GetRenderTree(component);
            Assert.Collection(frames,
                frame => AssertFrame.Whitespace(frame, 0),
                frame => AssertFrame.Text(frame, "Hello", 1),
                frame => AssertFrame.Whitespace(frame, 2),
                frame => AssertFrame.Whitespace(frame, 3), // @((object)null)
                frame => AssertFrame.Whitespace(frame, 4),
                frame => AssertFrame.Text(frame, "123", 5),
                frame => AssertFrame.Whitespace(frame, 6),
                frame => AssertFrame.Text(frame, new object().ToString(), 7),
                frame => AssertFrame.Whitespace(frame, 8));
        }

        [Fact]
        public void SupportsCSharpFunctionsBlock()
        {
            // Arrange/Act
            var component = CompileToComponent(@"
                @foreach(var item in items) {
                    @item
                }
                @functions {
                    string[] items = new[] { ""First"", ""Second"", ""Third"" };
                }
            ");

            // Assert
            var frames = GetRenderTree(component);
            Assert.Collection(frames,
                frame => AssertFrame.Whitespace(frame, 0),
                frame => AssertFrame.Text(frame, "First", 1),
                frame => AssertFrame.Text(frame, "Second", 1),
                frame => AssertFrame.Text(frame, "Third", 1),
                frame => AssertFrame.Whitespace(frame, 2),
                frame => AssertFrame.Whitespace(frame, 3));
        }

        [Fact]
        public void SupportsElements()
        {
            // Arrange/Act
            var component = CompileToComponent("<myelem>Hello</myelem>");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "myelem", 2, 0),
                frame => AssertFrame.Text(frame, "Hello", 1));
        }

        [Fact]
        public void SupportsSelfClosingElements()
        {
            // Arrange/Act
            var component = CompileToComponent("Some text so elem isn't at position 0 <myelem />");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Text(frame, "Some text so elem isn't at position 0 ", 0),
                frame => AssertFrame.Element(frame, "myelem", 1, 1));
        }

        [Fact]
        public void SupportsVoidHtmlElements()
        {
            // Arrange/Act
            var component = CompileToComponent("Some text so elem isn't at position 0 <img>");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Text(frame, "Some text so elem isn't at position 0 ", 0),
                frame => AssertFrame.Element(frame, "img", 1, 1));
        }

        [Fact]
        public void SupportsComments()
        {
            // Arrange/Act
            var component = CompileToComponent("Start<!-- My comment -->End");
            var frames = GetRenderTree(component);

            // Assert
            Assert.Collection(frames,
                frame => AssertFrame.Text(frame, "Start", 0),
                frame => AssertFrame.Text(frame, "End", 1));
        }

        [Fact]
        public void SupportsAttributesWithLiteralValues()
        {
            // Arrange/Act
            var component = CompileToComponent("<elem attrib-one=\"Value 1\" a2='v2' />");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "elem", 3, 0),
                frame => AssertFrame.Attribute(frame, "attrib-one", "Value 1", 1),
                frame => AssertFrame.Attribute(frame, "a2", "v2", 2));
        }

        [Fact]
        public void SupportsAttributesWithStringExpressionValues()
        {
            // Arrange/Act
            var component = CompileToComponent(
                "@{ var myValue = \"My string\"; }"
                + "<elem attr=@myValue />");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "elem", 2, 0),
                frame => AssertFrame.Attribute(frame, "attr", "My string", 1));
        }

        [Fact]
        public void SupportsAttributesWithNonStringExpressionValues()
        {
            // Arrange/Act
            var component = CompileToComponent(
                "@{ var myValue = 123; }"
                + "<elem attr=@myValue />");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "elem", 2, 0),
                frame => AssertFrame.Attribute(frame, "attr", "123", 1));
        }

        [Fact]
        public void SupportsAttributesWithInterpolatedStringExpressionValues()
        {
            // Arrange/Act
            var component = CompileToComponent(
                "@{ var myValue = \"world\"; var myNum=123; }"
                + "<elem attr=\"Hello, @myValue.ToUpperInvariant()    with number @(myNum*2)!\" />");

            // Assert
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "elem", 2, 0),
                frame => AssertFrame.Attribute(frame, "attr", "Hello, WORLD    with number 246!", 1));
        }

        [Fact]
        public void SupportsAttributesWithEventHandlerValues()
        {
            // Arrange/Act
            var component = CompileToComponent(
                @"<elem attr=@MyHandleEvent />
                @functions {
                    public bool HandlerWasCalled { get; set; } = false;

                    void MyHandleEvent(Microsoft.AspNetCore.Blazor.RenderTree.UIEventArgs eventArgs)
                    {
                        HandlerWasCalled = true;
                    }
                }");
            var handlerWasCalledProperty = component.GetType().GetProperty("HandlerWasCalled");

            // Assert
            Assert.False((bool)handlerWasCalledProperty.GetValue(component));
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "elem", 2, 0),
                frame =>
                {
                    Assert.Equal(RenderTreeFrameType.Attribute, frame.FrameType);
                    Assert.Equal(1, frame.Sequence);
                    Assert.NotNull(frame.AttributeValue);

                    ((UIEventHandler)frame.AttributeValue)(null);
                    Assert.True((bool)handlerWasCalledProperty.GetValue(component));
                },
                frame => AssertFrame.Whitespace(frame, 2));
        }

        [Fact]
        public void SupportsAttributesWithCSharpCodeBlockValues()
        {
            // Arrange/Act
            var component = CompileToComponent(
                @"<elem attr=@{ DidInvokeCode = true; } />
                @functions {
                    public bool DidInvokeCode { get; set; } = false;
                }");
            var didInvokeCodeProperty = component.GetType().GetProperty("DidInvokeCode");
            var frames = GetRenderTree(component);

            // Assert
            Assert.False((bool)didInvokeCodeProperty.GetValue(component));
            Assert.Collection(frames,
                frame => AssertFrame.Element(frame, "elem", 2, 0),
                frame =>
                {
                    Assert.Equal(RenderTreeFrameType.Attribute, frame.FrameType);
                    Assert.NotNull(frame.AttributeValue);
                    Assert.Equal(1, frame.Sequence);

                    ((UIEventHandler)frame.AttributeValue)(null);
                    Assert.True((bool)didInvokeCodeProperty.GetValue(component));
                },
                frame => AssertFrame.Whitespace(frame, 2));
        }

        [Fact]
        public void SupportsUsingStatements()
        {
            // Arrange/Act
            var component = CompileToComponent(
                @"@using System.Collections.Generic
                @(typeof(List<string>).FullName)");
            var frames = GetRenderTree(component);

            // Assert
            Assert.Collection(frames,
                frame => AssertFrame.Whitespace(frame, 0),
                frame => AssertFrame.Text(frame, typeof(List<string>).FullName, 1));
        }

        [Fact]
        public void SupportsAttributeFramesEvaluatedInline()
        {
            // Arrange/Act
            var component = CompileToComponent(
                @"<elem @onclick(MyHandler) />
                @functions {
                    public bool DidInvokeCode { get; set; } = false;
                    void MyHandler()
                    {
                        DidInvokeCode = true;
                    }
                }");
            var didInvokeCodeProperty = component.GetType().GetProperty("DidInvokeCode");

            // Assert
            Assert.False((bool)didInvokeCodeProperty.GetValue(component));
            Assert.Collection(GetRenderTree(component),
                frame => AssertFrame.Element(frame, "elem", 2, 0),
                frame =>
                {
                    Assert.Equal(RenderTreeFrameType.Attribute, frame.FrameType);
                    Assert.NotNull(frame.AttributeValue);
                    Assert.Equal(1, frame.Sequence);

                    ((UIEventHandler)frame.AttributeValue)(null);
                    Assert.True((bool)didInvokeCodeProperty.GetValue(component));
                },
                frame => AssertFrame.Whitespace(frame, 2));
        }

        [Fact]
        public void SupportsChildComponentsViaTemporarySyntax()
        {
            // Arrange/Act
            var testComponentTypeName = typeof(TestComponent).FullName.Replace('+', '.');
            var component = CompileToComponent($"<c:{testComponentTypeName} />");
            var frames = GetRenderTree(component);

            // Assert
            Assert.Collection(frames,
                frame => AssertFrame.Component<TestComponent>(frame, 1, 0));
        }

        [Fact]
        public void CanPassParametersToComponents()
        {
            // Arrange/Act
            var testComponentTypeName = typeof(TestComponent).FullName.Replace('+', '.');
            var testObjectTypeName = typeof(SomeType).FullName.Replace('+', '.');
            var component = CompileToComponent($"<c:{testComponentTypeName}" +
                $" IntProperty=123" +
                $" StringProperty=\"My string\"" +
                $" ObjectProperty=@(new {testObjectTypeName}()) />");
            var frames = GetRenderTree(component);

            // Assert
            // TODO: Fix this. 
            // * Currently the attribute names are lowercased if they were
            //   parsed by AngleSharp as HTML, and left in their original case if they
            //   were parsed by the Razor compiler as a C# expression. They should all
            //   retain their original case when the target element represents a component.
            // * Similarly, unquoted values are interpreted as strings if they were parsed
            //   by AngleSharp (e.g., intproperty=123 passes a string). The values should
            //   always be treated as C# expressions if the target represents a component.
            // This problem will probably go away on its own when we have new component
            // tooling.
            Assert.Collection(frames,
                frame => AssertFrame.Component<TestComponent>(frame, 4, 0),
                frame => AssertFrame.Attribute(frame, "intproperty", "123", 1),
                frame => AssertFrame.Attribute(frame, "stringproperty", "My string", 2),
                frame =>
                {
                    AssertFrame.Attribute(frame, "ObjectProperty");
                    Assert.IsType<SomeType>(frame.AttributeValue);
                });
        }

        [Fact]
        public void CanIncludeChildrenInComponents()
        {
            // Arrange/Act
            var testComponentTypeName = typeof(TestComponent).FullName.Replace('+', '.');
            var component = CompileToComponent($"<c:{testComponentTypeName} attr=\"abc\">" +
                $"Some text" +
                $"<some-child a='1'>Nested text</some-child>" +
                $"</c:{testComponentTypeName}>");
            var frames = GetRenderTree(component);

            Assert.Collection(frames,
                frame => AssertFrame.Component<TestComponent>(frame, 6, 0),
                frame => AssertFrame.Attribute(frame, "attr", "abc", 1),
                frame => AssertFrame.Text(frame, "Some text", 2),
                frame => AssertFrame.Element(frame, "some-child", 3, 3),
                frame => AssertFrame.Attribute(frame, "a", "1", 4),
                frame => AssertFrame.Text(frame, "Nested text", 5));
        }

        [Fact]
        public void ComponentsDoNotHaveLayoutAttributeByDefault()
        {
            // Arrange/Act
            var component = CompileToComponent($"Hello");

            // Assert
            Assert.Null(component.GetType().GetCustomAttribute<LayoutAttribute>());
        }

        [Fact]
        public void SupportsLayoutDeclarationsViaTemporarySyntax()
        {
            // Arrange/Act
            var testComponentTypeName = typeof(TestLayout).FullName.Replace('+', '.');
            var component = CompileToComponent(
                $"@(Layout<{testComponentTypeName}>())\n" +
                $"Hello");
            var frames = GetRenderTree(component);

            // Assert
            var layoutAttribute = component.GetType().GetCustomAttribute<LayoutAttribute>();
            Assert.NotNull(layoutAttribute);
            Assert.Equal(typeof(TestLayout), layoutAttribute.LayoutType);
            Assert.Collection(frames,
                frame => AssertFrame.Text(frame, "\nHello"));
        }

        [Fact]
        public void SupportsImplementsDeclarationsViaTemporarySyntax()
        {
            // Arrange/Act
            var testInterfaceTypeName = typeof(ITestInterface).FullName.Replace('+', '.');
            var component = CompileToComponent(
                $"@(Implements<{testInterfaceTypeName}>())\n" +
                $"Hello");
            var frames = GetRenderTree(component);

            // Assert
            Assert.IsAssignableFrom<ITestInterface>(component);
            Assert.Collection(frames,
                frame => AssertFrame.Text(frame, "\nHello"));
        }

        [Fact]
        public void SupportsInheritsDirective()
        {
            // Arrange/Act
            var testBaseClassTypeName = typeof(TestBaseClass).FullName.Replace('+', '.');
            var component = CompileToComponent(
                $"@inherits {testBaseClassTypeName}" + Environment.NewLine +
                $"Hello");
            var frames = GetRenderTree(component);

            // Assert
            Assert.IsAssignableFrom<TestBaseClass>(component);
            Assert.Collection(frames,
                frame => AssertFrame.Text(frame, "Hello"));
        }

        [Fact]
        public void SurfacesCSharpCompilationErrors()
        {
            // Arrange/Act
            var result = CompileToAssembly(
                GetArbitraryPlatformValidDirectoryPath(),
                "file.cshtml",
                "@invalidVar",
                "Test.Base");

            // Assert
            Assert.Collection(result.Diagnostics,
                diagnostic => Assert.Contains("'invalidVar'", diagnostic.GetMessage()));
        }

        [Fact]
        public void RejectsEndTagWithNoStartTag()
        {
            // Arrange/Act
            var result = CompileToCSharp(
                "Line1\nLine2\nLine3</mytag>");

            // Assert
            Assert.Collection(result.Diagnostics,
                item =>
                {
                    Assert.Equal(RazorCompilerDiagnostic.DiagnosticType.Error, item.Type);
                    Assert.StartsWith("Unexpected closing tag 'mytag' with no matching start tag.", item.Message);
                });
        }

        [Fact]
        public void RejectsEndTagWithDifferentNameToStartTag()
        {
            // Arrange/Act
            var result = CompileToCSharp(
                $"@{{\n" +
                $"   var abc = 123;\n" +
                $"}}\n" +
                $"<root>\n" +
                $"    <other />\n" +
                $"    text\n" +
                $"    <child>more text</root>\n" +
                $"</child>\n");

            // Assert
            Assert.Collection(result.Diagnostics,
                item =>
                {
                    Assert.Equal(RazorCompilerDiagnostic.DiagnosticType.Error, item.Type);
                    Assert.StartsWith("Mismatching closing tag. Found 'root' but expected 'child'.", item.Message);
                    Assert.Equal(7, item.Line);
                    Assert.Equal(21, item.Column);
                });
        }

        private static RenderTreeFrame[] GetRenderTree(IComponent component)
        {
            var renderer = new TestRenderer();
            renderer.AttachComponent(component);
            component.SetParameters(ParameterCollection.Empty);
            return renderer.LatestBatchReferenceFrames;
        }

        private static IComponent CompileToComponent(string cshtmlSource)
        {
            var testComponentTypeName = "TestComponent";
            var testComponentNamespace = "Test";
            var assemblyResult = CompileToAssembly(GetArbitraryPlatformValidDirectoryPath(), $"{testComponentTypeName}.cshtml", cshtmlSource, testComponentNamespace);
            Assert.Empty(assemblyResult.Diagnostics);
            var testComponentType = assemblyResult.Assembly.GetType($"{testComponentNamespace}.{testComponentTypeName}");
            return (IComponent)Activator.CreateInstance(testComponentType);
        }

        const string ArbitraryWindowsPath = "x:\\dir\\subdir";
        const string ArbitraryMacLinuxPath = "/dir/subdir";
        private static string GetArbitraryPlatformValidDirectoryPath()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ArbitraryWindowsPath : ArbitraryMacLinuxPath;

        private static CompileToAssemblyResult CompileToAssembly(string cshtmlRootPath, string cshtmlRelativePath, string cshtmlContent, string outputNamespace)
        {
            var csharpResult = CompileToCSharp(cshtmlRootPath, cshtmlRelativePath, cshtmlContent, outputNamespace);
            if (csharpResult.Diagnostics.Any())
            {
                var diagnosticsLog = string.Join(Environment.NewLine,
                    csharpResult.Diagnostics.Select(d => d.FormatForConsole()).ToArray());
                throw new InvalidOperationException($"Aborting compilation to assembly because RazorCompiler returned nonempty diagnostics: {diagnosticsLog}");
            }

            var syntaxTrees = new[]
            {
                CSharpSyntaxTree.ParseText(csharpResult.Code)
            };
            var referenceAssembliesContainingTypes = new[]
            {
                typeof(System.Runtime.AssemblyTargetedPatchBandAttribute), // System.Runtime
                typeof(BlazorComponent),
                typeof(RazorCompilerTest), // Reference this assembly, so that we can refer to test component types
            };
            var references = referenceAssembliesContainingTypes
                .SelectMany(type => type.Assembly.GetReferencedAssemblies().Concat(new[] { type.Assembly.GetName() }))
                .Distinct()
                .Select(Assembly.Load)
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .ToList();
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var assemblyName = "TestAssembly" + Guid.NewGuid().ToString("N");
            var compilation = CSharpCompilation.Create(assemblyName,
                syntaxTrees,
                references,
                options);

            using (var peStream = new MemoryStream())
            {
                compilation.Emit(peStream);

                var diagnostics = compilation
                    .GetDiagnostics()
                    .Where(d => d.Severity != DiagnosticSeverity.Hidden);
                return new CompileToAssemblyResult
                {
                    Diagnostics = diagnostics,
                    VerboseLog = csharpResult.VerboseLog,
                    Assembly = diagnostics.Any() ? null : Assembly.Load(peStream.ToArray())
                };
            }
        }

        private static CompileToCSharpResult CompileToCSharp(string cshtmlContent)
            => CompileToCSharp(
                GetArbitraryPlatformValidDirectoryPath(),
                "test.cshtml",
                cshtmlContent,
                "TestNamespace");

        private static CompileToCSharpResult CompileToCSharp(string cshtmlRootPath, string cshtmlRelativePath, string cshtmlContent, string outputNamespace)
        {
            using (var resultStream = new MemoryStream())
            using (var resultWriter = new StreamWriter(resultStream))
            using (var verboseLogStream = new MemoryStream())
            using (var verboseWriter = new StreamWriter(verboseLogStream))
            using (var inputContents = new MemoryStream(Encoding.UTF8.GetBytes(cshtmlContent)))
            {
                var diagnostics = new RazorCompiler().CompileSingleFile(
                    cshtmlRootPath,
                    cshtmlRelativePath,
                    inputContents,
                    outputNamespace,
                    resultWriter,
                    verboseWriter);

                resultWriter.Flush();
                verboseWriter.Flush();
                return new CompileToCSharpResult
                {
                    Code = Encoding.UTF8.GetString(resultStream.ToArray()),
                    VerboseLog = Encoding.UTF8.GetString(verboseLogStream.ToArray()),
                    Diagnostics = diagnostics
                };
            }
        }

        private class CompileToCSharpResult
        {
            public string Code { get; set; }
            public string VerboseLog { get; set; }
            public IEnumerable<RazorCompilerDiagnostic> Diagnostics { get; set; }
        }

        private class CompileToAssemblyResult
        {
            public Assembly Assembly { get; set; }
            public string VerboseLog { get; set; }
            public IEnumerable<Diagnostic> Diagnostics { get; set; }
        }

        private class TestRenderer : Renderer
        {
            public RenderTreeFrame[] LatestBatchReferenceFrames { get; private set; }

            public void AttachComponent(IComponent component)
                => AssignComponentId(component);

            protected override void UpdateDisplay(RenderBatch renderBatch)
            {
                LatestBatchReferenceFrames = renderBatch.ReferenceFrames.ToArray();
            }
        }

        public class TestComponent : IComponent
        {
            public void Init(RenderHandle renderHandle)
            {
            }

            public void SetParameters(ParameterCollection parameters)
            {
            }
        }

        public class TestLayout : ILayoutComponent
        {
            public RenderFragment Body { get; set; }

            public void Init(RenderHandle renderHandle)
            {
            }

            public void SetParameters(ParameterCollection parameters)
            {
            }
        }

        public interface ITestInterface { }

        public class TestBaseClass : BlazorComponent { }

        public class SomeType { }
    }
}

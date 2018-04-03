﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Xunit;

namespace Microsoft.AspNetCore.Blazor.Build.Test
{
    public class DeclarationRazorIntegrationTest : RazorIntegrationTestBase
    {
        internal override RazorConfiguration Configuration => BlazorExtensionInitializer.DeclarationConfiguration;

        [Fact]
        public void DeclarationConfiguration_IncludesFunctions()
        {
            // Arrange & Act
            var component = CompileToComponent(@"
@functions {
    public string Value { get; set; }
}");

            // Assert
            var property = component.GetType().GetProperty("Value");
            Assert.NotNull(property);
            Assert.Same(typeof(string), property.PropertyType);
        }

        [Fact]
        public void DeclarationConfiguration_IncludesInject()
        {
            // Arrange & Act
            var component = CompileToComponent(@"
@inject string Value
");

            // Assert
            var property = component.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(property);
            Assert.Same(typeof(string), property.PropertyType);
        }

        [Fact]
        public void DeclarationConfiguration_IncludesUsings()
        {
            // Arrange & Act
            var component = CompileToComponent(@"
@using System.Text
@inject StringBuilder Value
");

            // Assert
            var property = component.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(property);
            Assert.Same(typeof(StringBuilder), property.PropertyType);
        }

        [Fact]
        public void DeclarationConfiguration_IncludesInherits()
        {
            // Arrange & Act
            var component = CompileToComponent($@"
@inherits {FullTypeName<BaseClass>()}
");

            // Assert
            Assert.Same(typeof(BaseClass), component.GetType().BaseType);
        }

        [Fact]
        public void DeclarationConfiguration_IncludesImplements()
        {
            // Arrange & Act
            var component = CompileToComponent($@"
@implements {FullTypeName<IDoCoolThings>()}
");

            // Assert
            var type = component.GetType();
            Assert.Contains(typeof(IDoCoolThings), component.GetType().GetInterfaces());
        }

        [Fact]
        public void DeclarationConfiguration_RenderMethodIsEmpty()
        {
            // Arrange & Act
            var component = CompileToComponent(@"
<html>
@{ var message = ""hi""; }
<span class=""@(5 + 7)"">@message</span>
</html>
");

            var frames = GetRenderTree(component);

            // Assert
            Assert.Empty(frames);
        }

        [Fact] // Regression test for https://github.com/aspnet/Blazor/issues/453
        public void DeclarationConfiguration_FunctionsBlockHasLineMappings_MappingsApplyToError()
        {
            // Arrange & Act 1
            var generated = CompileToCSharp(@"
@functions {
    public StringBuilder Builder { get; set; }
}
");

            // Assert 1
            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
        }
        #pragma warning restore 1998
#line 1 ""x:\dir\subdir\Test\TestComponent.cshtml""
            
    public StringBuilder Builder { get; set; }

#line default
#line hidden
    }
}
#pragma warning restore 1591
", generated);

            // Act 2
            var assembly = CompileToAssembly(generated, throwOnFailure: false);

            // Assert 2
            var diagnostic = Assert.Single(assembly.Diagnostics);

            // This error should map to line 2 of the generated file, the test
            // says 1 because Roslyn's line/column data structures are 0-based.
            var position = diagnostic.Location.GetMappedLineSpan();
            Assert.EndsWith(".cshtml", position.Path);
            Assert.Equal(1, position.StartLinePosition.Line);
        }

        public class BaseClass : BlazorComponent
        {
        }

        public interface IDoCoolThings
        {
        }
    }
}

﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Microsoft.AspNetCore.Blazor.Build.Test
{
    public class RuntimeCodeGenerationRazorIntegrationTest : RazorIntegrationTestBase
    {
        internal override bool UseTwoPhaseCompilation => true;

        [Fact]
        public void CodeGeneration_ChildComponent_Simple()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
<MyComponent />");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
", generated);
        }

        [Fact]
        public void CodeGeneration_ChildComponent_WithParameters()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class SomeType
    {
    }

    public class MyComponent : BlazorComponent
    {
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
        public SomeType ObjectProperty { get; set; }
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
<MyComponent 
    IntProperty=""123""
    BoolProperty=""true""
    StringProperty=""My string""
    ObjectProperty=""new SomeType()""/>");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, ""IntProperty"", 123);
            builder.AddAttribute(2, ""BoolProperty"", true);
            builder.AddAttribute(3, ""StringProperty"", ""My string"");
            builder.AddAttribute(4, ""ObjectProperty"", new SomeType());
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
", generated);
        }

        [Fact]
        public void CodeGeneration_ChildComponent_WithExplicitStringParameter()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
        public string StringProperty { get; set; }
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
<MyComponent StringProperty=""@(42.ToString())"" />");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, ""StringProperty"", 42.ToString());
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
", generated);
        }

        [Fact]
        public void CodeGeneration_ChildComponent_WithNonPropertyAttributes()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
<MyComponent some-attribute=""foo"" another-attribute=""@(43.ToString())""/>");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, ""some-attribute"", ""foo"");
            builder.AddAttribute(2, ""another-attribute"", 43.ToString());
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
", generated);
        }


        [Fact]
        public void CodeGeneration_ChildComponent_WithLambdaEventHandler()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using System;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
        public UIEventHandler OnClick { get; set; }
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
<MyComponent OnClick=""@(e => { Increment(); })""/>

@functions {
    private int counter;
    private void Increment() {
        counter++;
    }
}");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, ""OnClick"", new Microsoft.AspNetCore.Blazor.UIEventHandler(e => { Increment(); }));
            builder.CloseComponent();
            builder.AddContent(2, ""\n\n"");
        }
        #pragma warning restore 1998
#line 4 ""x:\dir\subdir\Test\TestComponent.cshtml""
            
    private int counter;
    private void Increment() {
        counter++;
    }

#line default
#line hidden
    }
}
#pragma warning restore 1591

", generated);
        }

        [Fact]
        public void CodeGeneration_ChildComponent_WithExplicitEventHandler()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using System;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
        public UIEventHandler OnClick { get; set; }
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
@using Microsoft.AspNetCore.Blazor
<MyComponent OnClick=""@Increment""/>

@functions {
    private int counter;
    private void Increment(UIEventArgs e) {
        counter++;
    }
}");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Blazor;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, ""OnClick"", new Microsoft.AspNetCore.Blazor.UIEventHandler(Increment));
            builder.CloseComponent();
            builder.AddContent(2, ""\n\n"");
        }
        #pragma warning restore 1998
#line 5 ""x:\dir\subdir\Test\TestComponent.cshtml""
            
    private int counter;
    private void Increment(UIEventArgs e) {
        counter++;
    }

#line default
#line hidden
    }
}
#pragma warning restore 1591

", generated);
        }

        [Fact]
        public void CodeGeneration_ChildComponent_WithChildContent()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
        public string MyAttr { get; set; }

        public RenderFragment ChildContent { get; set; }
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
<MyComponent MyAttr=""abc"">Some text<some-child a='1'>Nested text</some-child></MyComponent>");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, ""MyAttr"", ""abc"");
            builder.AddAttribute(2, ""ChildContent"", (Microsoft.AspNetCore.Blazor.RenderFragment)((builder2) => {
                builder2.AddContent(3, ""Some text"");
                builder2.OpenElement(4, ""some-child"");
                builder2.AddAttribute(5, ""a"", ""1"");
                builder2.AddContent(6, ""Nested text"");
                builder2.CloseElement();
            }
            ));
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
", generated);
        }

        [Fact]
        public void CodeGeneration_ChildComponent_WithPageDirective()
        {
            // Arrange
            AdditionalSyntaxTrees.Add(CSharpSyntaxTree.ParseText(@"
using Microsoft.AspNetCore.Blazor.Components;

namespace Test
{
    public class MyComponent : BlazorComponent
    {
    }
}
"));

            // Act
            var generated = CompileToCSharp(@"
@addTagHelper *, TestAssembly
@page ""/MyPage""
@page ""/AnotherRoute/{id}""
<MyComponent />");

            // Assert
            CompileToAssembly(generated);

            AssertSourceEquals(@"
// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    [Microsoft.AspNetCore.Blazor.Components.RouteAttribute(""/MyPage"")]
    [Microsoft.AspNetCore.Blazor.Components.RouteAttribute(""/AnotherRoute/{id}"")]
    public partial class TestComponent : Microsoft.AspNetCore.Blazor.Components.BlazorComponent
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Blazor.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<Test.MyComponent>(0);
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
", generated);
        }
    }
}

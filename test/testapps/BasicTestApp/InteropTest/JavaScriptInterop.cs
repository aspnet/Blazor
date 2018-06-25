// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BasicTestApp.InteropTest
{
    public class JavaScriptInterop
    {
        public static IDictionary<string, object[]> Invocations = new Dictionary<string, object[]>();

        [JSInvokable(nameof(ThrowException))]
        public static void ThrowException() => throw new InvalidOperationException("Threw an exception!");

        [JSInvokable(nameof(AsyncThrowSyncException))]
        public static Task AsyncThrowSyncException()
            => throw new InvalidOperationException("Threw a sync exception!");

        [JSInvokable(nameof(AsyncThrowAsyncException))]
        public static async Task AsyncThrowAsyncException()
        {
            await Task.Yield();
            throw new InvalidOperationException("Threw an async exception!");
        }

        [JSInvokable(nameof(VoidParameterless))]
        public static void VoidParameterless()
        {
            Invocations[nameof(VoidParameterless)] = new object[0];
        }

        [JSInvokable(nameof(VoidWithOneParameter))]
        public static void VoidWithOneParameter(ComplexParameter parameter1)
        {
            Invocations[nameof(VoidWithOneParameter)] = new object[] { parameter1 };
        }

        [JSInvokable(nameof(VoidWithTwoParameters))]
        public static void VoidWithTwoParameters(
            ComplexParameter parameter1,
            byte parameter2)
        {
            Invocations[nameof(VoidWithTwoParameters)] = new object[] { parameter1, parameter2 };
        }

        [JSInvokable(nameof(VoidWithThreeParameters))]
        public static void VoidWithThreeParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3)
        {
            Invocations[nameof(VoidWithThreeParameters)] = new object[] { parameter1, parameter2, parameter3 };
        }

        [JSInvokable(nameof(VoidWithFourParameters))]
        public static void VoidWithFourParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4)
        {
            Invocations[nameof(VoidWithFourParameters)] = new object[] { parameter1, parameter2, parameter3, parameter4 };
        }

        [JSInvokable(nameof(VoidWithFiveParameters))]
        public static void VoidWithFiveParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5)
        {
            Invocations[nameof(VoidWithFiveParameters)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5 };
        }

        [JSInvokable(nameof(VoidWithSixParameters))]
        public static void VoidWithSixParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6)
        {
            Invocations[nameof(VoidWithSixParameters)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6 };
        }

        [JSInvokable(nameof(VoidWithSevenParameters))]
        public static void VoidWithSevenParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7)
        {
            Invocations[nameof(VoidWithSevenParameters)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7 };
        }

        [JSInvokable(nameof(VoidWithEightParameters))]
        public static void VoidWithEightParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7,
            Segment parameter8)
        {
            Invocations[nameof(VoidWithEightParameters)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8 };
        }

        [JSInvokable(nameof(ReturnArray))]
        public static decimal[] ReturnArray()
        {
            return new decimal[] { 0.1M, 0.2M };
        }

        [JSInvokable(nameof(EchoOneParameter))]
        public static object[] EchoOneParameter(ComplexParameter parameter1)
        {
            return new object[] { parameter1 };
        }

        [JSInvokable(nameof(EchoTwoParameters))]
        public static object[] EchoTwoParameters(
            ComplexParameter parameter1,
            byte parameter2)
        {
            return new object[] { parameter1, parameter2 };
        }

        [JSInvokable(nameof(EchoThreeParameters))]
        public static object[] EchoThreeParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3)
        {
            return new object[] { parameter1, parameter2, parameter3 };
        }

        [JSInvokable(nameof(EchoFourParameters))]
        public static object[] EchoFourParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4)
        {
            return new object[] { parameter1, parameter2, parameter3, parameter4 };
        }

        [JSInvokable(nameof(EchoFiveParameters))]
        public static object[] EchoFiveParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5)
        {
            return new object[] { parameter1, parameter2, parameter3, parameter4, parameter5 };
        }

        [JSInvokable(nameof(EchoSixParameters))]
        public static object[] EchoSixParameters(ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6)
        {
            return new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6 };
        }

        [JSInvokable(nameof(EchoSevenParameters))]
        public static object[] EchoSevenParameters(ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7)
        {
            return new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7 };
        }

        [JSInvokable(nameof(EchoEightParameters))]
        public static object[] EchoEightParameters(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7,
            Segment parameter8)
        {
            return new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8 };
        }

        [JSInvokable(nameof(VoidParameterlessAsync))]
        public static Task VoidParameterlessAsync()
        {
            Invocations[nameof(VoidParameterlessAsync)] = new object[0];
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithOneParameterAsync))]
        public static Task VoidWithOneParameterAsync(ComplexParameter parameter1)
        {
            Invocations[nameof(VoidWithOneParameterAsync)] = new object[] { parameter1 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithTwoParametersAsync))]
        public static Task VoidWithTwoParametersAsync(
            ComplexParameter parameter1,
            byte parameter2)
        {
            Invocations[nameof(VoidWithTwoParametersAsync)] = new object[] { parameter1, parameter2 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithThreeParametersAsync))]
        public static Task VoidWithThreeParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3)
        {
            Invocations[nameof(VoidWithThreeParametersAsync)] = new object[] { parameter1, parameter2, parameter3 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithFourParametersAsync))]
        public static Task VoidWithFourParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4)
        {
            Invocations[nameof(VoidWithFourParametersAsync)] = new object[] { parameter1, parameter2, parameter3, parameter4 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithFiveParametersAsync))]
        public static Task VoidWithFiveParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5)
        {
            Invocations[nameof(VoidWithFiveParametersAsync)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithSixParametersAsync))]
        public static Task VoidWithSixParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6)
        {
            Invocations[nameof(VoidWithSixParametersAsync)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithSevenParametersAsync))]
        public static Task VoidWithSevenParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7)
        {
            Invocations[nameof(VoidWithSevenParametersAsync)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(VoidWithEightParametersAsync))]
        public static Task VoidWithEightParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7,
            Segment parameter8)
        {
            Invocations[nameof(VoidWithEightParametersAsync)] = new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8 };
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(ReturnArrayAsync))]
        public static Task<decimal[]> ReturnArrayAsync()
        {
            return Task.FromResult(new decimal[] { 0.1M, 0.2M });
        }

        [JSInvokable(nameof(EchoOneParameterAsync))]
        public static Task<object[]> EchoOneParameterAsync(ComplexParameter parameter1)
        {
            return Task.FromResult(new object[] { parameter1 });
        }

        [JSInvokable(nameof(EchoTwoParametersAsync))]
        public static Task<object[]> EchoTwoParametersAsync(
            ComplexParameter parameter1,
            byte parameter2)
        {
            return Task.FromResult(new object[] { parameter1, parameter2 });
        }

        [JSInvokable(nameof(EchoThreeParametersAsync))]
        public static Task<object[]> EchoThreeParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3)
        {
            return Task.FromResult(new object[] { parameter1, parameter2, parameter3 });
        }

        [JSInvokable(nameof(EchoFourParametersAsync))]
        public static Task<object[]> EchoFourParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4)
        {
            return Task.FromResult(new object[] { parameter1, parameter2, parameter3, parameter4 });
        }

        [JSInvokable(nameof(EchoFiveParametersAsync))]
        public static Task<object[]> EchoFiveParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5)
        {
            return Task.FromResult(new object[] { parameter1, parameter2, parameter3, parameter4, parameter5 });
        }

        [JSInvokable(nameof(EchoSixParametersAsync))]
        public static Task<object[]> EchoSixParametersAsync(ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6)
        {
            return Task.FromResult(new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6 });
        }

        [JSInvokable(nameof(EchoSevenParametersAsync))]
        public static Task<object[]> EchoSevenParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7)
        {
            return Task.FromResult(new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7 });
        }

        [JSInvokable(nameof(EchoEightParametersAsync))]
        public static Task<object[]> EchoEightParametersAsync(
            ComplexParameter parameter1,
            byte parameter2,
            short parameter3,
            int parameter4,
            long parameter5,
            float parameter6,
            List<double> parameter7,
            Segment parameter8)
        {
            return Task.FromResult(new object[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8 });
        }
    }
}

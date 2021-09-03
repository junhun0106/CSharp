﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    internal class MemoryPoolThrowHelper
    {
        public static void ThrowArgumentOutOfRangeException(int sourceLength, int offset)
        {
            throw GetArgumentOutOfRangeException(sourceLength, offset);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(int sourceLength, int offset)
        {
            if ((uint)offset > (uint)sourceLength)
            {
                // Offset is negative or less than array length
                return new ArgumentOutOfRangeException(GetArgumentName(ExceptionArgument.offset));
            }

            // The third parameter (not passed) length must be out of range
            return new ArgumentOutOfRangeException(GetArgumentName(ExceptionArgument.length));
        }

        public static void ThrowInvalidOperationException_DoubleDispose()
        {
            throw new InvalidOperationException("Object is being disposed twice");
        }

        public static void ThrowArgumentOutOfRangeException_BufferRequestTooLarge(int maxSize)
        {
            throw GetArgumentOutOfRangeException_BufferRequestTooLarge(maxSize);
        }

        public static void ThrowObjectDisposedException(ExceptionArgument argument)
        {
            throw GetObjectDisposedException(argument);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException_BufferRequestTooLarge(int maxSize)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(ExceptionArgument.size), $"Cannot allocate more than {maxSize} bytes in a single buffer");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ObjectDisposedException GetObjectDisposedException(ExceptionArgument argument)
        {
            return new ObjectDisposedException(GetArgumentName(argument));
        }

        private static string GetArgumentName(ExceptionArgument argument)
        {
            Debug.Assert(Enum.IsDefined(typeof(ExceptionArgument), argument), "The enum value is not defined, please check the ExceptionArgument Enum.");

            return argument.ToString();
        }

        internal enum ExceptionArgument
        {
            size,
            offset,
            length,
            MemoryPoolBlock,
            MemoryPool
        }
    }
}

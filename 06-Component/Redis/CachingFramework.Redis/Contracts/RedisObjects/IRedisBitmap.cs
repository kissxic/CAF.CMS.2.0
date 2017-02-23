﻿using System;
using System.Collections.Generic;
using Bit = System.Byte;

namespace CachingFramework.Redis.Contracts.RedisObjects
{
    /// <summary>
    /// Managed collection of bools using Redis bitmaps
    /// </summary>
    public interface IRedisBitmap : ICollection<byte>, IRedisObject
    {
        /// <summary>
        /// Sets or clears the bit at offset. The bit is either set or cleared depending on bit parameter.
        /// When key does not exist, a new string value is created. The string is grown to make sure it can hold a bit at offset.
        /// </summary>
        /// <param name="offset">The zero-based offset.</param>
        /// <param name="bit">The bit value (any number > 0 is considered as true).</param>
        void SetBit(long offset, Bit bit);
        /// <summary>
        /// Returns the bit value at offset in the string value stored at key. 
        /// When offset is beyond the string length, 0 is returned.
        /// </summary>
        /// <param name="offset">The zero-based offset.</param>
        Bit GetBit(long offset);
        /// <summary>
        /// Return the position of the first bit set to 1 or 0.  
        /// An start and end may be specified; these are in bytes, not bits; start and end can contain negative 
        /// values in order to index bytes starting from the end of the string.
        /// </summary>
        /// <param name="bit">The bit to search</param>
        /// <param name="start">The start position (in bytes)</param>
        /// <param name="stop">The end position (in bytes)</param>
        long BitPosition(Bit bit, long start = 0, long stop = -1);
        /// <summary>
        /// Count the number of set bits (population counting) in the given byte range.  
        /// It is possible to specify the counting operation only in an interval passing the additional arguments start and end.  
        /// </summary>
        /// <param name="start">The start position (in bytes)</param>
        /// <param name="stop">The end position (in bytes)</param>
        new long Count(long start = 0, long stop = -1);
        /// <summary>
        /// Appends an *entire byte* (8 bits) to the bitmap set to the given value.
        /// </summary>
        new void Add(byte value);
        /// <summary>
        /// Determines whether the bitmap contains the given bit within the byte(s) specified on the start/stop range.
        /// </summary>
        /// <param name="bit">The bit to check (any number > 0 is considered as true)</param>
        /// <param name="start">The start position (in bytes)</param>
        /// <param name="stop">The end position (in bytes)</param>
        bool Contains(Bit bit, long start, long stop = -1);
        /// <summary>
        /// Inverts the first occurence of the specified bit in the bitmap.
        /// </summary>
        new bool Remove(Bit bit);
        /// <summary>
        /// Gets the specified integer field in the bitmap
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="offset">The offset (bit or ordinal).</param>
        /// <param name="offsetIsOrdinal">if set to <c>true</c>, offset is ordinal, so offset=N means the N-th counter of the fieldType size.
        /// If set to <c>false</c>, offset is the bit position, so offset=N means the N-th bit</param>
        T BitfieldGet<T>(BitfieldType fieldType, long offset, bool offsetIsOrdinal = false)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable;
        /// <summary>
        /// Sets the specified integer field in the bitmap
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="offset">The offset (bit or ordinal).</param>
        /// <param name="value">The value to set.</param>
        /// <param name="offsetIsOrdinal">if set to <c>true</c>, offset is ordinal, so offset=N means the N-th counter of the fieldType size.
        /// If set to <c>false</c>, offset is the bit position, so offset=N means the N-th bit</param>
        /// <param name="overflowType">Overflow handling type.</param>
        /// <returns>The previous value.</returns>
        T BitfieldSet<T>(BitfieldType fieldType, long offset, T value, bool offsetIsOrdinal = false,
            OverflowType overflowType = OverflowType.Wrap)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable;
        /// <summary>
        /// Increment the specified integer counter
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="offset">The offset (bit or ordinal).</param>
        /// <param name="increment">The value to increment.</param>
        /// <param name="offsetIsOrdinal">if set to <c>true</c>, offset is ordinal, so offset=N means the N-th counter of the fieldType size.
        /// If set to <c>false</c>, offset is the bit position, so offset=N means the N-th bit</param>
        /// <param name="overflowType">Overflow handling.</param>
        /// <returns>The previous value.</returns>
        T BitfieldIncrementBy<T>(BitfieldType fieldType, long offset, T increment, bool offsetIsOrdinal = false,
            OverflowType overflowType = OverflowType.Wrap)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable;
    }
}

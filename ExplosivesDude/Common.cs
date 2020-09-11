/// <summary>
/// 
/// </summary>
namespace ExplosivesDude
{
    using System;

    /// <summary>
    /// Provides common and useful functions.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Appends an integer to an existing byte array.
        /// </summary>
        /// <param name="origin">The original byte array.</param>
        /// <param name="add">The integer value to be appended.</param>
        /// <returns>The new byte array including the integer at the end.</returns>
        public static byte[] AppendInt(byte[] origin, int add)
        {
            byte[] intData = BitConverter.GetBytes(add);
            byte[] data = new byte[origin.Length + intData.Length];
            Array.Copy(origin, data, origin.Length);
            Array.Copy(intData, 0, data, origin.Length, intData.Length);
            return data;
        }

        /// <summary>
        /// Concatenates two byte arrays.
        /// </summary>
        /// <param name="one">The first byte array.</param>
        /// <param name="two">The second byte array.</param>
        /// <returns>The combined byte array.</returns>
        public static byte[] Concatenate(byte[] one, byte[] two)
        {
            byte[] three = new byte[one.Length + two.Length];
            Array.Copy(one, 0, three, 0, one.Length);
            Array.Copy(two, 0, three, one.Length, two.Length);
            return three;
        }
    }
}

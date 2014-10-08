using System;
using System.Text;

namespace Swarmops.Logic.Support
{
    /// <summary>
    /// This class implements the Verhoeff check digit scheme.
    /// This is one of the best available check digit algorithms
    /// that works with any length input.
    /// See:    http://www.cs.utsa.edu/~wagner/laws/verhoeff.html
    ///         http://www.augustana.ca/~mohrj/algorithms/checkdigit.html
    ///         http://modp.com/release/checkdigits/
    /// </summary>
    public sealed class CheckDigit
    {
        #region Private Static Variables
        //-----------------------------------------------------------------------------------------------
        private static CheckDigit _instance = null;
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Private Instance Variables
        //-----------------------------------------------------------------------------------------------
        private int[][] op = new int[10][];
        private int[] inv = { 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 };
        private int[][] F = new int[8][];
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Private Constructor
        //-----------------------------------------------------------------------------------------------
        private CheckDigit()
        {
            op[0] = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            op[1] = new int[] {1, 2, 3, 4, 0, 6, 7, 8, 9, 5};
            op[2] = new int[] {2, 3, 4, 0, 1, 7, 8, 9, 5, 6};
            op[3] = new int[] {3, 4, 0, 1, 2, 8, 9, 5, 6, 7};
            op[4] = new int[] {4, 0, 1, 2, 3, 9, 5, 6, 7, 8};
            op[5] = new int[] {5, 9, 8, 7, 6, 0, 4, 3, 2 ,1};
            op[6] = new int[] {6, 5, 9, 8, 7, 1, 0, 4, 3, 2};
            op[7] = new int[] {7, 6, 5, 9, 8, 2, 1, 0, 4, 3};
            op[8] = new int[] {8, 7, 6, 5, 9, 3, 2, 1, 0, 4};
            op[9] = new int[] {9, 8, 7, 6, 5, 4, 3, 2, 1, 0};

            F[0] = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };  // identity permutation
            F[1] = new int[]{ 1, 5, 7, 6, 2, 8, 3, 0, 9, 4 };  // "magic" permutation
            for (int i = 2; i < 8; i++)
            {
                // iterate for remaining permutations
                F[i] = new int[10];
                for (int j = 0; j < 10; j++)
                    F[i][j] = F[i - 1][F[1][j]];
            }
        }
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Public Static Methods
        //-----------------------------------------------------------------------------------------------

        #region AppendCheckDigit Method
        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the Verhoeff check digit for the given input, then returns
        /// the input with the check digit appended at the end.
        /// </summary>
        /// <param name="input">The string for which the check digit is to be calculated.</param>
        /// <returns>The input with the calculated check digit appended.</returns>
        public static string AppendCheckDigit(string input)
        {
            int[] resultArray = Instance._AppendCheckDigit(_ConvertToIntArray(input));

            StringBuilder resultString = new StringBuilder();

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultString.Append(resultArray[i]);
            }

            return resultString.ToString();
        }

        /// <summary>
        /// Calculates the Verhoeff check digit for the given input, then returns
        /// the input with the check digit appended at the end.
        /// </summary>
        /// <param name="input">The long integer for which the check digit is to be calculated.</param>
        /// <returns>The input with the calculated check digit appended.</returns>
        public static long AppendCheckDigit(long input)
        {
            int[] resultArray = Instance._AppendCheckDigit(_ConvertToIntArray(input));
            return _ConvertToLong(resultArray);
        }

        /// <summary>
        /// Calculates the Verhoeff check digit for the given input, then returns
        /// the input with the check digit appended at the end.
        /// </summary>
        /// <param name="input">The integer for which the check digit is to be calculated.</param>
        /// <returns>The input with the calculated check digit appended.</returns>
        public static int AppendCheckDigit(int input)
        {
            int[] resultArray = Instance._AppendCheckDigit(_ConvertToIntArray(input));
            long resultLong = _ConvertToLong(resultArray);
            return (int)resultLong;
        }

        /// <summary>
        /// Calculates the Verhoeff check digit for the given input, then returns
        /// the input with the check digit appended at the end.
        /// </summary>
        /// <param name="input">The integer array for which the check digit is to be calculated.</param>
        /// <returns>The input with the calculated check digit appended.</returns>
        public static int[] AppendCheckDigit(int[] input)
        {
            return Instance._AppendCheckDigit(input);
        }
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region CalculateCheckDigit  Method
        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the Verhoeff check digit for the given input.
        /// </summary>
        /// <param name="input">The string for which the check digit is to be calculated.</param>
        /// <returns>The check digit for the input.</returns>
        public static int CalculateCheckDigit(string input)
        {
            return Instance._CalculateCheckDigit(_ConvertToIntArray(input));
        }

        /// <summary>
        /// Calculates the Verhoeff check digit for the given input.
        /// </summary>
        /// <param name="input">The long integer for which the check digit is to be calculated.</param>
        /// <returns>The check digit for the input.</returns>
        public static int CalculateCheckDigit(long input)
        {
            return Instance._CalculateCheckDigit(_ConvertToIntArray(input));
        }

        /// <summary>
        /// Calculates the Verhoeff check digit for the given input.
        /// </summary>
        /// <param name="input">The integer for which the check digit is to be calculated.</param>
        /// <returns>The check digit for the input.</returns>
        public static int CalculateCheckDigit(int input)
        {
            return Instance._CalculateCheckDigit(_ConvertToIntArray(input));
        }

        /// <summary>
        /// Calculates the Verhoeff check digit for the given input.
        /// </summary>
        /// <param name="input">The integer array for which the check digit is to be calculated.</param>
        /// <returns>The check digit for the input.</returns>
        public static int CalculateCheckDigit(int[] input)
        {
            return Instance._CalculateCheckDigit(input);
        }
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Check  Method
        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Verifies that a given string has a valid Verhoeff check digit as the last digit.
        /// </summary>
        /// <param name="input">The string for which the check digit is to be checked. The check digit is the last digit in the string.</param>
        /// <returns>Returns true if the last digit of the input is the valid check digit for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(string input)
        {
            return Instance._Check(_ConvertToIntArray(input));
        }

        /// <summary>
        /// Verifies that a given long integer has a valid Verhoeff check digit as the last digit.
        /// </summary>
        /// <param name="input">The long integer for which the check digit is to be checked. The check digit is the last digit in the input.</param>
        /// <returns>Returns true if the last digit of the input is the valid check digit for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(long input)
        {
            return Instance._Check(_ConvertToIntArray(input));
        }

        /// <summary>
        /// Verifies that a given integer has a valid Verhoeff check digit as the last digit.
        /// </summary>
        /// <param name="input">The integer for which the check digit is to be checked. The check digit is the last digit in the input.</param>
        /// <returns>Returns true if the last digit of the input is the valid check digit for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(int input)
        {
            return Instance._Check(_ConvertToIntArray(input));
        }

        /// <summary>
        /// Verifies that a given integer array has a valid Verhoeff check digit as the last digit
        /// in the array.
        /// </summary>
        /// <param name="input">The integer array for which the check digit is to be checked. The check digit is the last element of the array.</param>
        /// <returns>Returns true if the last digit of the input is the valid check digit for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(int[] input)
        {
            return Instance._Check(input);
        }

        /// <summary>
        /// Verifies the Verhoeff check digit for a given string.
        /// </summary>
        /// <param name="input">The string for which the check digit is to be verified. The input 
        /// does not include the check digit.</param>
        /// <param name="checkDigit">The check digit to be verified.</param>
        /// <returns>Returns true if the check digit is valid for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(string input, int checkDigit)
        {
            return Instance._Check(_ConvertToIntArray(input), checkDigit);
        }

        /// <summary>
        /// Verifies the Verhoeff check digit for a given long integer.
        /// </summary>
        /// <param name="input">The long integer for which the check digit is to be verified. The input 
        /// does not include the check digit.</param>
        /// <param name="checkDigit">The check digit to be verified.</param>
        /// <returns>Returns true if the check digit is valid for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(long input, int checkDigit)
        {
            return Instance._Check(_ConvertToIntArray(input), checkDigit);
        }

        /// <summary>
        /// Verifies the Verhoeff check digit for a given integer.
        /// </summary>
        /// <param name="input">The integer for which the check digit is to be verified. The input 
        /// does not include the check digit.</param>
        /// <param name="checkDigit">The check digit to be verified.</param>
        /// <returns>Returns true if the check digit is valid for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(int input, int checkDigit)
        {
            return Instance._Check(_ConvertToIntArray(input), checkDigit);
        }

        /// <summary>
        /// Verifies the Verhoeff check digit for a given integer array.
        /// </summary>
        /// <param name="input">The integer array for which the check digit is to be verified. The input 
        /// does not include the check digit.</param>
        /// <param name="checkDigit">The check digit to be verified.</param>
        /// <returns>Returns true if the check digit is valid for
        /// the input. Otherwise returns false.</returns>
        public static bool Check(int[] input, int checkDigit)
        {
            return Instance._Check(input, checkDigit);
        }
        //-----------------------------------------------------------------------------------------------
        #endregion

        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Private Static Properties
        //-----------------------------------------------------------------------------------------------
        private static CheckDigit Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CheckDigit();
                return _instance;
            }
        }
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Private Static Methods
        //-----------------------------------------------------------------------------------------------
        private static int[] _ConvertToIntArray(string input)
        {
            int[] inputArray = new int[input.Length];

            for (int i = 0; i < input.Length; i++)
                inputArray[i] = Convert.ToInt32(input.Substring(i, 1));

            return inputArray;
        }

        private static int[] _ConvertToIntArray(long input)
        {
            return _ConvertToIntArray(input.ToString());
        }

        private static int[] _ConvertToIntArray(int input)
        {
            return _ConvertToIntArray(input.ToString());
        }

        private static long _ConvertToLong(int[] input)
        {
            long result = 0;
            long power = 1;

            for (int i = 0; i < input.Length; i++)
            {
                result += input[input.Length - (i + 1)] * power;
                power *= 10;
            }

            return result;
        }
        //-----------------------------------------------------------------------------------------------
        #endregion

        #region Private Instance Methods
        //-----------------------------------------------------------------------------------------------
        private int[] _AppendCheckDigit(int[] input)
        {
            int checkDigit = _CalculateCheckDigit(input);
            int[] result = new int[input.Length + 1];
            input.CopyTo(result, 0);
            result[result.Length - 1] = checkDigit;

            return result;
        }

        private int _CalculateCheckDigit(int[] input)
        {
            // First we need to reverse the order of the input digits
            int[] reversedInput = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
                reversedInput[i] = input[input.Length - (i + 1)];

            int check = 0;
            for (int i = 0; i < reversedInput.Length; i++)
                check = op[check][F[(i + 1) % 8][reversedInput[i]]];
            int checkDigit = inv[check];

            return checkDigit;
        }

        private bool _Check(int[] input)
        {
            // First we need to reverse the order of the input digits
            int[] reversedInput = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
                reversedInput[i] = input[input.Length - (i + 1)];

            int check = 0;
            for (int i = 0; i < reversedInput.Length; i++)
                check = op[check][F[i % 8][reversedInput[i]]];
            
            return (check == 0);
        }

        private bool _Check(int[] input, int checkDigit)
        {
            int[] newInput = new int[input.Length + 1];
            input.CopyTo(newInput, 0);
            newInput[newInput.Length - 1] = checkDigit;
            return _Check(newInput);
        }
        //-----------------------------------------------------------------------------------------------
        #endregion
    }
}

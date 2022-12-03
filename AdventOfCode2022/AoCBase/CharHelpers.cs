using System;

namespace AoCBase
{
    public static class CharHelpers
    {
        public static int PositionInAlphabet(this char c)
        {
            if (char.IsLower(c))
            {
                return c - 'a' + 1;
            }

            if (char.IsUpper(c))
            {
                return c - 'A' + 1;
            }

            throw new ArgumentOutOfRangeException($"{c} is not a letter");
        }
    }
}

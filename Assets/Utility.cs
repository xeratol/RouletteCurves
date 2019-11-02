public class Utility
{
    public static int GCD(int a, int b)
    {
        if (a == 0)
        {
            return b;
        }

        if (b == 0)
        {
            return a;
        }

        int c;
        if (b > a)
        {
            c = b;
            b = a;
            a = c;
        }
        c = a % b;

        return GCD(b, c);
    }
}

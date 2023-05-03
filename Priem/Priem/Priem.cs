using System;

static class Program {
    public static void Main() {
        string stdin = Console.ReadLine();
        if (stdin == null) {
            Environment.Exit(1);
        }

        if (!long.TryParse(stdin, out long value)) {
            Console.Error.WriteLine("Invalid number provided");
            Environment.Exit(1);
        }

        if (value == 2) {
            Console.WriteLine("2");
            return;
        }
        
        long prime = 0;
        do {
            bool isPrime = IsPrime(++value, 200);
            if (isPrime) {
                prime = value;
            }
        } while (prime == 0);

        Console.WriteLine(prime.ToString());
    }

    private static long ModularPower(long x, long y, long p) {
        long res = 1;
        x %= p;
        while (y > 0) {
            if ((y & 1) != 0) {
                res = res * x % p;
            }

            y /= 2;
            x = x * x % p;
        }

        return res;
    }
    
    private static bool IsLikelyPrime(long d, long n) {
        long a = 2 + new Random().Next() % (n - 4);
        long x = ModularPower(a, (long) d, n);

        if (x == 1 || x == n - 1) {
            return true;
        }

        while (d != n - 1) {
            x = x * x % n;
            d *= 2;
            
            if (x == 1) {
                return false;
            }

            if (x == n - 1) {
                return true;
            } 
        }

        return false;
    }

    private static bool IsPrime(long n, int k) {
        if (n <= 1 || n == 4) {
            return false;
        }

        if (n <= 3) {
            return true;
        }

        long d = n - 1;      
        while (d % 2 == 0) {
            d /= 2;
        }

        for (int i = 0; i < k; i++) {
            if (!IsLikelyPrime(d, n)) {
                return false;
            }
        }

        return true;
    }
}
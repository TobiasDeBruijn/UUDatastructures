using System;
using System.Collections.Generic;
using System.Linq;

internal struct Inputs {
    public readonly long MaxTripCount;
    public readonly long[] PkgWeights;

    public Inputs(long maxTripCount, long[] pkgWeights) {
        this.MaxTripCount = maxTripCount;
        this.PkgWeights = pkgWeights;
    }
}

static class Program {

    public static void Main() {
        Inputs inputs = ParseStdin();
        long minVanSize = CalculateVanSize(inputs.MaxTripCount, inputs.PkgWeights);
        
        Console.WriteLine(minVanSize.ToString());
    }

    private static Inputs ParseStdin() {
        string stdin = Console.ReadLine();
        string[] parts = stdin
            .Trim()
            .Split(' ');

        long maxTripCount = long.Parse(parts[1].Trim());
        List<long> pkgWeights = new List<long>();

        do {
            stdin = Console.ReadLine();
            if (string.IsNullOrEmpty(stdin)) {
                break;
            }
            
            parts = stdin.Split(' ');
            foreach (string part in parts) {
                if (string.IsNullOrEmpty(part) || string.IsNullOrWhiteSpace(part)) {
                    continue;
                }
                
                pkgWeights.Add(long.Parse(part.Trim()));
            }
        } while (true);

        return new Inputs(maxTripCount, pkgWeights.ToArray());
    }

    private static long CalculateVanSize(long maxTripCount, long[] packageWeights) {
        long left = 0;
        long right = packageWeights.Sum();

        while (left < right) {
            long mid = (left + right) / 2;

            if (IsWeightValid(maxTripCount, mid, packageWeights)) {
                right = mid;
            } else {
                left = mid + 1;
            }
        }

        return left;
    }

    private static bool IsWeightValid(long maxTripCount, long maxWeight, long[] pkgWeights) {
        long tripWeightSum = 0;
        long tripCount = 1;
        
        for (long i = 0; i < pkgWeights.Length; i++) {
            long pkgWeight = pkgWeights[i];

            if (tripWeightSum + pkgWeight > maxWeight) {
                tripWeightSum = pkgWeight;
                tripCount++;
            } else {
                tripWeightSum += pkgWeight;
            }
        }
        
        return tripCount <= maxTripCount;
    }
}
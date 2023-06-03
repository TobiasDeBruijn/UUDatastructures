using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MotorMerge {
    internal struct Input {
        public readonly long RowCount;
        public readonly MotorSpec[] MotorSpecs;

        public Input(long rowCount, MotorSpec[] motorSpecs) {
            RowCount = rowCount;
            MotorSpecs = motorSpecs;
        }
    }

    internal struct MotorSpec {
        public readonly string Name;
        public readonly long Price;
        public readonly long Speed;
        public readonly long Weight;

        public MotorSpec(string name, long price, long speed, long weight) {
            Name = name;
            Price = price;
            Speed = speed;
            Weight = weight;
        }
    }

    internal struct Output {
        public readonly long RowCount;
        public readonly MotorSpec[] List1;
        public readonly MotorSpec[] List2;

        public Output(long rowCount, MotorSpec[] list1, MotorSpec[] list2) {
            RowCount = rowCount;
            List1 = list1;
            List2 = list2;
        }
    }
    
    public static class Program {
        public static void Main() {
            Input input = ParseStdin();
            MotorSpec[] list1 = SortList1(input.MotorSpecs);
            MotorSpec[] list2 = SortList2(input.MotorSpecs);
            Output output = new Output(input.RowCount, list1, list2);
            PrintOutput(output);
        }

        private static void PrintOutput(Output output) {
            Console.WriteLine(output.RowCount);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < output.List1.Length; i++) {
                MotorSpec el1 = output.List1[i];
                MotorSpec el2 = output.List2[i];

                stringBuilder.Append(el1.Name);
                stringBuilder.Append(new string('.', 21 - stringBuilder.Length));
                stringBuilder.Append(el2.Name);
                stringBuilder.Append(new string('.', 42 - stringBuilder.Length));
                
                Console.WriteLine(stringBuilder.ToString());
                stringBuilder.Clear();
            }
        }
        
        private static MotorSpec[] SortList1(MotorSpec[] input) {
            return MergeSort(input, (a, b) => {
                if (a.Price == b.Price) {
                    if (a.Speed == b.Speed) {
                        if (a.Weight == b.Weight) {
                            return string.Compare(a.Name, b.Name, StringComparison.CurrentCulture) <= 0;
                        }

                        return a.Weight <= b.Weight;
                    }

                    return a.Speed >= b.Speed;
                }

                return a.Price <= b.Price;
            });
        }

        private static MotorSpec[] SortList2(MotorSpec[] input) {
            return MergeSort(input, (a, b) => {
                if (a.Speed == b.Speed) {
                    if (a.Weight == b.Weight) {
                        if (a.Price == b.Price) {
                            return string.Compare(a.Name, b.Name, StringComparison.CurrentCulture) <= 0;
                        }

                        return a.Price <= b.Price;
                    }

                    return a.Weight <= b.Weight;
                }

                return a.Speed >= b.Speed;
            });
        }

        private static Input ParseStdin() {
            string row = Console.ReadLine();
            Debug.Assert(row != null, nameof(row) + " != null");
            string[] parts = row.Split(' ');

            long rowCount = int.Parse(parts[0]);
            
            List<MotorSpec> spec = new List<MotorSpec>();
            for (long i = 0; i < rowCount; i++) {
                row = Console.ReadLine();
                Debug.Assert(row != null, nameof(row) + " != null");
                parts = row.Split(' ');
                spec.Add(new MotorSpec(
                    parts[0],
                    long.Parse(parts[1]),
                    long.Parse(parts[2]),
                    long.Parse(parts[3])
                ));
            }

            return new Input(rowCount, spec.ToArray());
        }

        private static T[] MergeSort<T>(T[] input, Func<T, T, bool> comparator) {
            if (input.Length <= 1) {
                return input;
            }

            int dividerIdx = input.Length / 2;
            T[] left = input.Take(dividerIdx).ToArray();
            T[] right = input.Skip(dividerIdx).Take(input.Length - dividerIdx).ToArray();
            
            left = MergeSort(left, comparator);
            right = MergeSort(right, comparator);
            
            return Merge(left.ToList(), right.ToList(), comparator);
        }

        private static T[] Merge<T>(ICollection<T> left, ICollection<T> right, Func<T, T, bool> comparator) {
            List<T> result = new List<T>();

            while (left.Count > 0 || right.Count > 0) {
                if (left.Count > 0 && right.Count > 0) {
                    if (comparator(left.First(), right.First())) {
                        result.Add(left.First());
                        left.Remove(left.First());
                    } else {
                        result.Add(right.First());
                        right.Remove(right.First());
                    }
                } else if (left.Count > 0) {
                    result.Add(left.First());
                    left.Remove(left.First());
                } else if (right.Count > 0) {
                    result.Add(right.First());
                    right.Remove(right.First());
                }
            }

            return result.ToArray();
        }
    }
}
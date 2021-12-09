using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FreeUp {
    class Program {
        static void Main(string[] args) {
            // args: dir size(mb)

            if (args.Length != 2) {
                Console.WriteLine(@"Useage: FreeUp.exe C:\some\folder 5000(in mb) ");
                return;
            }

            DirectoryInfo root = new DirectoryInfo(args[0]);
            DataSize minSize = new DataSize(Convert.ToInt64(args[1]) * 1000000);
            Console.WriteLine("Culling files from " + root.FullName + " until there is " + minSize + " free space.");

            FileInfo[] allFiles = root.GetFiles("*", SearchOption.AllDirectories);
            Console.WriteLine("Found " + allFiles.Count() + " files.");

            //Console.WriteLine("Largest: " + allFiles.OrderByDescending(o => o.Length).First().FullName);
            //Console.WriteLine("Oldest: " + allFiles.OrderBy(o => o.CreationTime).First().FullName);

            // iterate through the files, starting at the oldest, and delete them until there's enough free space
            foreach (FileInfo oldFile in allFiles.OrderBy(o => o.CreationTime)) {
                DataSize freeSpace = GetFreeSpace(root.Root.FullName);
                if (freeSpace.Bytes >= minSize.Bytes) break;

                Console.WriteLine("Free space: " + freeSpace + ", deleting: " + oldFile.FullName);
                oldFile.Delete();
            }

            DataSize freeSpace2 = GetFreeSpace(root.Root.FullName);
            if (freeSpace2.Bytes >= minSize.Bytes) {
                Console.WriteLine("Free space: " + GetFreeSpace(root.Root.FullName) + ", done.");
            } else {
                Console.WriteLine("Free space: " + GetFreeSpace(root.Root.FullName) + ", no files left to delete.");
            }

            // Console.ReadKey();
        }

        public static DataSize GetFreeSpace(string directory) {
            DriveInfo driveInfo = new DriveInfo(directory);
            return new DataSize(driveInfo.AvailableFreeSpace);
        }
    }

    public class DataSize {
        public long Bytes;

        public decimal Kilobytes {
            get {
                return Bytes / 1000;
            }
            set {
                Bytes = (long)(value * 1000);
            }
        }

        public decimal Megabytes {
            get {
                return Kilobytes / 1000;
            }
            set {
                Kilobytes = value * 1000;
            }
        }

        public decimal Gigabytes {
            get {
                return Megabytes / 1000;
            }
            set {
                Megabytes = value * 1000;
            }
        }

        public decimal Terabytes {
            get {
                return Gigabytes / 1000;
            }
            set {
                Gigabytes = value * 1000;
            }
        }

        public DataSize(long bytes) {
            Bytes = bytes;
        }

        public override string ToString() {
            if (Bytes < 2000) return Bytes.ToString("N") + " Bytes";
            if (Kilobytes < 2000) return Kilobytes.ToString("N2") + " Kilobytes";
            if (Megabytes < 2000) return Megabytes.ToString("N2") + " Megabytes";
            if (Gigabytes < 2000) return Gigabytes.ToString("N2") + " Gigabytes";
            return Terabytes.ToString("N2") + " Terabytes";
        }
    }
}

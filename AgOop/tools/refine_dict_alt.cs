namespace RefineDict
{

    internal class RefineDict
    {
        internal static int Main(string[] args)
        {
            Console.Write("Path/filename of the source file with words list to be checked: ");
            string? inFileName = Console.ReadLine();

            if (inFileName == "")
            {
                Console.WriteLine("No source specified - Operation cancelled");
                return 1;
            }

            if (!File.Exists(inFileName))
            {
                Console.WriteLine("File: %s not found - Operation cancelled", inFileName);
                return 1;
            }

            Console.Write("Path/filename of the destination file with refined dictionary: ");
            string? outFileName = Console.ReadLine();

            if (File.Exists(outFileName))
            {
                Console.Write("destination file already exists, OK to overwrite it? ");
                string? confirm = Console.ReadLine();
                if (confirm == null || !confirm.ToLower().Equals("y")) 
                {
                    Console.WriteLine("No overwriting of destination file - Operation cancelled");
                    return 1;
                }
            }
            else
            {
                Console.WriteLine("No destination specified - Operation cancelled");
                return 1;
            }


            if (File.Exists(inFileName))
            {
                using StreamReader sr = new StreamReader(inFileName);
                using StreamWriter sw = new StreamWriter(outFileName, false);
                List<string> lines = new List<string>();
                while(!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (line != null)
                    {
                        if (line.Length >= 3 && line.Length <= 7)
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }

            Console.WriteLine("File processed successfully");
            return 0;
        }

    }
}
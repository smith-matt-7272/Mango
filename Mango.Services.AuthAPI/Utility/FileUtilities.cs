namespace Mango.Services.AuthAPI.Utility
{
    public static class FileUtilities
    {
        private const string CSV_EXT = ".csv";
        private const string DATE_FORMAT = "yyyy-MM-dd hhmmss";

        public static List<string[]> ProcessBulkRegistrationFile(IFormFile bulkFile, string[] headers)
        {
            List<string[]> result = new List<string[]>();

            if (CSV_EXT.Equals(Path.GetExtension(bulkFile.FileName)))
            {
                // Rename the file to be the product ID, but we also want to append the existing
                // file extension from the dto
                string filePathDirectory = CopyToLocal(bulkFile);

                if(!string.IsNullOrEmpty(filePathDirectory))
                {
                    using (StreamReader sr = new StreamReader(filePathDirectory))
                    {
                        string? line = sr.ReadLine();
                        int[]? headerOrder = null;

                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] firstLine = line.Split(",");

                            if (firstLine.Contains(headers[0]))
                            {
                                headerOrder = OrderHeaders(headers, firstLine);
                            }
                        }

                        if( headerOrder != null)
                        {
                            while((line = sr.ReadLine()) != null) 
                            {
                                string[] splitLine = line.Split(",");
                                string[] arranged = new string[splitLine.Length];

                                for(int i = 0; i < headerOrder.Length; i++)
                                {
                                    arranged[i] = splitLine[headerOrder[i]];
                                }

                                result.Add(arranged);
                            }
                        }

                    }
                }
            }
            return result;
        }

        private static string CopyToLocal(IFormFile bulkFile)
        {
            try
            {
                // Rename the file to be the product ID, but we also want to append the existing
                // file extension from the dto
                string fileName = DateTime.Now.ToString(DATE_FORMAT) + CSV_EXT;
                string filePath = @"wwwroot/BulkRegistrations/" + fileName;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    bulkFile.CopyTo(fileStream);
                }

                return filePathDirectory;
            } catch
            {

            }

            return "";
        }

        private static int[] OrderHeaders(string[] definedHeaders, string[] fileHeaders)
        {
            int headerCount = definedHeaders.Length;
            int[] headerOrder = new int[headerCount];

            for(int i = 0; i < headerCount; i++)
            {
                for (int j = 0; j < headerCount; j++)
                {
                    if (definedHeaders[i].Equals(fileHeaders[j]))
                    {
                        headerOrder[i] = j;
                    }
                }
            }

            return headerOrder;
        }
    }
}

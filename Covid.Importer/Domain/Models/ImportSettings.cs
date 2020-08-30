namespace Covid.Importer.Domain.Models
{
    public class ImportSettings
    {
        public bool RecreateDataBase { get; set; }
        public string SourceFolder { get; set; }
        public string FileSearchPattern { get; set; }
        public int BatchSize { get; set; }
        public string ErrorsFolder { get; set; }
        public bool RecreateErrorsFolder {get; set;}
        public string DbConnectionString { get; set; }
        public int CSVFileSizeInMBLimit { get; set; }
        public int ParallelCSVReaders { get; set; }
        public int ParallelPersisters { get; set;}

    }
}
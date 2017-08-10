using System;
using System.Configuration;

namespace Molmed.Chiasma
{
    public class ServiceConfig
    {
        public static String BulkResultPlateGenotypeFormatFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["BulkResultPlateGenotypeFormatFilePath"];
            }
        }

        public static String BulkInternalReportGenotypeFormatFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["BulkInternalReportGenotypeFormatFilePath"];
            }
        }

        public static String BulkFileBaseDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["BulkFileBaseDirectory"];
            }
        }
    
    }



}
using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.IO;

namespace Molmed.Chiasma
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "Molmed.Chiasma")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class BulkFileService : System.Web.Services.WebService
    {
        private const String RESULT_PLATE_BULK_PREFIX = "GenotypesForResultPlate";
        private const String INTERNAL_REPORT_BULK_PREFIX = "GenotypesForInternalReport";
        private const String COLUMN_DELIMITER = "\t";


        [WebMethod]
        public void AppendToBulkResultPlateGenotypeFile(String databaseName, Int32 resultPlateId, Int32 sampleId,
            Int32[] markerId, Char[] strand, Byte[] alleleResultId)
        {
            StreamWriter sw = null;

            try
            {
                //Check arguments.
                if (strand.Length != markerId.Length || alleleResultId.Length != markerId.Length)
                {
                    throw new Exception("Not same array lengths when trying to append to bulk file.");
                }

                sw = new StreamWriter(GetBulkResultPlateGenotypeFilePath(databaseName, resultPlateId), true);
                for (int i = 0; i < markerId.GetLength(0); i++)
                {
                    sw.WriteLine(sampleId.ToString() + COLUMN_DELIMITER +
                        markerId[i].ToString() + COLUMN_DELIMITER +
                        strand[i].ToString() + COLUMN_DELIMITER +
                        alleleResultId[i].ToString());
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        [WebMethod]
        public void AppendToBulkInternalReportGenotypeFile(String databaseName, Int32 internalReportId, Int32 itemId,
            Int32[] markerId, Byte[] topAlleleResultId)
        {
            StreamWriter sw = null;

            try
            {
                //Check arguments.
                if (topAlleleResultId.Length != markerId.Length)
                {
                    throw new Exception("Not same array lengths when trying to append to bulk file.");
                }

                sw = new StreamWriter(GetBulkInternalReportGenotypeFilePath(databaseName, internalReportId), true);
                for (int i = 0; i < markerId.GetLength(0); i++)
                {
                    sw.WriteLine(itemId.ToString() + COLUMN_DELIMITER +
                        markerId[i].ToString() + COLUMN_DELIMITER +
                        topAlleleResultId[i].ToString());
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        [WebMethod]
        public void AppendToBulkInternalReportGenotypeFile_Byte(String databaseName, Int32 internalReportId, byte[] fileByte, int chunkSize)
        {
            FileStream fileStream = null;

            try
            {
                fileStream = File.Open(GetBulkInternalReportGenotypeFilePath(databaseName, internalReportId),
                    FileMode.Append, FileAccess.Write, FileShare.Read);
                fileStream.Write(fileByte, 0, chunkSize);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [WebMethod]
        public bool CheckBulkResultPlateGenotypeFileExists(String databaseName, Int32 resultPlateId)
        {
            return File.Exists(GetBulkResultPlateGenotypeFilePath(databaseName, resultPlateId));
        }

        [WebMethod]
        public bool CheckBulkInternalReportGenotypeFileExists(String databaseName, Int32 internalReportId)
        {
            return File.Exists(GetBulkInternalReportGenotypeFilePath(databaseName, internalReportId));
        }

        [WebMethod]
        public void DeleteBulkResultPlateGenotypeFile(String databaseName, Int32 resultPlateId)
        {
            File.Delete(GetBulkResultPlateGenotypeFilePath(databaseName, resultPlateId));
        }

        [WebMethod]
        public void DeleteBulkInternalReportGenotypeFile(String databaseName, Int32 internalReportId)
        {
            File.Delete(GetBulkInternalReportGenotypeFilePath(databaseName, internalReportId));
        }

        [WebMethod]
        public String GetBulkFileBaseDirectory()
        {
            return ServiceConfig.BulkFileBaseDirectory;
        }

        [WebMethod]
        public String GetBulkResultPlateGenotypeFilePath(String databaseName, Int32 resultPlateId)
        {
            return GetBulkFileSubDirectory(databaseName) + "\\" + 
                RESULT_PLATE_BULK_PREFIX + "_" + resultPlateId.ToString() + ".txt";
        }

        [WebMethod]
        public String GetBulkInternalReportGenotypeFilePath(String databaseName, Int32 internalReportId)
        {
            return GetBulkFileSubDirectory(databaseName) + "\\" +
                INTERNAL_REPORT_BULK_PREFIX + "_" + internalReportId.ToString() + ".txt";
        }

        [WebMethod]
        public String GetBulkResultPlateGenotypeFormatFilePath()
        {
            return ServiceConfig.BulkResultPlateGenotypeFormatFilePath;
        }

        [WebMethod]
        public String GetBulkInternalReportGenotypeFormatFilePath()
        {
            return ServiceConfig.BulkInternalReportGenotypeFormatFilePath;
        }

        private String GetBulkFileSubDirectory(String databaseName)
        {
            return ServiceConfig.BulkFileBaseDirectory + "\\" + databaseName;
        }

        [WebMethod]
        public void InitBulkResultPlateGenotypeFile(String databaseName, Int32 resultPlateId)
        {
            StreamWriter sw = null;

            try
            {
                //Make sure that the base directory exists, otherwise cannot continue.
                if (!Directory.Exists(ServiceConfig.BulkFileBaseDirectory))
                {
                    throw new Exception("The bulk file base directory does not exist.");
                }

                //Create the subdirectory for the specified database
                //if it does not already exist.
                if (!Directory.Exists(GetBulkFileSubDirectory(databaseName)))
                {
                    Directory.CreateDirectory(GetBulkFileSubDirectory(databaseName));
                }

                sw = new StreamWriter(GetBulkResultPlateGenotypeFilePath(databaseName, resultPlateId));
                sw.WriteLine("result_plate_id=" + resultPlateId.ToString());
                sw.WriteLine("sample_id" + COLUMN_DELIMITER +
                    "marker_id" + COLUMN_DELIMITER +
                    "strand" + COLUMN_DELIMITER +
                    "allele_result_id");
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        [WebMethod]
        public void InitBulkInternalReportGenotypeFile(String databaseName, Int32 internalReportId, bool isItemIndividual)
        {
            StreamWriter sw = null;
            string itemString;

            try
            {
                //Make sure that the base directory exists, otherwise cannot continue.
                if (!Directory.Exists(ServiceConfig.BulkFileBaseDirectory))
                {
                    throw new Exception("The bulk file base directory does not exist.");
                }

                //Create the subdirectory for the specified database
                //if it does not already exist.
                if (!Directory.Exists(GetBulkFileSubDirectory(databaseName)))
                {
                    Directory.CreateDirectory(GetBulkFileSubDirectory(databaseName));
                }

                sw = new StreamWriter(GetBulkInternalReportGenotypeFilePath(databaseName, internalReportId));
                sw.WriteLine("internal_report_id=" + internalReportId.ToString());

                if (isItemIndividual)
                {
                    itemString = "individual_id";
                }
                else
                {
                    itemString = "sample_id";
                }
                sw.WriteLine(itemString + COLUMN_DELIMITER +
                    "marker_id" + COLUMN_DELIMITER +
                    "top_allele_result_id");
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

    }
}

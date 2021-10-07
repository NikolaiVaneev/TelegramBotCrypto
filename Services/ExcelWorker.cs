using ExcelDataReader;
using System.Collections.Generic;
using System.IO;
using TelegramBotCrypto.Models;

namespace TelegramBotCrypto.Services
{
    public static class ExcelWorker
    {
        public static List<Crypto> GetData(string filePath, string cryptoTypeId)
        {
            List<Crypto> cryptoList = new List<Crypto>();

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {

                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    System.Data.DataSet doc = reader.AsDataSet();
                    System.Data.DataTable table = doc.Tables[0];

                    if (table.Rows.Count > 0)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            cryptoList.Add(new Crypto()
                            {
                                Code = table.Rows[i][0].ToString(),
                                Link = table.Rows[i][1].ToString(),
                                CryptoId = cryptoTypeId
                            });
                        }
                    }
                }
            }
            return cryptoList;
        }
    }
}

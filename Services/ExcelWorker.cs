using ExcelDataReader;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Models;

namespace TelegramBotCrypto.Services
{
    public static class ExcelWorker
    {
        public static List<Wallet> GetData(string filePath, int cryptoTypeId)
        {
            List<Wallet> cryptoList = new List<Wallet>();

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
                            cryptoList.Add(new Wallet()
                            {
                                Code = table.Rows[i][0].ToString(),
                            //    Link = table.Rows[i][1].ToString(),
                               CryptoTypeId = cryptoTypeId
                            });
                        }
                    }
                }
            }
            return cryptoList;
        }

        public static void ShowAllProjectUsers(string projectName)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel не установлен!");
                return;
            }


            Project project = DataBase.GetAllProjects().FirstOrDefault(u => u.Title == projectName);
            if (project == null) return;

            List<Participation> participations = DataBase.GetParticipationProject(project.Id);

            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Columns["A:A"].ColumnWidth = 12;
            xlWorkSheet.Columns["B:B"].ColumnWidth = 14;
            xlWorkSheet.Columns["C:C"].ColumnWidth = 10;
            xlWorkSheet.Columns["D:D"].ColumnWidth = 45;


            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Ник";
            xlWorkSheet.Cells[1, 3] = "Тип";
            xlWorkSheet.Cells[1, 4] = "Кошелек";
            

            for (int i = 0; i < participations.Count(); i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = participations[i].User.User_Id;
                xlWorkSheet.Cells[i + 2, 2] = participations[i].User.User_Nickname;
                xlWorkSheet.Cells[i + 2, 3] = project.CryptoType.Title;
                xlWorkSheet.Cells[i + 2, 4] = DataBase.GetWallet(participations[i].User.User_Id, project.CryptoTypeId).Code;
            }
            xlApp.Visible = true;
        }

        internal static void ShowSendingReport(List<User> sendedMessage, List<User> notSendedMessage)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel не установлен!");
                return;
            }

            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Columns["A:A"].ColumnWidth = 20;
            xlWorkSheet.Columns["B:B"].ColumnWidth = 20;

            xlWorkSheet.Cells[1, 1] = "Получившие";
            xlWorkSheet.Cells[1, 2] = "Не получившие";


            for (int i = 0; i < sendedMessage.Count; i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = $"{sendedMessage[i].User_Nickname} ({sendedMessage[i].User_Id})" ;
            }

            for (int i = 0; i < notSendedMessage.Count; i++)
            {
                xlWorkSheet.Cells[i + 2, 2] = $"{notSendedMessage[i].User_Nickname} ({notSendedMessage[i].User_Id})";
            }
            

            xlApp.Visible = true;
        }
    }
}

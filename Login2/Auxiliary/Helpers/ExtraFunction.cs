﻿using Login2.Auxiliary.Enums;
using Login2.Models;
using Login2.ViewModels;

using Login2.ViewModels.HumanResources;
using Login2.ViewModels.Receptionist;
using Login2.ViewModels.Sales;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Login2.Auxiliary.Helpers
{
    public static class ExtraFunction
    {
        public static bool ValidDateTime(string dateString)
        {
            DateTime dateValue;
            if (DateTime.TryParse(dateString, out dateValue))
                return true;
            else
                return false;
        }
        public static int[]  getWH(string value) 
        {
            var token= value.Split(new char[] {',','='});
            //Regex regex = new Regex(@"Width=(?<Width>[0-9]+),Height=(?<Height>[0-9]+)");

            return new int[] { Int32.Parse(token[1]), Int32.Parse(token[3].Remove(token[3].Length - 1)) };
        }
        public static List<string> featureOfRole(Roles role)
        {
            List<string> items = new List<string>();
            switch (role)
            {
                case Roles.HumanResources:
                    // code block
                    items = ConvertEnumToList.GetListOfDescription<HumanResourcesFeatures>();
                    break;
                case Roles.Sales:
                    // code block
                    items = ConvertEnumToList.GetListOfDescription<SalesFeatures>();
                    break;
                case Roles.Accountings:
                    items = ConvertEnumToList.GetListOfDescription<AccountantFeatures>();
                    break;
                case Roles.Receiptions:
                    items = ConvertEnumToList.GetListOfDescription<ReceptionistFeatures>();
                    break;
                default:
                    // code block
                    break;
            }
            return items;
        }

        public static MyBaseViewModel getUserControl(int index, Roles role)
        {
            //MyBaseViewModel item = new MyBaseViewModel();
            ViewModelFactory viewModelFactory = ViewModelFactory.Instance();
            switch (role)
            {
                case Roles.HumanResources:
                    {
                        HumanResourcesFeatures feateture = (HumanResourcesFeatures)Enum.ToObject(typeof(HumanResourcesFeatures), index);
                        return viewModelFactory.getViewModel(feateture.ToString());
                    }

                case Roles.Accountings:
                    {
                        AccountantFeatures feateture = (AccountantFeatures)Enum.ToObject(typeof(AccountantFeatures), index);
                        return viewModelFactory.getViewModel(feateture.ToString());
                    }
                case Roles.Receiptions:
                    {
                        ReceptionistFeatures feateture = (ReceptionistFeatures)Enum.ToObject(typeof(ReceptionistFeatures), index);
                        return viewModelFactory.getViewModel(feateture.ToString());
                    }

                case Roles.Sales:
                    {
                        SalesFeatures feateture = (SalesFeatures)Enum.ToObject(typeof(SalesFeatures), index);
                        return viewModelFactory.getViewModel(feateture.ToString());
                    }
                default:
                    // code block
                    break;
            }
            return null;
        }

        public static string generateUserName(staff p)
        {
            StringBuilder res = new StringBuilder();
            string[] words = RemoveSign4VietnameseString(p.Name).Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                res.Append(words[i][0]);
            }
            res.Append(words[words.Length - 1]);
            res.Append(p.DOB.Value.Year);
            return res.ToString();
        }

        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        {
            //DateTime Conversion
            var convertDateTime = new Func<double, DateTime>(excelDate =>
            {
                if (excelDate < 1)
                    throw new ArgumentException("Excel dates cannot be smaller than 0.");

                var dateOfReference = new DateTime(1900, 1, 1);

                if (excelDate > 60d)
                    excelDate = excelDate - 2;
                else
                    excelDate = excelDate - 1;
                return dateOfReference.AddDays(excelDate);
            });

            //Get the properties of T
            var tprops = (new T())
                .GetType()
                .GetProperties()
                .ToList();

            //Cells only contains references to cells with actual data
            var groups = worksheet.Cells
                .GroupBy(cell => cell.Start.Row)
                .ToList();

            //Assume the second row represents column data types (big assumption!)
            var types = groups
                .Skip(1)
                .First()
                .Select(rcell => rcell.Value.GetType())
                .ToList();

            //Assume first row has the column names
            var colnames = groups
                .First()
                .Select((hcell, idx) => new { Name = hcell.Value.ToString(), index = idx })
                .Where(o => tprops.Select(p => p.Name).Contains(o.Name))
                .ToList();

            //Everything after the header is data
            var rowvalues = groups
                .Skip(1) //Exclude header
                .Select(cg => cg.Select(c => c.Value).ToList());


            //Create the collection container
            var collection = rowvalues
                .Select(row =>
                {
                    var tnew = new T();
                    colnames.ForEach(colname =>
                    {
                        //This is the real wrinkle to using reflection - Excel stores all numbers as double including int
                        var val = row[colname.index];
                        var type = types[colname.index];
                        var prop = tprops.First(p => p.Name == colname.Name);
                        //If it is numeric it is a double since that is how excel stores all numbers
                        if (type == typeof(double))
                        {
                            //Unbox it
                            var unboxedVal = (double)val;
                            //Debug.WriteLine(unboxedVal);
                            //FAR FROM A COMPLETE LIST!!!
                            if (prop.PropertyType == typeof(Int32))
                                prop.SetValue(tnew, (int)unboxedVal);
                            else if (prop.PropertyType == typeof(double))
                                prop.SetValue(tnew, unboxedVal);
                            else if (prop.PropertyType == typeof(DateTime))
                                prop.SetValue(tnew, convertDateTime(unboxedVal));
                            else
                                throw new NotImplementedException(String.Format("Type '{0}' not implemented yet!", prop.PropertyType.Name));
                        }
                        else
                        {
                            //Its a string
                            prop.SetValue(tnew, val);
                        }
                    });

                    return tnew;
                });


            //Send it back
            return collection;
        }
        
        private static readonly string[] VietnameseSigns = new string[]
            {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
            };
        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        public static SolidColorBrush getColorFrom(string hexCode)
        {
            byte R = Convert.ToByte(hexCode.Substring(1, 2), 16);
            byte G = Convert.ToByte(hexCode.Substring(3, 2), 16);
            byte B = Convert.ToByte(hexCode.Substring(5, 2), 16);
            return new SolidColorBrush(Color.FromRgb(R, G, B));
        }
    }
}

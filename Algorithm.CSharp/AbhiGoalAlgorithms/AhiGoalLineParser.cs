

using System;
using System.Collections.Generic;
using System.IO;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// The object which processes the csv data into a list of AbhiGoalCustomData objects
    /// </summary>
    public class AhiGoalLineParser
    {
        private int count;
        private string getset = @"{ get; set; }";
        private List<AbhiGoalCustomData> list;
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public AhiGoalLineParser()
        {
            list = new List<AbhiGoalCustomData>();
        }
        /// <summary>
        /// Processes the csv data into a list of AbhiGoalCustomData objects
        /// </summary>
        /// <param name="inputString">the inputString to read from</param>
        /// <returns>the List of AbhiGoalCustomData objects </returns>
        public List<AbhiGoalCustomData> ProcessStreamIntoList(string inputString)
        {
            string[] arrLine = inputString.Split('\n');
            foreach (string t in arrLine)
            {
                string line = t.Trim();
                if (line.Length == 0)
                    continue;

                // skip the first line.  It is now a do nothing if body, but was used to 
                //  get a list of the columns in the csv file from the first line to 
                //  generate the ExternalIndicators class code
                if (count == 0)
                {
                    string[] columns = line.Split(',');
                    foreach (string column in columns)
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format(@"        public string {0} {1}",column.Trim(), getset));
                    }
                }
                else
                {
                    var xi = ParceRowIntoObject(line);
                    list.Add(xi);
                }
                count++;
            }
            return list;
        }

        /// <summary>
        /// Parces a line from a csv file into an object
        /// </summary>
        /// <param name="line">string - the csv line</param>
        /// <returns>AbhiGoalCustomData - the data object</returns>
        /// <exception cref="Exception"></exception>
        public AbhiGoalCustomData ParceRowIntoObject(string line)
        {
            #region "process row into the object"

            if (line.Length == 0)
                return null;
            if (line.StartsWith("date"))
                return null;
            string[] row = line.Split(',');
            AbhiGoalCustomData abhiGoalCustomData = new AbhiGoalCustomData();
            DateTime dt;

            // date
            var result = DateTime.TryParse(row[0], out dt);
            if (!result)
                throw new InvalidDataException(string.Format("Invalid date in row {0}, column 1", count));
            else
            {
                abhiGoalCustomData.date = dt;
                abhiGoalCustomData.Time = dt + new TimeSpan(10,0,0);
            }
            // next_date
            result = DateTime.TryParse(row[1], out dt);
            if (!result)
                throw new InvalidDataException(string.Format("Invalid date in row {0}, column 2", count));
            else
            {
                abhiGoalCustomData.next_date = dt;
            }
            //next_close
            decimal next_close;
            result = decimal.TryParse(row[2], out next_close);
            if (!result)
                throw new InvalidDataException(string.Format("Invalid close in row {0}, column 3", count));
            else
            {
                abhiGoalCustomData.next_close = next_close;
            }

            // symbol
            abhiGoalCustomData.symbol = row[3];
            abhiGoalCustomData.Symbol = row[3];

            // close
            decimal close;
            result = decimal.TryParse(row[4], out close);
            abhiGoalCustomData.close = close;

            // kno
            int kno;
            string row5 = row[5].Trim();
            result = int.TryParse(row5.Substring(0, row5.Length - 1), out kno);
            abhiGoalCustomData.kno = kno;

            abhiGoalCustomData.w = row[6];
            abhiGoalCustomData.h = row[7];
            abhiGoalCustomData.e = row[8];
            abhiGoalCustomData.n = row[9];
            abhiGoalCustomData.rd = row[10];


            // pct
            decimal pct;
            result = decimal.TryParse(row[11], out pct);
            abhiGoalCustomData.pct = pct;

            abhiGoalCustomData.cr = row[12];
            abhiGoalCustomData.bsval1 = row[13];
            abhiGoalCustomData.bsval2 = row[14];
            abhiGoalCustomData.bsval3 = row[15];
            abhiGoalCustomData.bsval4 = row[16];



            #endregion

            return abhiGoalCustomData;
        }
    }
}

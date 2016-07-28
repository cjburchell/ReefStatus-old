// <copyright file="DataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI;

    /// <summary>
    /// Data Access comon methods
    /// </summary>
    public abstract class DataAccess
    {
        /// <summary>
        /// Gets or sets the write count.
        /// </summary>
        /// <value>The write count.</value>
        public int WriteCount { get; protected set; }

        /// <summary>
        /// Gets or sets the delete count.
        /// </summary>
        /// <value>The delete count.</value>
        public int DeleteCount { get; protected set; }

        /// <summary>
        /// Logs to file.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void AddLog(IList items, DateTime time, int Controller, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Update Database";
                progress.SetProgressSteps(items.Count);
            }

            try
            {
                var itemList = new Collection<BaseInfo>();
                foreach (BaseInfo item in items)
                {
                    itemList.Add(item);
                }

                foreach (BaseInfo param in itemList)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Saving " + param.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    if (param.SaveToDatabase && !(param is UserInfo))
                    {
                        this.InsertItem(param.DoubleValue, time, param.Id, true, Controller, param.OldDoubleValue);

                        if (param is SPort)
                        {
                            this.InsertItem(((SPort)param).Current, time, ((SPort)param).CurrentId, true, Controller, ((SPort)param).OldCurrentValue);
                        }
                    }
                }
            }
            finally
            {
                if (progress != null)
                {
                    progress.DisplayProgress = false;
                }
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        /// <param name="name">The name.</param>
        /// <param name="optimize">if set to <c>true</c> [optimize].</param>
        /// <param name="Controller">The Controller.</param>
        public abstract void InsertItem(double value, DateTime time, string name, bool optimize, int Controller, double? oldValue);

        /// <summary>
        /// Exports the data to the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="contoler">The contoler.</param>
        public void Export(string fileName, IProgressCallback callback, int contoler)
        {
            lock (callback.Lock)
            {
                try
                {
                    using (StreamWriter logFile = File.CreateText(fileName))
                    {
                        Dictionary<int, double> lastvalue = new Dictionary<int, double>();

                        Dictionary<int, string> types = this.GetTypes();
                        if (types.Count == 0)
                        {
                            return;
                        }

                        IEnumerable<DateTime> times = this.GetTimes();
                        int totalItems = times.Count();

                        callback.Begin(0, totalItems, Language.GetResource("strExportData"));
                        callback.SetText(Language.GetResource("strWritingHeader"));
                        WriteHeader(logFile, types);

                        int count = 0;
                        foreach (DateTime dataTime in times)
                        {
                            if (callback.IsAborting)
                            {
                                break;
                            }

                            callback.SetText(Language.GetResource("strProcessing") + " " + dataTime);
                            if (count < totalItems)
                            {
                                callback.StepTo(count++);
                            }

                            logFile.Write(dataTime.ToString("d", CultureInfo.CurrentCulture));
                            logFile.Write("\t" + dataTime.ToString("T", CultureInfo.CurrentCulture));
                            Dictionary<int, double> data = this.GetDataPoints(dataTime, contoler);

                            foreach (int type in types.Keys)
                            {
                                if (data.ContainsKey(type))
                                {
                                    logFile.Write("\t" + data[type].ToString(CultureInfo.CurrentCulture));
                                    if (lastvalue.ContainsKey(type))
                                    {
                                        lastvalue[type] = data[type];
                                    }
                                    else
                                    {
                                        lastvalue.Add(type, data[type]);
                                    }
                                }
                                else
                                {
                                    if (lastvalue.ContainsKey(type))
                                    {
                                        logFile.Write("\t" + lastvalue[type].ToString(CultureInfo.CurrentCulture));
                                    }
                                    else
                                    {
                                        logFile.Write("\t0");
                                    }
                                }
                            }

                            logFile.Write("\r\n");
                        }
                    }
                }
                catch (IOException ex)
                {
                    throw new DataAccessException(7000, "Unable to Export Data to file error writing file", ex);
                }
                catch (Exception ex)
                {
                    throw new DataAccessException(7001, "Unable to Export Data to file error in reading database", ex);
                }
                finally
                {
                    callback.End();
                }
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns>a dictionary of types and there indexes</returns>
        public abstract Dictionary<int, string> GetTypes();

        /// <summary>
        /// Inports the data from a specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        public void Import(string fileName, IProgressCallback callback, int Controller)
        {
            lock (callback.Lock)
            {
                try
                {
                    if (File.Exists(fileName))
                    {
                        {
                            string[] lines = File.ReadAllLines(fileName);
                            callback.Begin(0, lines.Length, Language.GetResource("strImport"));
                        }

                        using (StreamReader logFile = File.OpenText(fileName))
                        {
                            string line = logFile.ReadLine();
                            if (line != null)
                            {
                                string lineNoDate = line.Split(new[] { '\t' }, 3)[2];
                                string[] graphNames = lineNoDate.Split(new[] { '\t' });

                                line = logFile.ReadLine();
                                int count = 0;
                                while (!string.IsNullOrEmpty(line))
                                {
                                    if (callback.IsAborting)
                                    {
                                        break;
                                    }

                                    string[] line2 = line.Split(new[] { '\t' }, 3);

                                    DateTime datetime;
                                    if (!DateTime.TryParse(line2[0] + " " + line2[1], CultureInfo.CurrentCulture, DateTimeStyles.None, out datetime))
                                    {
                                        DateTime date;
                                        if (
                                            !DateTime.TryParseExact(
                                                line2[0],
                                                new[] { "dd/MM/yyyy", "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "dd/MM/yy", "d/MM/yy", "d/M/yy", "dd-MM-yyyy", "dd-MM-yyyy", "dd-M-yyyy", "d-M-yyyy", "dd-MM-yy", "d-MM-yy", "d-M-yy" },
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out date))
                                        {
                                            throw new DataAccessException(7004, "Invalid Date");
                                        }


                                        DateTime time;
                                        if (!DateTime.TryParseExact(
                                            line2[1],
                                            new[] { "HH:mm:ss", "H:mm:ss", "hh:mm:ss tt", "h:mm:ss tt" },
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None,
                                            out time))
                                        {
                                            throw new DataAccessException(7004, "Invalid Date");
                                        }

                                        datetime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                                    }


                                    callback.SetText(Language.GetResource("strProcessing") + datetime);
                                    callback.StepTo(count++);

                                    int column = 0;
                                    foreach (string item in line2[2].Split(new[] { '\t' }))
                                    {
                                        if (graphNames.Length > column && !string.IsNullOrEmpty(item))
                                        {
                                            string[] name = graphNames[column].Split(new[] { ' ' });
                                            double value;
                                            if (!double.TryParse(item, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value))
                                            {
                                                string normalVal = item.Replace(',', '.');
                                                if (!double.TryParse(normalVal, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                                                {
                                                    throw new DataAccessException(
                                                        7002, "Error Formating the data : " + item);
                                                }
                                            }


                                            this.InsertItem(value, datetime, name[0], false, Controller, null);
                                        }

                                        column++;
                                    }

                                    line = logFile.ReadLine();
                                }
                            }

                            logFile.Close();
                        }
                    }
                }
                catch (FormatException ex)
                {
                    throw new DataAccessException(7002, "Error Formating the data", ex);
                }
                catch (IOException ex)
                {
                    throw new DataAccessException(7003, "Unable to Export Data to file error writing file", ex);
                }
                catch (Exception ex)
                {
                    throw new DataAccessException(7004, "Unable to Export Data to file error in reading database", ex);
                }
                finally
                {
                    callback.End();
                }
            }
        }

        /// <summary>
        /// Logs to file.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        public void AddLog(Collection<ItemDataRow> data, IProgressCallback callback, int Controller)
        {
            callback.SetText(Language.GetResource("strAddingRecordstodatabase"));
            callback.SetRange(0, data.Count);
            int count = 0;
            foreach (ItemDataRow param in data)
            {
                if (callback.IsAborting)
                {
                    break;
                }

                foreach (ItemDataRow.Item value in param.Values)
                {
                    this.InsertItem(value.Value, param.Time, value.Id, false, Controller, null);
                }

                callback.StepTo(count++);
            }
        }

        /// <summary>
        /// Gets the number of data entries.
        /// </summary>
        /// <returns>the total number of reccords in the database</returns>
        public abstract int RecordCount();

        /// <summary>
        /// Calculates the STD dev.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>the standard deviation of the values</returns>
        protected static double CalculateStdDev(IEnumerable<double> values)
        {
            var count = values.Count();
            if (count > 1)
            {
                // Compute the Average
                double avg = values.Average();

                // Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together
                return Math.Sqrt(sum / ((double)count - 1));
            }

            return 0;
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="dataTime">
        /// The data time.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// a list of data points
        /// </returns>
        protected abstract Dictionary<int, double> GetDataPoints(DateTime dataTime, int Controller);

        /// <summary>
        /// Gets the times.
        /// </summary>
        /// <returns>a list of times</returns>
        protected abstract IEnumerable<DateTime> GetTimes();

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <param name="logFile">The log file.</param>
        /// <param name="columns">The columns.</param>
        private static void WriteHeader(StreamWriter logFile, Dictionary<int, string> columns)
        {
            logFile.Write(columns.Values.Aggregate("Date\tTime", (current, name) => current + ("\t" + name.Trim())) + "\r\n");
        }
    }
}

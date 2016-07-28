namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    using RedPoint.ReefStatus.Common.Settings;

    public class NeptuneControler : IProfilux
    {

        public NeptuneControler(ConnectionSettings connection)
        {
            this.OutletStates = new Collection<TimerHistoryInstance>();
            this.ProbeReadings = new Collection<ValueHistoryInstance>();
            ReadHtmlStatus(connection);
        }

        public void ReadHtmlStatus(ConnectionSettings connection)
        {
            XmlReader reader = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            StreamReader input = null;
            try
            {
                HttpWebRequest request;
                if (connection.Port == 80)
                {
                    request = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}/cgi-bin/status.xml", connection.Address));
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}:{1}/cgi-bin/status.xml", connection.Address, connection.Port));
                }
                request.PreAuthenticate = true;
                request.KeepAlive = false;
                if (!string.IsNullOrEmpty(connection.UserName) && !string.IsNullOrEmpty(connection.Password))
                {
                    request.Credentials = new NetworkCredential(connection.UserName, connection.Password);
                }
                request.Timeout = 0x7530;
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                input = new StreamReader(responseStream, Encoding.UTF8);

                XmlDocument doc = new XmlDocument();

                reader = XmlReader.Create(input);
                this.ResetStatus();
                string outletName = null;
                string probeName = null;
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                int num = 0;
                int num2 = 0;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (!string.Equals(reader.Name, "date", StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                                flag = true;
                                continue;
                            }
                        case XmlNodeType.Attribute:
                            {
                                continue;
                            }
                        case XmlNodeType.Text:
                            {
                                if (!flag)
                                {
                                    goto Label_01EB;
                                }
                                continue;
                            }
                        case XmlNodeType.EndElement:
                            {
                                if (flag)
                                {
                                    flag = false;
                                }
                                else if (flag2)
                                {
                                    flag2 = false;
                                }
                                else if (flag3)
                                {
                                    flag3 = false;
                                }
                                else if (flag4 && string.Equals(reader.Name, "probes", StringComparison.OrdinalIgnoreCase))
                                {
                                    flag4 = false;
                                }
                                else if (flag5 && string.Equals(reader.Name, "outlets", StringComparison.OrdinalIgnoreCase))
                                {
                                    flag5 = false;
                                }
                                continue;
                            }
                        default:
                            {
                                continue;
                            }
                    }
                    if (string.Equals(reader.Name, "failed", StringComparison.OrdinalIgnoreCase))
                    {
                        flag2 = true;
                    }
                    else if (string.Equals(reader.Name, "restored", StringComparison.OrdinalIgnoreCase))
                    {
                        flag3 = true;
                    }
                    else if (string.Equals(reader.Name, "probes", StringComparison.OrdinalIgnoreCase))
                    {
                        flag4 = true;
                    }
                    else if (string.Equals(reader.Name, "outlets", StringComparison.OrdinalIgnoreCase))
                    {
                        flag5 = true;
                    }
                    continue;
                Label_01EB:
                    if (flag2)
                    {
                        //this.PowerFailed = reader.Value;
                    }
                    else
                    {
                        if (flag3)
                        {
                            //this.PowerRestored = reader.Value;
                            continue;
                        }
                        if (flag4)
                        {
                            if ((num % 2) == 0)
                            {
                                probeName = reader.Value;
                            }
                            else
                            {
                                double recording = Convert.ToDouble(reader.Value);
                                this.ProbeReadings.Add(new ValueHistoryInstance(probeName, recording));
                            }
                            num++;
                            continue;
                        }
                        if (flag5)
                        {
                            if ((num2 % 2) == 0)
                            {
                                outletName = reader.Value;
                            }
                            else
                            {
                                this.OutletStates.Add(new TimerHistoryInstance(outletName, reader.Value));
                            }
                            num2++;
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                //this.Result = exception.Message;
            }
            catch (ArgumentOutOfRangeException exception2)
            {
                //this.Result = exception2.Message;
            }
            finally
            {
                if (input != null)
                {
                    input.Dispose();
                }
                if (responseStream != null)
                {
                    responseStream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void ResetStatus()
        {
            this.OutletStates.Clear();
            this.ProbeReadings.Clear();
        }

        private Collection<TimerHistoryInstance> OutletStates{ get; set;}

        private Collection<ValueHistoryInstance> ProbeReadings { get; set; }

        #region IProfilux Members

        public CurrentState Alarm
        {
            get 
            {
                return CurrentState.Off; 
            }
        }

        public ProductId ProductId
        {
            get
            {
                return Common.ProfiLux.ProductId.Neptune;
            }
        }

        public int SerialNumber
        {
            get
            {
                return 0;
            }
        }

        public System.DateTime SoftwareDate
        {
            get 
            {
                return new DateTime();
            }
        }

        public int DeviceAddress
        {
            get 
            {
                return 0;
            }
        }

        public int ModuleCount
        {
            get
            {
                return 0;
            }
        }

        public int ReminderCount
        {
            get { return 0; }
        }

        public int SensorCount
        {
            get
            {
                return this.ProbeReadings.Count;
            }
        }

        public int LevelSenosrCount
        {
            get { return 0; }
        }

        public int SPortCount
        {
            get 
            {
                return this.OutletStates.Count;
            }
        }

        public int LPortCount
        {
            get { return 0; }
        }

        public int DigitalInputCount
        {
            get { return 0; }
        }

        public int LightCount
        {
            get { return 0; }
        }

        public int TimerCount
        {
            get { return 0; }
        }

        public OperationMode OpMode
        {
            get
            {
                return OperationMode.Normal;
            }
            set
            {
                
            }
        }

        public CurrentState GetDigitalInputState(int index)
        {
            throw new System.NotImplementedException();
        }

        public PortMode GetSPortFunction(int portIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetSPortValue(int portIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetModuleVersion(int module)
        {
            throw new System.NotImplementedException();
        }

        public int GetModuleState(int module)
        {
            throw new System.NotImplementedException();
        }

        public int GetModuleId(int module)
        {
            throw new System.NotImplementedException();
        }

        public string GetReminderText(int reminder)
        {
            throw new System.NotImplementedException();
        }

        public void KeyCommand(FaceplateKey facePlateKey)
        {
            throw new System.NotImplementedException();
        }

        public void Thunderstorm(int duration)
        {
            throw new System.NotImplementedException();
        }

        public void WaterChange(int index)
        {
            throw new System.NotImplementedException();
        }

        public void FeedPause(bool activate)
        {
            throw new System.NotImplementedException();
        }

        public void Maintenace(bool activate)
        {
            throw new System.NotImplementedException();
        }

        public System.DateTime? GetNextReminder(int reminder)
        {
            throw new System.NotImplementedException();
        }

        public SensorType GetSensorType(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetSensorFormat(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetSensorNominalValue(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetSensorAlarmDeviation(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool GetSensorAlarmEnable(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetSensorValue(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public LevelState GetLevelSensorState(int levelSensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetLPortValue(int portIndex)
        {
            throw new System.NotImplementedException();
        }

        public PortMode GetLPortFunction(int portIndex)
        {
            throw new System.NotImplementedException();
        }

        public DigitalInputFunction GetDigitalInputFunction(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public LevelSensorOpertationMode GetLevelSensorMode(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public CurrentState GetSensorAlarm(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public int GetReminderPeriod(int reminder)
        {
            throw new System.NotImplementedException();
        }

        public bool GetReminderRepeats(int reminder)
        {
            throw new System.NotImplementedException();
        }

        public void ResetReminder(int reminder, int period)
        {
            throw new System.NotImplementedException();
        }

        public void ClearReminder(int reminder)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLightActive(int channel)
        {
            throw new System.NotImplementedException();
        }

        public double GetLightValue(int channel)
        {
            throw new System.NotImplementedException();
        }

        public int GetDosingRate(int channel)
        {
            throw new System.NotImplementedException();
        }

        public TimerSettings GetTimerSettings(int channel)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateTimerSettings(int channel, TimerSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateDosingRate(int channel, int rate)
        {
            throw new System.NotImplementedException();
        }

        public void ClearLevelAlarm(int index)
        {
            throw new System.NotImplementedException();
        }

        public SensorMode GetSensorMode(int sensorIndex)
        {
            throw new System.NotImplementedException();
        }

        public void SetSocketState(int portNumber, bool value)
        {
            throw new System.NotImplementedException();
        }

        public void SetLightValue(int channel, double value)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLightDimmable(int channel)
        {
            throw new System.NotImplementedException();
        }

        public int GetProbeOperationHours(int index)
        {
            return 0;
        }

        public void SetProbeOperationHours(int index, int value)
        {
            throw new System.NotImplementedException();
        }

        public int GetLightOperationHours(int channel)
        {
            return 0;
        }

        public void SetLightOperationHours(int channel, int value)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public bool IsConnected
        {
            get 
            {
                return true;
            }
        }

        public double Version
        {
            get
            {
                return 0;
            }
        }

        public string DisplayText
        {
            get 
            {
                return string.Empty; 
            }
        }

        public string ViewText
        {
            get { return string.Empty; }
        }

        public void Disconnect()
        {
        }

        public System.Collections.ObjectModel.Collection<ItemDataRow> GetDataPoints(UI.IProgressCallback callback, System.Collections.ObjectModel.ReadOnlyCollection<Probe> probes)
        {
            return new Collection<ItemDataRow>();
        }

        #region IProfilux Members


        public string GetSensorId(int index, SensorType type)
        {
            return ProbeReadings[index].ProbeName;
        }

        public string GetLevelId(int index)
        {
            throw new NotImplementedException();
        }

        public string GetDigtialInputId(int index)
        {
            throw new NotImplementedException();
        }

        public string GetSPortId(int index)
        {
            return OutletStates[index].OutletName;
        }

        public string GetLPortId(int index)
        {
            throw new NotImplementedException();
        }

        public string GetLightId(int index)
        {
            throw new NotImplementedException();
        }

        public string GetDousingPumpId(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IProfilux Members


        public string[] View
        {
            get { throw new NotImplementedException(); }
        }

        public string[] Display
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IProfilux Members


        public bool GetSensorActive(int index)
        {
            return true;
        }

        #endregion

        #region IProfilux Members


        public int WebServerPort
        {
            get 
            {
                return 80;
            }
        }

        public double Latitude
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        public double Longitude
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        public double MoonPhase
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region IProfilux Members


        public Image DisplayImage
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IProfilux Members


        public int CurrentPumpCount
        {
            get
            {
                return 0;
            }
        }

        public int GetCurrentPumpValue(int index)
        {
            return 0;
        }

        public string GetCurrentPumpId(int index)
        {
            return string.Empty;
        }

        #endregion

        #region IProfilux Members


        public bool IsCurrentPumpAssinged(int i)
        {
            return false;
        }

        #endregion
    }
}

using Microsoft.Win32;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using YamuraViewControls;

namespace YamuraView
{
    public partial class YamuraView : Form
    {
        #region members
        private Button colorButton = new Button();
        List<Color> penColors = new List<Color>();
        private List<Task> tasks = new List<Task>();

        // DataLogger contains runs, which contain channels which contain data points
        public static DataLogger dataLogger = new DataLogger();

        float gpsDist = 0.0F;
        public String FolderToWatch { get; private set; }
        public String ConfigurationFile { get; private set; }
        public SortedList<String, String> folderToWatchFiles = new System.Collections.Generic.SortedList<String, String>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SortedList<String, String> FolderToWatchFiles
        {
            get { return folderToWatchFiles; }
            set { folderToWatchFiles = value; }
        }
        List<YamuraViewControls.Chart> chartControls = new List<YamuraViewControls.Chart>();
        public bool timeAlign = true;
        public bool distanceAlign = true;
        public string timeAlignChannel = "gX";
        public float timeAlignThreshold = 0.5f;
        public bool timeAlignRisingEdge = true;
        #endregion

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public YamuraView()
        {
            InitializeComponent();
            FolderToWatch = @"C:\ftp_transfer";
            #region setup charts
            StripChart.ChartViewType = YamuraViewControls.Chart.ChartType.Stripchart;
            StripChart.ChartName = "Strip Chart";
            StripChart.EqualScale = false;

            TrackMap.ChartViewType = YamuraViewControls.Chart.ChartType.XYChart;
            TrackMap.ChartName = "Track Map";
            TrackMap.EqualScale = true;

            TractionCircle.ChartViewType = YamuraViewControls.Chart.ChartType.XYChart;
            TractionCircle.ChartName = "Traction Circle";
            TractionCircle.EqualScale = true;
            #endregion

            #region add chart controls
            chartControls.Add(StripChart);
            chartControls.Add(TrackMap);
            chartControls.Add(TractionCircle);
            chartControls[0].CursorMode = YamuraViewControls.ChartView.CursorStyle.VERTICAL;
            chartControls[0].CursorUpdateSource = true;
            chartControls[1].CursorMode = YamuraViewControls.ChartView.CursorStyle.BOX;
            chartControls[1].CursorUpdateSource = false;
            chartControls[2].CursorMode = YamuraViewControls.ChartView.CursorStyle.BOX;
            chartControls[2].CursorUpdateSource = false;
            #endregion

            #region chart control event handlers
            chartControls[0].chartProperties1.ChartXAxisChangeEvent += chartControls[0].chartView1.OnChartXAxisChange;
            chartControls[1].chartProperties1.ChartXAxisChangeEvent += chartControls[1].chartView1.OnChartXAxisChange;
            chartControls[2].chartProperties1.ChartXAxisChangeEvent += chartControls[2].chartView1.OnChartXAxisChange;

            chartControls[0].chartProperties1.ClearGraphicsPathEvent += chartControls[0].chartView1.OnClearGraphicsPath;
            chartControls[0].chartProperties1.ClearGraphicsPathEvent += chartControls[1].chartView1.OnClearGraphicsPath;
            chartControls[0].chartProperties1.ClearGraphicsPathEvent += chartControls[2].chartView1.OnClearGraphicsPath;

            chartControls[1].chartProperties1.ClearGraphicsPathEvent += chartControls[0].chartView1.OnClearGraphicsPath;
            chartControls[1].chartProperties1.ClearGraphicsPathEvent += chartControls[1].chartView1.OnClearGraphicsPath;
            chartControls[1].chartProperties1.ClearGraphicsPathEvent += chartControls[2].chartView1.OnClearGraphicsPath;


            chartControls[2].chartProperties1.ClearGraphicsPathEvent += chartControls[0].chartView1.OnClearGraphicsPath;
            chartControls[2].chartProperties1.ClearGraphicsPathEvent += chartControls[1].chartView1.OnClearGraphicsPath;
            chartControls[2].chartProperties1.ClearGraphicsPathEvent += chartControls[2].chartView1.OnClearGraphicsPath;


            chartControls[0].chartView1.ChartMouseTrackEvent += chartControls[1].OnChartMouseTrack;
            chartControls[0].chartView1.ChartMouseTrackEvent += chartControls[2].OnChartMouseTrack;
            #endregion

            #region load initialization file, create if needed
            LoadInitFile();
            #endregion

            #region get list of files in folder to watch (for new files to process)
            if (Directory.Exists(FolderToWatch))
            {
                String[] files = Directory.GetFiles(FolderToWatch);
                foreach (String file in files)
                {
                    FolderToWatchFiles.Add(file, file);
                }
            }
            checkAutoAddTimer.Interval = 30000;
            checkAutoAddTimer.Start();
            #endregion
        }
        #endregion

        #region read various log file formats
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        private void ReadTXTFile(String fileName)
        {
            String inputStr;
            int runIdx = 0;
            float priorLatVal = 0.0F;
            float priorLongVal = 0.0F;
            float latVal = 0.0F;
            float longVal = 0.0F;
            float gX = 0.0F;
            float gY = 0.0F;
            float gZ = 0.0F;
            ulong timestamp = 0;
            ulong timestampOffset = 0;
            float timestampSeconds = 0.0F;
            float mph = 0;
            float heading = 0;
            bool timestampOffsetValid = false;
            bool gpsDistanceValid = false;
            StringBuilder strRunsList = new StringBuilder();

            #region create a temp file to write cleaned up data stream
            String tempLogFile = fileName.Replace(".txt", ".tmp");
            tempLogFile = tempLogFile.Replace(".TXT", ".TMP");
            StreamReader readLog = new StreamReader(fileName, true);
            StreamWriter writeLog = new StreamWriter(tempLogFile, false);
            String tmp_text = readLog.ReadToEnd();// readFile.ReadToEnd();
            StringBuilder gpx_text = new StringBuilder();
            foreach (char c in tmp_text)
            {
                if ((c != 0x01) && (c != 0x11) && (c != 0x0C))
                {
                    writeLog.Write(c);
                    gpx_text.Append(c);
                }
            }
            readLog.Close();
            writeLog.Close();
            #endregion

            String[] splitStr;
            String runName = GetFileName(fileName, false);

            StreamReader readTemp = new StreamReader(tempLogFile, true);
            while (!readTemp.EndOfStream)
            {
                #region skip blanks
                inputStr = readTemp.ReadLine();
                if (inputStr.Length == 0)
                {
                    continue;
                }
                #endregion
                #region run start, add a new run to logger
                // found run start, create new data list in log events
                //                  new run data header
                //                  new display header
                if (String.Compare(inputStr, "Start", true) == 0)
                {
                    gpsDistanceValid = false;
                    gpsDist = 0.0F;
                    timestampOffsetValid = false;
                    timestampOffset = 0;
                    dataLogger.runData.Add(new RunData(runName));
                    runIdx = dataLogger.runData.Count - 1;
                    // set run file name in run data
                    dataLogger.runData[runIdx].fileName = System.IO.Path.GetFullPath(fileName);
                    dataLogger.runData[runIdx].runName = runName;
                    // add timestamp here, since it is always present
                    dataLogger.runData[runIdx].AddChannel("Time", "Timestamp", "Internal", runName, 1.0F);
                    continue;
                }
                #endregion
                #region run end, skip just a marker
                else if ((String.Compare(inputStr, "Stop", true) == 0) ||
                         inputStr.StartsWith("GPS") ||
                         inputStr.StartsWith("Accel") ||
                         inputStr.StartsWith("Team Yamura"))
                {
                    continue;
                }
                #endregion
                #region break up the input string
                splitStr = inputStr.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                #endregion
                #region add timestamp
                timestamp = (ulong)BitConverter.ToUInt32(BitConverter.GetBytes(Convert.ToInt32(splitStr[0])), 0);
                if (!timestampOffsetValid)
                {
                    timestampOffset = timestamp;
                    timestampOffsetValid = true;
                }
                timestamp -= timestampOffset;
                timestampSeconds = Convert.ToSingle(timestamp) / 1000000.0F;
                dataLogger.runData[runIdx].channels["Time"].AddPoint(timestampSeconds, timestampSeconds);
                #endregion
                #region GPS data
                // gps+accel form - 11 fields
                // 35990532 07/11/2019 12:44:39.00 42.446449 -83.456070  0.70    183.920000  7   0.306   0.005   0.947
                //
                // gps only - 8 fields
                // 35990532 07/11/2019 12:44:39.00 42.446449 -83.456070  0.70    183.920000  7
                //
                // accel only - 4 fields
                //36050376 0.41   0.00    0.94
                // timestamp is always first

                // gps only, gps+accelerometer - get gps portion
                if ((splitStr.Count() == 8) || (splitStr.Count() == 11))
                {
                    latVal = Convert.ToSingle(splitStr[3]);
                    longVal = Convert.ToSingle(splitStr[4]);
                    mph = Convert.ToSingle(splitStr[5]);
                    heading = Convert.ToSingle(splitStr[6]);
                    if (dataLogger.runData[runIdx].dateStr.Length == 0)
                    {
                        dataLogger.runData[runIdx].dateStr = splitStr[1];
                        dataLogger.runData[runIdx].timeStr = splitStr[2];
                    }
                    // add channel (only if needed)
                    dataLogger.runData[runIdx].AddChannel("Latitude", "GPS Latitude", "GPS", runName, 1.0F);
                    dataLogger.runData[runIdx].AddChannel("Longitude", "GPS Longitude", "GPS", runName, 1.0F);
                    dataLogger.runData[runIdx].AddChannel("Speed-GPS", "GPS Speed", "GPS", runName, 1.0F);
                    dataLogger.runData[runIdx].AddChannel("Heading-GPS", "GPS Heading", "GPS", runName, 1.0F);
                    dataLogger.runData[runIdx].AddChannel("Distance-GPS", "GPS Distance", "GPS", runName, 1.0F);
                    // add data to channel
                    dataLogger.runData[runIdx].channels["Latitude"].AddPoint(timestampSeconds, latVal);
                    dataLogger.runData[runIdx].channels["Longitude"].AddPoint(timestampSeconds, longVal);
                    dataLogger.runData[runIdx].channels["Speed-GPS"].AddPoint(timestampSeconds, mph);
                    dataLogger.runData[runIdx].channels["Heading-GPS"].AddPoint(timestampSeconds, heading);
                    if (!gpsDistanceValid)
                    {
                        dataLogger.runData[runIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, 0.0F);
                        priorLatVal = latVal;
                        priorLongVal = longVal;
                        gpsDistanceValid = true;
                    }
                    else
                    {
                        gpsDist += GPSDistance(priorLatVal, priorLongVal, latVal, longVal);
                        dataLogger.runData[runIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, gpsDist);
                        // update prior position
                        priorLatVal = latVal;
                        priorLongVal = longVal;
                    }
                }
                #endregion
                #region accelerometer data
                // accelerometer only, or gps+accelerometer - get accelerometer portion
                if ((splitStr.Count() == 4) || (splitStr.Count() == 11))
                {
                    latVal = 0;
                    longVal = 0;
                    int xValIdx = splitStr.Count() == 4 ? 1 : 8;
                    int yValIdx = splitStr.Count() == 4 ? 2 : 9;
                    int zValIdx = splitStr.Count() == 4 ? 3 : 10;

                    gX = Convert.ToSingle(splitStr[xValIdx]);
                    gY = Convert.ToSingle(splitStr[yValIdx]);
                    gZ = Convert.ToSingle(splitStr[zValIdx]);
                    // add channel (only if needed)
                    dataLogger.runData[runIdx].AddChannel("gX", "X Axis Acceleration", "IMU", runName, 1.0F);
                    dataLogger.runData[runIdx].AddChannel("gY", "Y Axis Acceleration", "IMU", runName, 1.0F);
                    dataLogger.runData[runIdx].AddChannel("gZ", "Z Axis Acceleration", "IMU", runName, 1.0F);
                    // add data to channel
                    dataLogger.runData[runIdx].channels["gX"].AddPoint(timestampSeconds, gX);
                    dataLogger.runData[runIdx].channels["gY"].AddPoint(timestampSeconds, gY);
                    dataLogger.runData[runIdx].channels["gZ"].AddPoint(timestampSeconds, gZ);

                }
                #endregion
            }
            #region close and delete temp file
            // close and delete temp file
            readTemp.Close();
            System.IO.File.Delete(tempLogFile);
            #endregion
            AddLatestDataToCharts();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        private void ReadYLGFile(String fileName)
        {
            char[] b = new char[3];
            int runIdx = 0;
            uint timeStamp = 0;
            uint timeStampOffset = 0;
            bool timeStampOffsetSet = false;
            float timestampSeconds = 0;
            float priorLatVal = 0.0F;
            float priorLongVal = 0.0F;
            bool gpsDistanceValid = false;
            StringBuilder errStr = new StringBuilder();

            String runName = GetFileName(fileName, false);
            dataLogger.runData.Add(new RunData(runName));
            runIdx = dataLogger.runData.Count - 1;

            dataLogger.runData[runIdx].AddChannel("Time", "Timestamp", "Internal", runName, 1.0F);

            dataLogger.runData[runIdx].fileName = System.IO.Path.GetFullPath(fileName);


            Cursor = Cursors.WaitCursor;
            using (BinaryReader inFile = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                // check for EOF
                while (inFile.BaseStream.Position != inFile.BaseStream.Length)
                {
                    #region read 1 char, exception on EOF
                    try
                    {
                        b[0] = (char)inFile.ReadByte();
                    }
                    catch
                    {
                        continue;
                    }
                    #endregion
                    #region timestamp
                    // 'T', next 4 bytes are a unsigned long int
                    if ((char)b[0] == 'T')
                    {
                        timeStamp = inFile.ReadUInt32();
                        if (!timeStampOffsetSet)
                        {
                            timeStampOffset = timeStamp;
                            timeStampOffsetSet = true;
                        }
                        timeStamp -= timeStampOffset;
                        timestampSeconds = (float)timeStamp / 1000000.0F;
                        dataLogger.runData[runIdx].channels["Time"].AddPoint(timestampSeconds, timestampSeconds);
                        continue;
                    }
                    #endregion
                    #region get channel type chars
                    try
                    {
                        b[1] = (char)inFile.ReadByte();
                        b[2] = (char)inFile.ReadByte();
                    }
                    catch
                    {
                        break;
                    }
                    #endregion
                    #region GPS
                    // GPS (gps device) returns NMEA strings
                    // 4 byte channel number followed by NMEA string
                    if ((b[0] == 'G') && (b[1] == 'P') && (b[2] == 'S'))
                    {
                        inFile.ReadUInt32();

                        float lat = 0.0F;
                        String ns = "";
                        float lng = 0.0F;
                        String ew = "";
                        float hd = 0.0F;
                        float speed = 0.0F;
                        int sat = 0;
                        String date = "";
                        int utcHr = 0;
                        int utcMin = 0;
                        Single utcSec = 0.0F;
                        if (ParseGPS_NMEA(inFile, out date, out utcHr, out utcMin, out utcSec, out lat, out ns, out lng, out ew, out hd, out speed, out sat, ref errStr))
                        {
                            dataLogger.runData[runIdx].AddChannel("Latitude", "GPS Latitude", "GPS", runName, 1.0F);
                            dataLogger.runData[runIdx].AddChannel("Longitude", "GPS Longitude", "GPS", runName, 1.0F);
                            dataLogger.runData[runIdx].AddChannel("Speed-GPS", "GPS Speed", "GPS", runName, 1.0F);
                            dataLogger.runData[runIdx].AddChannel("Heading-GPS", "GPS Heading", "GPS", runName, 1.0F);
                            dataLogger.runData[runIdx].AddChannel("Distance-GPS", "GPS Distance", "GPS", runName, 1.0F);
                            // add data to channel
                            dataLogger.runData[runIdx].channels["Latitude"].AddPoint(timestampSeconds, lat);
                            dataLogger.runData[runIdx].channels["Longitude"].AddPoint(timestampSeconds, lng);
                            dataLogger.runData[runIdx].channels["Speed-GPS"].AddPoint(timestampSeconds, speed);
                            dataLogger.runData[runIdx].channels["Heading-GPS"].AddPoint(timestampSeconds, hd);
                            if (!gpsDistanceValid)
                            {
                                dataLogger.runData[runIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, 0.0F);
                                priorLatVal = lat;
                                priorLongVal = lng;
                                gpsDistanceValid = true;
                            }
                            else
                            {
                                gpsDist += GPSDistance(priorLatVal, priorLongVal, lat, lng);
                                dataLogger.runData[runIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, gpsDist);
                            }
                        }
                    }
                    #endregion
                    #region ACC
                    //
                    // accel channel
                    // ACC (3 axis accelerometer) returns byte channel number followed by 3 float values
                    //
                    else if ((b[0] == 'A') && (b[1] == 'C') && (b[2] == 'C'))
                    {
                        inFile.ReadUInt32();
                        // add channel (only if needed)
                        dataLogger.runData[runIdx].AddChannel("gX", "X Axis Acceleration", "IMU", runName, 1.0F);
                        dataLogger.runData[runIdx].AddChannel("gY", "Y Axis Acceleration", "IMU", runName, 1.0F);
                        dataLogger.runData[runIdx].AddChannel("gZ", "Z Axis Acceleration", "IMU", runName, 1.0F);
                        Single accelVal = 0.0F;
                        for (int valIdx = 0; valIdx < 3; valIdx++)
                        {
                            accelVal = inFile.ReadSingle();
                            // add data to channel
                            if (valIdx == 0)
                            {
                                dataLogger.runData[runIdx].channels["gX"].AddPoint(timestampSeconds, accelVal);
                            }
                            else if (valIdx == 1)
                            {
                                dataLogger.runData[runIdx].channels["gY"].AddPoint(timestampSeconds, accelVal);
                            }
                            else if (valIdx == 2)
                            {
                                dataLogger.runData[runIdx].channels["gZ"].AddPoint(timestampSeconds, accelVal);
                            }
                        }
                    }
                    #endregion
                    #region IMU
                    // not implemented
                    #endregion
                    #region A2D/DIG/CNT/RPM
                    // 
                    // analog channel
                    // 4 byte channel number followed by 1 float (value)
                    //
                    else if ((b[0] == 'A') && (b[1] == '2') && (b[2] == 'D'))
                    {
                        uint channelNum = inFile.ReadUInt32();
                        UInt32 channelVal = inFile.ReadUInt32();
                        float channelValF = (float)channelVal;
                        String channelName = "A2D" + channelNum.ToString();
                        dataLogger.runData[runIdx].AddChannel(channelName, "Analog to Digital channel " + channelName, "A2D", runName, 1.0F);
                        dataLogger.runData[runIdx].channels[channelName].AddPoint(timestampSeconds, channelValF);

                    }
                    else
                    {
                        errStr.AppendFormat("unexpected channel type - read {0}{1}{2}", b[0], b[1], System.Environment.NewLine);
                    }
                    #endregion
                }
                inFile.Close();
            }
            Cursor = Cursors.Default;

            if (errStr.Length > 0)
            {
                FileInfo errInfo = new FileInfo();
                errInfo.FileInfoText = errStr.ToString();
                errInfo.ShowDialog();
            }
            AddLatestDataToCharts();
        }
        /// <summary>
        /// YamuraLog v7 CAN files
        /// </summary>
        /// <param name="fileName"></param>
        private void ReadYL5File(String fileName)
        {
            int runIdx = 0;
            char[] b = new char[3];
            float priorLatVal = 0.0F;
            float priorLongVal = 0.0F;
            bool gpsDistanceValid = false;
            uint absTime_uint = 0;
            float absTime = 0.0F;
            float offsetTime = -1.0F;
            float gpsDist = 0.0F;
            bool hasGPS = false;
            String channelName = "";
            List<float> timestamps = new List<float>();
            timestamps.Add(0.0F);

            StringBuilder errStr = new StringBuilder();

            String runName = GetFileName(fileName, false);

            dataLogger.runData.Add(new RunData(runName));
            runIdx = dataLogger.runData.Count - 1;
            dataLogger.runData[runIdx].AddChannel("Time", "Timestamp", "Internal", runName, 1.0F);

            dataLogger.runData[runIdx].fileName = System.IO.Path.GetFullPath(fileName);


            Cursor = Cursors.WaitCursor;
            using (BinaryReader inFile = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                while (true)
                {
                    try
                    {
                        Byte recordType = inFile.ReadByte();
                        /// HUB node (0x10)
                        /// no logged messages
                        /// Control node (0x20)
                        /// no logged messages
                        /// AD node (0x30-0x3F)
                        if ((recordType >= 0x30) && (recordType <= 0x3F))
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint((float)absTime, (float)absTime);

                            Byte digitalVals = inFile.ReadByte();
                            UInt16[] a2d = new UInt16[8];
                            #region read the digital data
                            for (int idx = 0; idx < 8; idx++)
                            {
                                channelName = "D_" + ((recordType - 0x30) + idx).ToString();
                                if (!dataLogger.runData[runIdx].channels.ContainsKey(channelName))
                                {
                                    dataLogger.runData[runIdx].AddChannel(channelName, "Digital channel " + channelName, "D", runName, 1.0F);
                                }
                                dataLogger.runData[runIdx].channels[channelName].AddPoint((float)absTime, (float)((digitalVals >> idx) & 0x01));
                            }
                            #endregion
                            #region read the a2d data
                            for (int idx = 0; idx < 8; idx++)
                            {
                                a2d[idx] = inFile.ReadUInt16();
                                channelName = "A2D_" + ((recordType - 0x30) + idx).ToString();
                                if (!dataLogger.runData[runIdx].channels.ContainsKey(channelName))
                                {
                                    dataLogger.runData[runIdx].AddChannel(channelName, "Analog to Digital channel " + channelName, "A2D", runName, 1.0F);
                                }
                                dataLogger.runData[runIdx].channels[channelName].AddPoint((float)absTime, (float)a2d[idx]);
                            }
                            #endregion
                        }
                        /// IMU/accelerometer node (0x40)
                        else if (recordType == 0x40)
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint(absTime, absTime);

                            float ax = inFile.ReadSingle();
                            float ay = inFile.ReadSingle();
                            float az = inFile.ReadSingle();
                            if (!dataLogger.runData[runIdx].channels.ContainsKey("gX"))
                            {
                                dataLogger.runData[runIdx].AddChannel("gX", "Accelerometer channel " + "gX", "IMU", runName, 1.0F);
                            }
                            if (!dataLogger.runData[runIdx].channels.ContainsKey("gY"))
                            {
                                dataLogger.runData[runIdx].AddChannel("gY", "Accelerometer channel " + "gY", "IMU", runName, 1.0F);
                            }
                            if (!dataLogger.runData[runIdx].channels.ContainsKey("gZ"))
                            {
                                dataLogger.runData[runIdx].AddChannel("gZ", "Accelerometer channel " + "gZ", "IMU", runName, 1.0F);
                            }
                            dataLogger.runData[runIdx].channels["gX"].AddPoint(absTime, ax);
                            dataLogger.runData[runIdx].channels["gY"].AddPoint(absTime, ay);
                            dataLogger.runData[runIdx].channels["gZ"].AddPoint(absTime, az);
                        }
                        /// GPS node (0x50)
                        else if (recordType == 0x50)
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint(absTime, absTime);

                            UInt16 gpsTimeYear = inFile.ReadUInt16();
                            Byte gpsTimeMonth = inFile.ReadByte();
                            Byte gpsTimeDay = inFile.ReadByte();
                            Byte gpsTimeHour = inFile.ReadByte();
                            Byte gpsTimeMinute = inFile.ReadByte();
                            Byte gpsTimeSecond = inFile.ReadByte();
                            float latitude = (float)inFile.ReadInt32() / 10000000.0F;
                            float longitude = (float)inFile.ReadInt32() / 10000000.0F;
                            float course = (float)inFile.ReadInt32();
                            float speed = ((float)inFile.ReadInt32() / 1000.0F) * 2.23694F; // convert GPS meters/sec to MPH
                            // convert meters/second to KPH
                            //float speed = ((float)inFile.ReadInt32() / 1000.0F) * 3.6F; // convert GPS meters/sec to KPH
                            Byte SIV = inFile.ReadByte();
                            if (SIV > 0)
                            {
                                if (gpsDistanceValid)
                                {
                                    float gpsStepDist = GPSDistance(priorLatVal, priorLongVal, latitude, longitude);
                                    gpsDist += gpsStepDist;
                                }
                                priorLatVal = latitude;
                                priorLongVal = longitude;
                                gpsDistanceValid = true;
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("Latitude"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("Latitude", "GPS Latitude", "GPS", runName, 1.0F);
                                }
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("Longitude"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("Longitude", "GPS Longitude", "GPS", runName, 1.0F);
                                }
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("Speed-GPS"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("Speed-GPS", "GPS Speed", "GPS", runName, 1.0F);
                                }
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("Heading-GPS"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("Heading-GPS", "GPS Heading", "GPS", runName, 1.0F);
                                }
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("Distance-GPS"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("Distance-GPS", "GPS Distance", "GPS", runName, 1.0F);
                                }
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("xDistance"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("xDistance", "Distance-Time", "Calculated", runName, 1.0F);
                                }
                                if (!dataLogger.runData[runIdx].channels.ContainsKey("Distance"))
                                {
                                    dataLogger.runData[runIdx].AddChannel("Distance", "Time-Distance", "Calculated", runName, 1.0F);
                                }
                                // add data to channels
                                dataLogger.runData[runIdx].channels["Latitude"].AddPoint(absTime, latitude);
                                dataLogger.runData[runIdx].channels["Longitude"].AddPoint(absTime, longitude);
                                dataLogger.runData[runIdx].channels["Speed-GPS"].AddPoint(absTime, speed);
                                dataLogger.runData[runIdx].channels["Heading-GPS"].AddPoint(absTime, course);
                                dataLogger.runData[runIdx].channels["Distance-GPS"].AddPoint(absTime, gpsDist);
                                hasGPS = true;
                            }
                        }
                        /// IR Tire temp node (0x60-0x6F)
                        else if ((recordType >= 0x60) && (recordType <= 0x6F))
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint((float)absTime, (float)absTime);
                        }
                        /// Shock travel (0x70-0x7F)
                        else if ((recordType >= 0x70) && (recordType <= 0x7F))
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint(absTime, absTime);
                        }
                        /// Wheel Speed node (4 groups - 0x80-0x83; 0x84-0x87; 0x88-0x8B; 0x8C-0x8F)
                        else if ((recordType >= 0x80) && (recordType <= 0x8F))
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            // conversion for 205/50R15 tires (874.18 revs/mile) with 4 magnets
                            // 1029.534994/interval
                            float interval = (float)inFile.ReadUInt32();
                            interval = (4118.139976F / 4.0F) / interval;
                            if (!(float.IsInfinity(interval)) &&
                                !(float.IsNaN(interval)))
                            {
                                channelName = "SPD_" + (recordType - 0x80).ToString();
                                if (!dataLogger.runData[runIdx].channels.ContainsKey(channelName))
                                {
                                    dataLogger.runData[runIdx].AddChannel(channelName, "Wheelspeed channel " + channelName, "SPD", runName, 1.0F);
                                }
                                dataLogger.runData[runIdx].channels["Time"].AddPoint(absTime, absTime);
                                dataLogger.runData[runIdx].channels[channelName].AddPoint(absTime, interval);
                            }
                        }
                        /// Engine RPM (0x90)
                        else if (recordType == 0x90)
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint(absTime, absTime);
                        }
                        /// CAN interface (0xA0)
                        else if (recordType == 0xA0)
                        {
                            absTime_uint = inFile.ReadUInt32();
                            absTime = (float)absTime_uint / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            dataLogger.runData[runIdx].channels["Time"].AddPoint(absTime, absTime);
                        }
                        /// unknown message
                        else
                        {
                            //outStr.AppendFormat("Unknown record type 0x{0:X02}{1}", (byte)recordType, System.Environment.NewLine);
                        }
                        /// add time to list of all timestamps for interpolation later
                        if (!timestamps.Contains(absTime))
                        {
                            timestamps.Add(absTime);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            /// interpolate for all times between GPS distance points
            /// dataLogger.runData[runIdx].channels["Distance-GPS"] has time/distance points from GPS
            /// timestamps has all timestamps in data set
            /// actual GPS distance points at 10Hz are sparse compared to sensor data
            if (hasGPS && (timestamps.Count > 1))
            {
                int[] gpsDistIdx = new int[2] { -1, 0 };
                float[] distRange = new float[3] { 0.0F, 0.0F, 0.0F };
                float[] timeRange = new float[3] { 0.0F, 0.0F, 0.0F };
                float interpolateDist = 0.0F;
                timeRange[0] = 0.0F; //dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(0).Key;
                timeRange[1] = dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(0).Key;
                timeRange[2] = timeRange[1] - timeRange[0];
                distRange[0] = 0.0F; // dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(0).Value;
                distRange[1] = dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(0).Value;
                distRange[2] = distRange[1] - distRange[0];
                // first distance/time point is 0, 0
                dataLogger.runData[runIdx].channels["xDistance"].AddPoint(0.0F, 0.0F);
                dataLogger.runData[runIdx].channels["Distance"].AddPoint(0.0F, 0.0F);
                for (int timestampIdx = 0; timestampIdx < timestamps.Count; timestampIdx++)
                {
                    // data before first GPS point
                    if (timestamps[timestampIdx] < dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(0).Key)
                    {
                        // distance is always 0 before 1st GPS point, so don't duplicate it
                        if (!dataLogger.runData[runIdx].channels["xDistance"].dataPoints.ContainsKey(0.0F))
                        {
                            dataLogger.runData[runIdx].channels["xDistance"].AddPoint(
                                                                 0.0F,
                                                                 timestamps[timestampIdx]);
                        }
                        // don't need to check for existing time point since timestamps are from original data and dont have duplicates
                        dataLogger.runData[runIdx].channels["Distance"].AddPoint(
                                                             timestamps[timestampIdx],
                                                             0.0F);
                        continue;
                    }
                    /// add known GPS point, no interpolation required
                    /// update data range for interpolation
                    else if (dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ContainsKey(timestamps[timestampIdx]))
                    {
                        if (!dataLogger.runData[runIdx].channels["xDistance"].dataPoints.ContainsKey(dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints[timestamps[timestampIdx]]))
                        {
                            dataLogger.runData[runIdx].channels["xDistance"].AddPoint(
                                                                 dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints[timestamps[timestampIdx]],
                                                                 timestamps[timestampIdx]);
                        }
                        dataLogger.runData[runIdx].channels["Distance"].AddPoint(
                                                             timestamps[timestampIdx],
                                                             dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints[timestamps[timestampIdx]]);
                        // reset gps point range for interpolation
                        gpsDistIdx[0]++;
                        if (gpsDistIdx[1] < dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.Count() - 1)
                        {
                            gpsDistIdx[1]++;
                        }
                        timeRange[0] = dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Key;
                        timeRange[1] = dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[1]).Key;
                        timeRange[2] = timeRange[1] - timeRange[0];
                        distRange[0] = dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Value;
                        distRange[1] = dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[1]).Value;
                        distRange[2] = distRange[1] - distRange[0];
                        continue;
                    }
                    /// data after end of GPS data, add last known distace
                    else if (gpsDistIdx[0] == gpsDistIdx[1])
                    {
                        if (!dataLogger.runData[runIdx].channels["xDistance"].dataPoints.ContainsKey(dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Value))
                        {
                            dataLogger.runData[runIdx].channels["xDistance"].AddPoint(
                                 dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Value,
                                 dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Key);
                        }
                        dataLogger.runData[runIdx].channels["Distance"].AddPoint(
                             dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Key,
                             dataLogger.runData[runIdx].channels["Distance-GPS"].dataPoints.ElementAt(gpsDistIdx[0]).Value);
                        continue;
                    }
                    /// time between 2 known distances - interpolate to get distance at time
                    interpolateDist = distRange[0] + (distRange[2] * ((timestamps[timestampIdx] - timeRange[0]) / timeRange[2]));
                    if (!dataLogger.runData[runIdx].channels["xDistance"].dataPoints.ContainsKey(interpolateDist))
                    {
                        dataLogger.runData[runIdx].channels["xDistance"].AddPoint(
                                                             interpolateDist,
                                                             timestamps[timestampIdx]);
                    }
                    dataLogger.runData[runIdx].channels["Distance"].AddPoint(
                                                         timestamps[timestampIdx],
                                                         interpolateDist);
                }
            }
            /// restore default cursor
            Cursor = Cursors.Default;
            /// show any errors encountered during parsing
            if (errStr.Length > 0)
            {
                FileInfo errInfo = new FileInfo();
                errInfo.FileInfoText = errStr.ToString();
                errInfo.ShowDialog();
            }
            AlignGPS();
            AlignTime();
            /// update displays with new data
            AddLatestDataToCharts();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="includeExtension"></param>
        /// <returns></returns>
        public string GetFileName(string filePath, bool includeExtension)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            if (!includeExtension)
            {
                fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            return fileName;
        }
        /// <summary>
        /// very specific NMEA parser for the output from Sparkfun QWIIC GPS breakout
        /// see the Titan datasheet for more info
        /// 
        /// GGA - Time, position and fix type data.
        /// $GPGGA -
        /// $GNGGA -
        ///
        /// GSA - GNSS receiver operating mode, active satellites used in the position solution and DOP values.
        /// $GPGSA
        /// $GLGSA
        ///
        /// GSV - The number of GPS satellites in view satellite ID numbers, elevation, azimuth, and SNR values.
        /// $GPGSV
        /// $GLGSV
        ///
        /// RMC - Time, date, position, course and speed data. The recommended minimum navigation information.
        /// $GPRMC
        /// $GNRMC
        /// 
        /// Course and speed information relative to the ground.
        /// $GPVTG
        /// $GNVTG
        /// 
        /// </summary>
        /// <param name="inFile"></param>
        public bool ParseGPS_NMEA(BinaryReader inFile, out String date, out int hr, out int min, out float sec, out float lat, out String ns, out float lng, out String ew, out float hd, out float speed, out int sat, ref StringBuilder errStr)
        {
            bool rVal = false;
            int utcHour = -1;
            int utcMin = -1;
            int utcSec = -1;
            int utcmSec = -1;
            int latDeg = -1;
            int latMin = -1;
            int latMinDecimal = -1;
            int longDeg = -1;
            int longMin = -1;
            int longMinDecimal = -1;
            int fixType = -1;
            int satellites = -1;
            Single speedKnotsPH = 0.0F;
            Single speedKmPH = 0.0F;
            Single heading = 0.0F;
            String dateStr = "";
            String nsIndication = "";
            String ewIndication = "";
            String dataValid = "";
            lat = 0.0F;
            ns = "X";
            lng = 0.0F;
            ew = "X";
            hd = 0.0F;
            speed = 0.0F;
            sat = 0;
            date = "xx/xx/xxxx";
            hr = 0;
            min = 0;
            sec = 0F;


            char c;
            String dataSentence;
            // sentence always begins with '$', ends with 0x0D
            // except when it doesn't - sometimes the '$' gets dropped....
            while ((inFile.PeekChar() == '$') || (inFile.PeekChar() == 'G'))
            {
                #region read sentence
                dataSentence = "";
                c = inFile.ReadChar();
                while (c != 0x0D)
                {
                    dataSentence += c;
                    c = (char)inFile.ReadByte();
                }
                #region checksum
                // malformed, no * 
                if (dataSentence.IndexOf('*') < 0)
                {
                    errStr.AppendFormat("malformed NMEA sentance - missing '*' {0}{1}", dataSentence, System.Environment.NewLine);
                    continue;
                }
                int receivedChecksum = 0;
                // check for malformed, illegal char in hex value
                try
                {
                    receivedChecksum = Convert.ToInt32(dataSentence.Substring(dataSentence.IndexOf('*') + 1), 16);
                }
                catch
                {
                    errStr.AppendFormat("error reading checksum from NMEA sentance {0}{1}", dataSentence, System.Environment.NewLine);
                    continue;
                }
                // calculate checksum for characters between $ and *
                int calculatedChecksum = 0;
                int charIdx = 1;
                while (dataSentence[charIdx] != '*')
                {
                    calculatedChecksum ^= Convert.ToByte(dataSentence[charIdx]);
                    charIdx++;
                }
                // bad checksum - skip this sentence
                if (calculatedChecksum != receivedChecksum)
                {
                    errStr.AppendFormat("checksum mismatch read 0x{0:X} calculated 0x{1:X} for NMEA sentance {2}{3}", receivedChecksum, calculatedChecksum, dataSentence, System.Environment.NewLine);
                    continue;
                }
                #endregion

                String[] words = dataSentence.Split(new char[] { ',' });
                #endregion
                try
                {
                    #region PARSE GGA - Time, position and fix type data.
                    if ((dataSentence.StartsWith("$GPGGA")) || // GPS
                        (dataSentence.StartsWith("$GNGGA")))   // 
                    {
                        utcHour = Convert.ToInt32(words[1].Substring(0, 2));
                        utcMin = Convert.ToInt32(words[1].Substring(2, 2));
                        utcSec = Convert.ToInt32(words[1].Substring(4, 2));
                        utcmSec = Convert.ToInt32(words[1].Substring(7, 3));

                        latDeg = Convert.ToInt32(words[2].Substring(0, 2));
                        latMin = Convert.ToInt32(words[2].Substring(2, 2));
                        latMinDecimal = Convert.ToInt32(words[2].Substring(5, 4));

                        nsIndication = words[3];

                        longDeg = Convert.ToInt32(words[4].Substring(0, 3));
                        longMin = Convert.ToInt32(words[4].Substring(3, 2));
                        longMinDecimal = Convert.ToInt32(words[4].Substring(6, 4));

                        ewIndication = words[5];

                        fixType = Convert.ToInt32(words[6]);

                        satellites = Convert.ToInt32(words[7]);
                    }
                    #endregion
                    #region PARSE RMC - Time, date, position, course and speed data. The recommended minimum navigation information.
                    else if ((dataSentence.StartsWith("$GPRMC")) || // GPS
                             (dataSentence.StartsWith("$GNRMC"))) // GNSS
                    {
                        utcHour = Convert.ToInt32(words[1].Substring(0, 2));
                        utcMin = Convert.ToInt32(words[1].Substring(2, 2));
                        utcSec = Convert.ToInt32(words[1].Substring(4, 2));
                        utcmSec = Convert.ToInt32(words[1].Substring(7, 3));

                        dataValid = words[2];

                        latDeg = Convert.ToInt32(words[3].Substring(0, 2));
                        latMin = Convert.ToInt32(words[3].Substring(2, 2));
                        latMinDecimal = Convert.ToInt32(words[3].Substring(5, 4));

                        nsIndication = words[4];

                        longDeg = Convert.ToInt32(words[5].Substring(0, 3));
                        longMin = Convert.ToInt32(words[5].Substring(3, 2));
                        longMinDecimal = Convert.ToInt32(words[5].Substring(6, 4));

                        ewIndication = words[6];

                        speedKnotsPH = Convert.ToSingle(words[7]);
                        heading = Convert.ToSingle(words[8]);
                        dateStr = words[9];
                    }
                    #endregion
                    #region PARSE VTG - Course and speed information relative to the ground.
                    else if ((dataSentence.StartsWith("$GPVTG")) || // GPS
                             (dataSentence.StartsWith("$GNVTG"))) // GNSS
                    {
                        heading = Convert.ToSingle(words[1]);
                        speedKnotsPH = Convert.ToSingle(words[5]);
                        speedKmPH = Convert.ToSingle(words[7]);
                    }
                    #endregion
                    #region SKIP: GSA - GNSS receiver operating mode, active satellites used in the position solution and DOP values.
                    else if ((dataSentence.StartsWith("$GPGSA")) || // GPS, GNSS
                             (dataSentence.StartsWith("$GLGSA"))) // GPS+GLONASS
                    {

                    }
                    #endregion
                    #region SKIP: GSV - The number of GPS satellites in view satellite ID numbers, elevation, azimuth, and SNR values.
                    else if ((dataSentence.StartsWith("$GPGSV")) || // GPS, GNSS
                             (dataSentence.StartsWith("$GLGSV"))) // GPS + GLONASS
                    {

                    }
                    #endregion
                    #region SKIP unknown/deformed sentances - ignore
                    else
                    {
                        errStr.AppendFormat("ignored unknown/deformed NMEA sentance {2}{3}", receivedChecksum, calculatedChecksum, dataSentence, System.Environment.NewLine);
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    errStr.AppendFormat("ParseNMEA error reading sentence from {0} error: {1}{2}", dataSentence, e.Message, System.Environment.NewLine);
                }
            }
            if (latDeg == -1)
            {
                rVal = false;
            }
            else
            {
                rVal = true;
                if (dateStr.Length < 6)
                {
                    errStr.AppendFormat("ParseNMEA bad date string {0}{1}", dateStr, System.Environment.NewLine);
                    date = "xx/xx/xxxx";
                }
                else
                {
                    date = dateStr.Substring(2, 2) + "/" + dateStr.Substring(0, 2) + "/20" + dateStr.Substring(4, 2);
                }
                hr = utcHour;
                min = utcMin;
                sec = (Single)utcSec + (Single)utcmSec / 1000.0F;

                lat = (Single)latDeg + ((Single)latMin + ((Single)latMinDecimal / 10000.0F)) / 60.0F;
                ns = nsIndication;
                lng = (Single)longDeg + ((Single)longMin + ((Single)longMinDecimal / 10000.0F)) / 60.0F;
                ew = ewIndication;
                hd = heading;
                if ((speedKmPH == -1.0F) && (speedKnotsPH != -1.0F))
                {
                    speedKmPH = speedKnotsPH * 1.852F;
                }
                speed = speedKmPH;
                sat = satellites;
            }
            return rVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// align most recent added data set to first data set using GPS data
        /// </summary>
        public void AlignGPS()
        {
            if ((dataLogger.runData.Count() <= 1) || (!distanceAlign))
            {
                return;
            }
            float distanceBetweenPositions = 0.0F;
            float minDistanceBetweenPositions = float.MaxValue;
            float distanceOffset = 0.0F;
            float gpsLat1;
            float gpsLong1;
            float gpsLat2;
            float gpsLong2;
            float distance1 = 0.0F;
            float distance2 = 0.0F;
            int lastRunIdx = dataLogger.runData.Count - 1;
            bool distanceOffsetSet = false;
            // GPS points from first data set
            for (int gps1Idx = 0; gps1Idx < dataLogger.runData[0].channels["Latitude"].DataPoints.Count; gps1Idx++)
            {
                gpsLat1 = dataLogger.runData[0].channels["Latitude"].DataPoints.ElementAt(gps1Idx).Value;
                gpsLong1 = dataLogger.runData[0].channels["Longitude"].DataPoints.ElementAt(gps1Idx).Value;
                distance1 = dataLogger.runData[0].channels["Distance-GPS"].DataPoints.ElementAt(gps1Idx).Value;
                // GPS points from last added data set
                for (int gps2Idx = 0; gps2Idx < dataLogger.runData[lastRunIdx].channels["Latitude"].DataPoints.Count; gps2Idx++)
                {
                    try
                    {
                        gpsLat2 = dataLogger.runData[lastRunIdx].channels["Latitude"].DataPoints.ElementAt(gps2Idx).Value;
                        gpsLong2 = dataLogger.runData[lastRunIdx].channels["Longitude"].DataPoints.ElementAt(gps2Idx).Value;
                        distance2 = dataLogger.runData[lastRunIdx].channels["Distance-GPS"].DataPoints.ElementAt(gps2Idx).Value;
                    }
                    catch
                    {
                        continue;
                    }
                    distanceBetweenPositions = GPSDistance(gpsLat1, gpsLong1, gpsLat2, gpsLong2);
                    if (distanceBetweenPositions < minDistanceBetweenPositions)
                    {
                        minDistanceBetweenPositions = distanceBetweenPositions;
                        if (minDistanceBetweenPositions == 0.0F)
                        {
                            if (!distanceOffsetSet)
                            {
                                distanceOffset = distance1 - distance2;
                                distanceOffsetSet = true;
                            }
                            break;
                        }
                    }
                }
                if (distanceOffsetSet)
                {
                    break;
                }
            }
            dataLogger.runData[lastRunIdx].DistanceOffset = distanceOffset;
        }
        /// <summary>
        /// align most recent added data set to first data set using accerlation data 
        /// first delta acceleration > threshold on defined axis is assumed to be the start of the run
        /// GPS data can't align time since there may be holds aafter the first matching location point
        /// </summary>
        public void AlignTime()
        {
            if (!timeAlign || dataLogger.runData.Count == 0)
                return;

            // find threshold-crossing time for each run
            var crossingTimes = new List<float>();
            foreach (RunData dataSet in dataLogger.runData)
            {
                float crossing = float.NaN;
                if (!dataSet.channels.ContainsKey(timeAlignChannel))
                {
                    crossingTimes.Add(float.NaN);
                    continue;
                }
                float prev = float.NaN;
                foreach (KeyValuePair<float, float> pt in dataSet.channels[timeAlignChannel].DataPoints)
                {
                    float val = pt.Value;
                    if (!float.IsNaN(prev))
                    {
                        bool crossed = timeAlignRisingEdge
                            ? (prev < timeAlignThreshold && val >= timeAlignThreshold)
                            : (prev > timeAlignThreshold && val <= timeAlignThreshold);
                        if (crossed)
                        {
                            crossing = pt.Key;
                            break;
                        }
                    }
                    prev = val;
                }
                crossingTimes.Add(crossing);
            }

            // use run 0's crossing as reference; skip runs where channel is missing or never crossed
            float reference = crossingTimes[0];
            if (float.IsNaN(reference))
                return;

            for (int i = 0; i < dataLogger.runData.Count; i++)
            {
                if (!float.IsNaN(crossingTimes[i]))
                    dataLogger.runData[i].TimeOffset = reference - crossingTimes[i];
            }
        }
        #endregion

        #region add data to charts
        public void AddLatestDataToCharts()
        {
            // no data to update
            if (dataLogger.runData.Count == 0)
            {
                return;
            }
            int runIdx = dataLogger.runData.Count - 1;
            string xAxisName = "X Axis";
            string yAxisName = "Y Axis";
            #region update display info
            RunData curRun = dataLogger.runData[dataLogger.runData.Count - 1];
            for (int chartIdx = 0; chartIdx < chartControls.Count; chartIdx++)
            {
                if (!chartControls[chartIdx].Y_Axes.ContainsKey(yAxisName))
                {
                    chartControls[chartIdx].Y_Axes.Add(yAxisName, new YamuraViewControls.Axis());
                }
                if (!chartControls[chartIdx].X_Axes.ContainsKey(xAxisName))
                {
                    chartControls[chartIdx].X_Axes.Add(xAxisName, new YamuraViewControls.Axis());
                }
                // add dataset to chart
                chartControls[chartIdx].dataSets.Add(new YamuraViewControls.DisplayDataSet(curRun.runName));
                int dataSetIdx = chartControls[chartIdx].dataSets.Count - 1;
                chartControls[chartIdx].dataSets[dataSetIdx].TimeOffset = curRun.TimeOffset;
                chartControls[chartIdx].dataSets[dataSetIdx].DistanceOffset = curRun.DistanceOffset;
                // assign channels to axes, add channels to view data set
                foreach (KeyValuePair<string, DataChannel> curChannel in curRun.channels)
                {
                    chartControls[chartIdx].dataSets[dataSetIdx].AddChannel(curChannel.Key,
                                                                            curChannel.Value.ChannelDescription,
                                                                            curChannel.Value.ChannelSource,
                                                                            curRun.runName,
                                                                            1.0F,
                                                                            curChannel.Value.dataPoints);
                    chartControls[chartIdx].dataSets[dataSetIdx].channels[curChannel.Key].ChannelColor = chartControls[chartIdx].AutoColors[(dataLogger.runData.Count - 1) % 7];
                    chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[0] = chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[0] < curChannel.Value.YRange[0] ? chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[0] : curChannel.Value.YRange[0];
                    chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[1] = chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[1] > curChannel.Value.YRange[1] ? chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[1] : curChannel.Value.YRange[1];
                    chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[2] = chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[1] - chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[0];
                    chartControls[chartIdx].Y_Axes[yAxisName].AxisDisplayRange[0] = chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[0];
                    chartControls[chartIdx].Y_Axes[yAxisName].AxisDisplayRange[1] = chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[1];
                    chartControls[chartIdx].Y_Axes[yAxisName].AxisDisplayRange[2] = chartControls[chartIdx].Y_Axes[yAxisName].AxisValueRange[2];

                    chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[0] = chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[0] < curChannel.Value.XRange[0] ? chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[0] : curChannel.Value.XRange[0];
                    chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[1] = chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[1] > curChannel.Value.XRange[1] ? chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[1] : curChannel.Value.XRange[1];
                    chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[2] = chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[1] - chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[0];
                    chartControls[chartIdx].X_Axes[xAxisName].AxisDisplayRange[0] = chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[0];
                    chartControls[chartIdx].X_Axes[xAxisName].AxisDisplayRange[1] = chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[1];
                    chartControls[chartIdx].X_Axes[xAxisName].AxisDisplayRange[2] = chartControls[chartIdx].X_Axes[xAxisName].AxisValueRange[2];

                    int associatedIdx = chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels.Count > 0 ? chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels.Count - 1 : 0;
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels.Add(chartControls[chartIdx].dataSets[dataSetIdx].channels[curChannel.Key]);
                    int associatedChannelIdx = chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels.Count - 1;
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].DataSetName = curRun.runName;
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].DataSetIndex = dataSetIdx;
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].ChannelColor = chartControls[chartIdx].AutoColors[(dataLogger.runData.Count - 1) % 7];//Color.Red/* penColors[channelIdx % penColors.Count]*/;
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].ShowChannel = false;
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].XRange[0] = curChannel.Value.XRange[0];
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].XRange[1] = curChannel.Value.XRange[1];
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].XRange[2] = curChannel.Value.XRange[1] - curChannel.Value.XRange[0];
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].YRange[0] = curChannel.Value.YRange[0];
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].YRange[1] = curChannel.Value.YRange[1];
                    chartControls[chartIdx].Y_Axes[yAxisName].AssociatedChannels[associatedChannelIdx].YRange[2] = curChannel.Value.YRange[1] - curChannel.Value.YRange[0];
                }
                chartControls[chartIdx].UpdateData();
            }
            #endregion
        }
        #endregion

        #region GPS support
        /// <summary>
        /// great circle distance between 2 lat/long points in feet
        /// </summary>
        /// <param name="lat1Deg"></param>
        /// <param name="long1Deg"></param>
        /// <param name="lat2Deg"></param>
        /// <param name="long2Deg"></param>
        /// <returns></returns>
        public float GPSDistance(float lat1Deg, float long1Deg, float lat2Deg, float long2Deg)
        {
            // This uses the ‘haversine’ formula to calculate the great - circle distance between two points 
            // the shortest distance over the earth’s surface – giving an ‘as- the - crow - flies’ distance between the points 
            // (ignoring any hills they fly over, of course!).
            // Haversine formula:	a = sin²(deltaLat / 2) + cos lat1 ⋅ cos lat2 ⋅ sin²(deltaLong / 2)
            // c = 2 ⋅ atan2( √a, √(1−a) )
            // d = R ⋅ c
            // where   'lat' is latitude, 'long' is longitude, R is earth’s radius (mean radius = 6371km);

            double R = 6371e3F; // meters
            R = R * 3.28084; // feet
            double phi1 = DegreesToRadians(lat1Deg);
            double phi2 = DegreesToRadians(lat2Deg);
            double long1 = DegreesToRadians(long1Deg);
            double long2 = DegreesToRadians(long2Deg);
            double delta_phi = phi2 - phi1;
            double delta_lambda = long2 - long1;

            double a = Math.Pow(Math.Sin(delta_phi / 2), 2) +
                       Math.Cos(phi1) * Math.Cos(phi2) *
                       Math.Pow(Math.Sin(delta_lambda / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c;
            return (float)d;
        }
        /// <summary>
        /// convert degrees to radians
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private float DegreesToRadians(double deg)
        {
            double rad = (deg * Math.PI) / 180.0;
            return (float)rad;
        }
        #endregion

        #region event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRunsMenuItem_Click(object sender, EventArgs e)
        {
            if (openLogFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            foreach (String fileName in openLogFile.FileNames)
            {
                if (fileName.EndsWith("TXT", StringComparison.CurrentCultureIgnoreCase))
                {
                    ReadTXTFile(fileName);
                    if (!FolderToWatchFiles.ContainsKey(fileName))
                    {
                        FolderToWatchFiles.Add(fileName, fileName);
                    }
                }
                else if (fileName.EndsWith("YLG", StringComparison.CurrentCultureIgnoreCase))
                {
                    ReadYLGFile(fileName);
                    if (!FolderToWatchFiles.ContainsKey(fileName))
                    {
                        FolderToWatchFiles.Add(fileName, fileName);
                    }
                }
                else if (fileName.EndsWith("YL5", StringComparison.CurrentCultureIgnoreCase))
                {
                    ReadYL5File(fileName);
                    if (!FolderToWatchFiles.ContainsKey(fileName))
                    {
                        FolderToWatchFiles.Add(fileName, fileName);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAutoAddTimer_Tick(object sender, EventArgs e)
        {
            bool loadFile = false;
            String loadFileName = "";
            #region find first unloaded file in in folder to watch
            if (Directory.Exists(FolderToWatch))
            {
                String[] files = Directory.GetFiles(FolderToWatch);
                foreach (String file in files)
                {
                    if (!FolderToWatchFiles.ContainsKey(file) &&
                        (file.EndsWith("TXT", StringComparison.CurrentCultureIgnoreCase) ||
                         file.EndsWith("YLG", StringComparison.CurrentCultureIgnoreCase) ||
                         file.EndsWith("YL5", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        loadFile = true;
                        loadFileName = file;
                        break;
                    }
                }
            }
            #endregion
            #region no new file to load, return
            if (!loadFile)
            {
                return;
            }
            #endregion
            #region load various file formats
            if (loadFileName.EndsWith("TXT", StringComparison.CurrentCultureIgnoreCase))
            {
                ReadTXTFile(loadFileName);
                FolderToWatchFiles.Add(loadFileName, loadFileName);
            }
            else if (loadFileName.EndsWith("YLG", StringComparison.CurrentCultureIgnoreCase))
            {
                ReadYLGFile(loadFileName);
                FolderToWatchFiles.Add(loadFileName, loadFileName);
            }
            else if (loadFileName.EndsWith("YL5", StringComparison.CurrentCultureIgnoreCase))
            {
                ReadYL5File(loadFileName);
                FolderToWatchFiles.Add(loadFileName, loadFileName);
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeAlignSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeAlignDialog timeAlignDialog = new TimeAlignDialog();

            // populate channel list from first run that has data
            if (dataLogger.runData.Count > 0)
                timeAlignDialog.PopulateChannels(dataLogger.runData[0].channels.Keys);

            timeAlignDialog.SetState(timeAlign, timeAlignChannel, timeAlignThreshold, timeAlignRisingEdge);

            if (timeAlignDialog.ShowDialog() != DialogResult.OK)
                return;

            timeAlign = timeAlignDialog.timeAligned;
            timeAlignChannel = timeAlignDialog.alignChannel ?? timeAlignChannel;
            timeAlignThreshold = timeAlignDialog.alignThreshold;
            timeAlignRisingEdge = timeAlignDialog.risingEdge;

            if (timeAlign)
            {
                AlignTime();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetAutoLoadFolder_Click(object sender, EventArgs e)
        {
            if (selectAutoAddFolder.ShowDialog() == DialogResult.Cancel)
            { return; }
            FolderToWatch = selectAutoAddFolder.SelectedPath;
            folderToWatchFiles.Clear();
            #region get list of files in folder to watch (for new files to process)
            if (Directory.Exists(FolderToWatch))
            {
                String[] files = Directory.GetFiles(FolderToWatch);
                foreach (String file in files)
                {
                    FolderToWatchFiles.Add(file, file);
                }
                SaveInitFile();
            }
            #endregion        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveConfigFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            XDocument setupDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                               new XElement("Setup",
                                                   new XAttribute("TimeAlign", timeAlign),
                                                   new XAttribute("TimeAlignChannel", timeAlignChannel ?? ""),
                                                   new XAttribute("TimeAlignThreshold", timeAlignThreshold),
                                                   new XAttribute("TimeAlignRisingEdge", timeAlignRisingEdge)));
            foreach (Chart curChart in chartControls)
            {
                curChart.SaveSetup(setupDoc);
            }
            setupDoc.Save(saveConfigFileDialog.FileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openConfigFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            ReadConfigFile(openConfigFileDialog.FileName);
            SaveInitFile();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void ReadConfigFile(String fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }
            XDocument setupDoc = XDocument.Load(fileName);
            XElement root = setupDoc.Element("Setup");
            timeAlign = (bool?)root?.Attribute("TimeAlign") ?? false;
            timeAlignChannel = (string)root?.Attribute("TimeAlignChannel") ?? "gX";
            timeAlignThreshold = (float?)root?.Attribute("TimeAlignThreshold") ?? 0.5f;
            timeAlignRisingEdge = (bool?)root?.Attribute("TimeAlignRisingEdge") ?? true;
            foreach (Chart curChart in chartControls)
            {
                curChart.ApplySetup(setupDoc);
            }
            SaveInitFile();
        }
        /// <summary>
        /// Load ini file containing folder to watch and layout to use
        /// </summary>
        public void LoadInitFile()
        {
            XDocument setupDoc;
            if (!File.Exists("YamuraLog.ini"))
            {
                FolderToWatch = @"C:\ftp_transfer";
                ConfigurationFile = @"C:\ftp_transfer\YamuraView.xml";
                ReadConfigFile(ConfigurationFile);
                SaveInitFile();
                return;
            }
            setupDoc = XDocument.Load("YamuraLog.ini");
            XElement root = setupDoc.Element("Setup");
            FolderToWatch = (string?)root?.Attribute("FolderToWatch") ?? @"C:\ftp_transfer";
            ConfigurationFile = (string)root?.Attribute("Config") ?? @"C:\ftp_transfer\YamuraLog.xml";
            ReadConfigFile(ConfigurationFile);
        }
        /// <summary>
        /// write the ini file
        /// </summary>
        public void SaveInitFile()
        {
            XDocument setupDoc;
            setupDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                            new XElement("Setup",
                                                new XAttribute("FolderToWatch", FolderToWatch),
                                                new XAttribute("Config", ConfigurationFile)));
            setupDoc.Save("YamuraLog.ini");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (dataLogger.runData.Count == 0)
            {
                saveConfigurationToolStripMenuItem.Enabled = false;
                loadConfigurationToolStripMenuItem.Enabled = true;
            }
            else
            {
                saveConfigurationToolStripMenuItem.Enabled = true;
                loadConfigurationToolStripMenuItem.Enabled = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveInitFile();
            Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YamuraView_SizeChanged(object sender, EventArgs e)
        {
            Invalidate(true);
        }
        #endregion

    }
}

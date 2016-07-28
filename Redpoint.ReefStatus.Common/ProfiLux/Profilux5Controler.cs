// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Profilux5Controller.cs" company="Redpoint Apps">
//   2009
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;

    using RedPoint.ReefStatus.Common.UI;

    /// <summary>
    /// Profilux Version 5 Controller FW
    /// </summary>
    public class Profilux5Controller : ProfiluxController, IProfilux
    {
        #region Constants and Fields

        /// <summary>
        /// The blockitem s_1 t o 10 vint.
        /// </summary>
        public const int Blockitems1To10Vint = 10;

        /// <summary>
        /// The blockitem s_ illuminationchannel.
        /// </summary>
        public const int BlockitemsIlluminationchannel = 8;

        /// <summary>
        /// The blockitem s_ proglogic.
        /// </summary>
        public const int BlockitemsProglogic = 8;

        /// <summary>
        /// The blockitem s_ reminder.
        /// </summary>
        public const int BlockitemsReminder = 4;

        /// <summary>
        /// The blockitem s_ sensorstates.
        /// </summary>
        public const int BlockitemsSensorstates = 8;

        /// <summary>
        /// The blockitem s_ switchplug.
        /// </summary>
        public const int BlockitemsSwitchplug = 24;

        /// <summary>
        /// The blockitem s_ timer.
        /// </summary>
        public const int BlockitemsTimer = 12;

        /// <summary>
        /// The blocksiz e_1 t o 10 vint.
        /// </summary>
        public const int Blocksize1To10Vint = 3;

        /// <summary>
        /// The blocksiz e_ illuminationchannel.
        /// </summary>
        public const int BlocksizeIlluminationchannel = 28;

        /// <summary>
        /// The blocksiz e_ proglogic.
        /// </summary>
        public const int BlocksizeProglogic = 4;

        /// <summary>
        /// The blocksiz e_ reminder.
        /// </summary>
        public const int BlocksizeReminder = 12;

        /// <summary>
        /// The blocksiz e_ sensorstates.
        /// </summary>
        public const int BlocksizeSensorstates = 8;

        /// <summary>
        /// The blocksiz e_ switchplug.
        /// </summary>
        public const int BlocksizeSwitchplug = 1;

        /// <summary>
        /// The blocksiz e_ timer.
        /// </summary>
        public const int BlocksizeTimer = 21;

        /// <summary>
        /// The megabloc k_ size.
        /// </summary>
        public const int MegablockSize = 1000;

        /// <summary>
        /// The s f_ feedpause.
        /// </summary>
        private const int SfFeedpause = 2;

        /// <summary>
        /// The s f_ maintenance.
        /// </summary>
        private const int SfMaintenance = 1;

        /// <summary>
        /// The s f_ thunderstorm.
        /// </summary>
        private const int SfThunderstorm = 3;

        /// <summary>
        /// The s f_ waterchange.
        /// </summary>
        private const int SfWaterchange = 0;

        // Regular expresson to convert file
        // \#define:b*CODE_{:i}:b*{:d+}
        // \1=\2,

        /// <summary>
        /// The digital input count.
        /// </summary>
        private int? digitalInputCount;

        /// <summary>
        /// The l port count.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", 
            Justification = "Reviewed. Suppression is OK here.")]
        private int? lPortCount;

        /// <summary>
        /// The level senosr count.
        /// </summary>
        private int? levelSenosrCount;

        /// <summary>
        /// The light count.
        /// </summary>
        private int? lightCount;

        /// <summary>
        /// The reminder count.
        /// </summary>
        private int? reminderCount;

        /// <summary>
        /// The s port count.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", 
            Justification = "Reviewed. Suppression is OK here.")]
        private int? sPortCount;

        /// <summary>
        /// The sensor count.
        /// </summary>
        private int? sensorCount;

        /// <summary>
        /// the number of timers
        /// </summary>
        private int? timerCount;

        /// <summary>
        /// The current pump count
        /// </summary>
        private int? currentPumpCount;

        /// <summary>
        /// The programmable logic count
        /// </summary>
        private int? programmableLogicCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Profilux5Controller"/> class. 
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        public Profilux5Controller(IBasicProtocol protocol)
            : base(protocol)
        {
        }

        #endregion

        #region Enums

        /// <summary>
        /// Profilux 5 codes
        /// </summary>
        public enum Code5
        {
            // Codes
            // Codes, welche nicht im EEPROM gespeichert werden, sind ab= 1000,

            // Codes für Kommunikation
            // Codes fuer allgem. Werte
            // ReSharper disable UnusedMember.Global
            // ReSharper disable InconsistentNaming

            // Codes für Kommunikation
            // Codes fuer allgem. Werte

            /// <summary>
            /// The softwareversion.
            /// </summary>
            SOFTWAREVERSION = 0, 

            /// <summary>
            /// The softwaredate.
            /// </summary>
            SOFTWAREDATE = 1, 

            /// <summary>
            /// The productid.
            /// </summary>
            PRODUCTID = 2, 

            /// <summary>
            /// The loaddefaults.
            /// </summary>
            LOADDEFAULTS = 3, 

            // xxx = 4, //frei!

            /// <summary>
            /// The address.
            /// </summary>
            ADDRESS = 5, 

            /// <summary>
            /// The serialnumber.
            /// </summary>
            SERIALNUMBER = 6, 

            // danach Platz lassen, wenn Wert ins EEPROM geschrieben werden muss (Serialnumber = ULONG)
            // 7 ist frei

            /// <summary>
            /// The memorystate.
            /// </summary>
            MEMORYSTATE = 8, 

            /// <summary>
            /// The profiluxview.
            /// </summary>
            PROFILUXVIEW = 9, 

            /// <summary>
            /// The sensorpar a 1_ ca l 1 value.
            /// </summary>
            SENSORPARA1_CAL1VALUE = 10, 

            /// <summary>
            /// The sensorpar a 1_ ca l 2 value.
            /// </summary>
            SENSORPARA1_CAL2VALUE = 11, 

            /// <summary>
            /// The sensorpar a 1_ ca l 1 adc.
            /// </summary>
            SENSORPARA1_CAL1ADC = 12, 

            /// <summary>
            /// The sensorpar a 1_ ca l 2 adc.
            /// </summary>
            SENSORPARA1_CAL2ADC = 13, 

            /// <summary>
            /// The sensorpar a 1_ alarmmode.
            /// </summary>
            SENSORPARA1_ALARMMODE = 14, 

            /// <summary>
            /// The sensorpar a 1_ desvalue.
            /// </summary>
            SENSORPARA1_DESVALUE = 15, 

            /// <summary>
            /// The sensorpar a 1_ hyst.
            /// </summary>
            SENSORPARA1_HYST = 16, 

            /// <summary>
            /// The sensorpar a 1_ ncenabled.
            /// </summary>
            SENSORPARA1_NCENABLED = 17, 

            /// <summary>
            /// The sensorpar a 1_ ncvalue.
            /// </summary>
            SENSORPARA1_NCVALUE = 18, 

            /// <summary>
            /// The sensorpar a 1_ starttime.
            /// </summary>
            SENSORPARA1_STARTTIME = 19, 

            /// <summary>
            /// The sensorpar a 1_ endtime.
            /// </summary>
            SENSORPARA1_ENDTIME = 20, 

            /// <summary>
            /// The sensorpar a 1_ enabled.
            /// </summary>
            SENSORPARA1_PROPS = 21, 

            /// <summary>
            /// The sensorpar a 1_ par a 1.
            /// </summary>
            SENSORPARA1_PARA1 = 22, 

            /// <summary>
            /// The sensorpar a 1_ par a 2.
            /// </summary>
            SENSORPARA1_PARA2 = 23, 

            /// <summary>
            /// The sensorpar a 1_ alarmdeviation.
            /// </summary>
            SENSORPARA1_ALARMDEVIATION = 24, 

            /// <summary>
            /// The sensorpar a 1_ sensortype.
            /// </summary>
            SENSORPARA1_SENSORTYPE = 25, 

            /// <summary>
            /// The sensorpar a 1_ par a 3.
            /// </summary>
            SENSORPARA1_PARA3 = 26, 

            /// <summary>
            /// The sensorpar a 1_ displaymode.
            /// </summary>
            SENSORPARA1_DISPLAYMODE = 27, 

            /// <summary>
            /// The sensorpar a 1_ pulsewidth.
            /// </summary>
            SENSORPARA1_PULSEWIDTH = 28, 

            /// <summary>
            /// The sensorpar a 1_ pausewidth.
            /// </summary>
            SENSORPARA1_PAUSEWIDTH = 29, 

            /// <summary>
            /// The sensorpar a 1_ controllingmode.
            /// </summary>
            SENSORPARA1_CONTROLLINGMODE = 30, 

            /// <summary>
            /// The sensorpar a 1_ par a 4.
            /// </summary>
            SENSORPARA1_PARA4 = 31, 

            /// <summary>
            /// The sensorpar a 1_ par a 5.
            /// </summary>
            SENSORPARA1_PARA5 = 32, 

            /// <summary>
            /// The sensorpar a 1_ signalfilter.
            /// </summary>
            SENSORPARA1_SIGNALFILTER = 33, 

            /// <summary>
            /// The sensorpar a 8_ signalfilter.
            /// </summary>
            SENSORPARA8_SIGNALFILTER = 201, 

            // Codes Dimmkanäle (28 pro Dimmkanal, 8 Dimmkanäle)

            /// <summary>
            /// The illuminatio n 1_ simulationmask.
            /// </summary>
            ILLUMINATION1_SIMULATIONMASK = 202, 

            /// <summary>
            /// The illuminatio n 1_ burninduration.
            /// </summary>
            ILLUMINATION1_BURNINDURATION = 203, 

            /// <summary>
            /// The illuminatio n 1_ props.
            /// </summary>
            ILLUMINATION1_PROPS = 204, 

            /// <summary>
            /// The illuminatio n 1_ tim e 1.
            /// </summary>
            ILLUMINATION1_TIME1 = 205, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 1.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS1 = 206, 

            /// <summary>
            /// The illuminatio n 1_ tim e 2.
            /// </summary>
            ILLUMINATION1_TIME2 = 207, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 2.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS2 = 208, 

            /// <summary>
            /// The illuminatio n 1_ tim e 3.
            /// </summary>
            ILLUMINATION1_TIME3 = 209, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 3.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS3 = 210, 

            /// <summary>
            /// The illuminatio n 1_ tim e 4.
            /// </summary>
            ILLUMINATION1_TIME4 = 211, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 4.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS4 = 212, 

            /// <summary>
            /// The illuminatio n 1_ tim e 5.
            /// </summary>
            ILLUMINATION1_TIME5 = 213, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 5.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS5 = 214, 

            /// <summary>
            /// The illuminatio n 1_ tim e 6.
            /// </summary>
            ILLUMINATION1_TIME6 = 215, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 6.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS6 = 216, 

            /// <summary>
            /// The illuminatio n 1_ tim e 7.
            /// </summary>
            ILLUMINATION1_TIME7 = 217, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 7.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS7 = 218, 

            /// <summary>
            /// The illuminatio n 1_ tim e 8.
            /// </summary>
            ILLUMINATION1_TIME8 = 219, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 8.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS8 = 220, 

            /// <summary>
            /// The illuminatio n 1_ tim e 9.
            /// </summary>
            ILLUMINATION1_TIME9 = 221, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 9.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS9 = 222, 

            /// <summary>
            /// The illuminatio n 1_ tim e 10.
            /// </summary>
            ILLUMINATION1_TIME10 = 223, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 10.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS10 = 224, 

            /// <summary>
            /// The illuminatio n 1_ tim e 11.
            /// </summary>
            ILLUMINATION1_TIME11 = 225, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 11.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS11 = 226, 

            /// <summary>
            /// The illuminatio n 1_ tim e 12.
            /// </summary>
            ILLUMINATION1_TIME12 = 227, 

            /// <summary>
            /// The illuminatio n 1_ brightnes s 12.
            /// </summary>
            ILLUMINATION1_BRIGHTNESS12 = 228, 

            /// <summary>
            /// The illuminatio n 1_ reserved.
            /// </summary>
            ILLUMINATION1_RESERVED = 229, 

            /// <summary>
            /// The illuminatio n 8_ reserved.
            /// </summary>
            ILLUMINATION8_RESERVED = 425, 

            // Codes Erinnerung (12 pro Erinnerung, 4 Erinnerungen)

            /// <summary>
            /// The me m 1_ nextmem.
            /// </summary>
            MEM1_NEXTMEM = 426, 

            /// <summary>
            /// The me m 1_ repeat.
            /// </summary>
            MEM1_REPEAT = 427, 

            /// <summary>
            /// The me m 1_ days.
            /// </summary>
            MEM1_DAYS = 428, 

            /// <summary>
            /// The me m 1_ tex t 01.
            /// </summary>
            MEM1_TEXT01 = 429, 

            /// <summary>
            /// The me m 1_ tex t 23.
            /// </summary>
            MEM1_TEXT23 = 430, 

            /// <summary>
            /// The me m 1_ tex t 45.
            /// </summary>
            MEM1_TEXT45 = 431, 

            /// <summary>
            /// The me m 1_ tex t 67.
            /// </summary>
            MEM1_TEXT67 = 432, 

            /// <summary>
            /// The me m 1_ tex t 89.
            /// </summary>
            MEM1_TEXT89 = 433, 

            /// <summary>
            /// The me m 1_ tex t 1011.
            /// </summary>
            MEM1_TEXT1011 = 434, 

            /// <summary>
            /// The me m 1_ tex t 1213.
            /// </summary>
            MEM1_TEXT1213 = 435, 

            /// <summary>
            /// The me m 1_ tex t 1415.
            /// </summary>
            MEM1_TEXT1415 = 436, 

            /// <summary>
            /// The me m 1_ reserved.
            /// </summary>
            MEM1_RESERVED = 437, 

            /// <summary>
            /// The me m 4_ reserved.
            /// </summary>
            MEM4_RESERVED = 473, 

            // Codes Zeitschaltuhren (21 pro Zeitschaltuhr, 12 Zeitschaltuhren)
            // Zeitschaltuhr 1

            /// <summary>
            /// The time r 1_ props.
            /// </summary>
            TIMER1_PROPS = 474, 

            /// <summary>
            /// The time r 1_ rateperdosing.
            /// </summary>
            TIMER1_RATEPERDOSING = 475, 

            /// <summary>
            /// The time r 1_ dayoffset.
            /// </summary>
            TIMER1_DAYOFFSET = 476, 

            /// <summary>
            /// The time r 1_ flowrate.
            /// </summary>
            TIMER1_FLOWRATE = 477, 

            /// <summary>
            /// The time r 1_ do w_ int.
            /// </summary>
            TIMER1_DOW_INT = 478, 

            /// <summary>
            /// The time r 1_ star t 1.
            /// </summary>
            TIMER1_START1 = 479, 

            /// <summary>
            /// The time r 1_ en d 1.
            /// </summary>
            TIMER1_END1 = 480, 

            /// <summary>
            /// The time r 1_ star t 2.
            /// </summary>
            TIMER1_START2 = 481, 

            /// <summary>
            /// The time r 1_ en d 2.
            /// </summary>
            TIMER1_END2 = 482, 

            /// <summary>
            /// The time r 1_ star t 3.
            /// </summary>
            TIMER1_START3 = 483, 

            /// <summary>
            /// The time r 1_ en d 3.
            /// </summary>
            TIMER1_END3 = 484, 

            /// <summary>
            /// The time r 1_ star t 4.
            /// </summary>
            TIMER1_START4 = 485, 

            /// <summary>
            /// The time r 1_ en d 4.
            /// </summary>
            TIMER1_END4 = 486, 

            /// <summary>
            /// The time r 1_ star t 5.
            /// </summary>
            TIMER1_START5 = 487, 

            /// <summary>
            /// The time r 1_ en d 5.
            /// </summary>
            TIMER1_END5 = 488, 

            /// <summary>
            /// The time r 1_ star t 6.
            /// </summary>
            TIMER1_START6 = 489, 

            /// <summary>
            /// The time r 1_ en d 6.
            /// </summary>
            TIMER1_END6 = 490, 

            /// <summary>
            /// The time r 1_ star t 7.
            /// </summary>
            TIMER1_START7 = 491, 

            /// <summary>
            /// The time r 1_ en d 7.
            /// </summary>
            TIMER1_END7 = 492, 

            /// <summary>
            /// The time r 1_ star t 8.
            /// </summary>
            TIMER1_START8 = 493, 

            /// <summary>
            /// The time r 1_ en d 8.
            /// </summary>
            TIMER1_END8 = 494, 

            /// <summary>
            /// The time r 12_ en d 8.
            /// </summary>
            TIMER12_END8 = 725, 

            // Konfiguration 1-10V-Schnittstellen

            /// <summary>
            /// The l 1 t o 10 vin t 1_ umin.
            /// </summary>
            L1TO10VINT1_UMIN = 726, 

            /// <summary>
            /// The l 1 t o 10 vin t 1_ umax.
            /// </summary>
            L1TO10VINT1_UMAX = 727, 

            /// <summary>
            /// The l 1 t o 10 vin t 1_ function.
            /// </summary>
            L1TO10VINT1_FUNCTION = 728, 

            // ...

            /// <summary>
            /// The l 1 t o 10 vin t 10_ function.
            /// </summary>
            L1TO10VINT10_FUNCTION = 755, 

            // Codes Steckdosen

            /// <summary>
            /// The switchplu g 1_ function.
            /// </summary>
            SWITCHPLUG1_FUNCTION = 756, 

            // ...

            /// <summary>
            /// The switchplu g 24_ function.
            /// </summary>
            SWITCHPLUG24_FUNCTION = 779, 

            /// <summary>
            /// The digitalpowerbaravailable.
            /// </summary>
            DIGITALPOWERBARAVAILABLE = 780, 

            // 781 - 803 frei!!!

            // Wartung

            /// <summary>
            /// The maintenanc e_ spselmas k 1.
            /// </summary>
            MAINTENANCE_SPSELMASK1 = 804, 

            /// <summary>
            /// The maintenanc e_ spselmas k 2.
            /// </summary>
            MAINTENANCE_SPSELMASK2 = 805, 

            /// <summary>
            /// The maintenanc e_ spstatemas k 1.
            /// </summary>
            MAINTENANCE_SPSTATEMASK1 = 806, 

            /// <summary>
            /// The maintenanc e_ spstatemas k 2.
            /// </summary>
            MAINTENANCE_SPSTATEMASK2 = 807, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 1.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT1 = 808, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 2.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT2 = 809, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 3.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT3 = 810, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 4.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT4 = 811, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 5.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT5 = 812, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 6.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT6 = 813, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 7.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT7 = 814, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 8.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT8 = 815, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 9.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT9 = 816, 

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 10.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT10 = 817, 

            /// <summary>
            /// The maintenanc e_ onetotenselmask.
            /// </summary>
            MAINTENANCE_ONETOTENSELMASK = 818, 

            /// <summary>
            /// The maintenanc e_ timeout.
            /// </summary>
            MAINTENANCE_TIMEOUT = 816, 

            /// <summary>
            /// The pin.
            /// </summary>
            PIN = 820, 

            /// <summary>
            /// The time.
            /// </summary>
            TIME = 821, 

            /// <summary>
            /// The date.
            /// </summary>
            DATE = 822, 

            /// <summary>
            /// The dc f_ active.
            /// </summary>
            DCF_ACTIVE = 823, 

            /// <summary>
            /// The mesmes z_ change.
            /// </summary>
            MESMESZ_CHANGE = 824, 

            /// <summary>
            /// The correctionperday.
            /// </summary>
            CORRECTIONPERDAY = 825, 

            /// <summary>
            /// The changetime.
            /// </summary>
            CHANGETIME = 826, 

            /// <summary>
            /// The userinterface.
            /// </summary>
            USERINTERFACE = 827, 

            /// <summary>
            /// The lo c_ longitude.
            /// </summary>
            LOC_LONGITUDE = 828, 

            /// <summary>
            /// The lo c_ latitude.
            /// </summary>
            LOC_LATITUDE = 829, 

            /// <summary>
            /// The alarmbeepmode.
            /// </summary>
            ALARMBEEPMODE = 830, 

            /// <summary>
            /// The alarmbeepstarttime.
            /// </summary>
            ALARMBEEPSTARTTIME = 831, 

            /// <summary>
            /// The alarmbeependtime.
            /// </summary>
            ALARMBEEPENDTIME = 832, 

            // Bank 1

            /// <summary>
            /// The ma c 0.
            /// </summary>
            MAC0 = 1820, 

            /// <summary>
            /// The ma c 1.
            /// </summary>
            MAC1 = 1821, 

            /// <summary>
            /// The ma c 2.
            /// </summary>
            MAC2 = 1822, 

            /// <summary>
            /// The ma c 3.
            /// </summary>
            MAC3 = 1823, 

            /// <summary>
            /// The ma c 4.
            /// </summary>
            MAC4 = 1824, 

            /// <summary>
            /// The ma c 5.
            /// </summary>
            MAC5 = 1825, 

            /// <summary>
            /// The pabaddress.
            /// </summary>
            PABADDRESS = 1826, 

            /// <summary>
            /// The localip.
            /// </summary>
            LOCALIP = 1827, 

            /// <summary>
            /// The defgw.
            /// </summary>
            DEFGW = 1828, 

            /// <summary>
            /// The netmask.
            /// </summary>
            NETMASK = 1829, 

            /// <summary>
            /// Web Server Port
            /// </summary>
            WEBSERVERPORT = 1830, 

            // Gewitter

            /// <summary>
            /// The thunderstor m_ intensity.
            /// </summary>
            THUNDERSTORM_INTENSITY = 833, 

            /// <summary>
            /// The thunderstor m_ darkening.
            /// </summary>
            THUNDERSTORM_DARKENING = 834, 

            /// <summary>
            /// The thunderstor m_ count.
            /// </summary>
            THUNDERSTORM_COUNT = 835, 

            /// <summary>
            /// The thunderstor m_ star t 1.
            /// </summary>
            THUNDERSTORM_START1 = 836, 

            /// <summary>
            /// The thunderstor m_ duratio n 1.
            /// </summary>
            THUNDERSTORM_DURATION1 = 837, 

            /// <summary>
            /// The thunderstor m_ star t 2.
            /// </summary>
            THUNDERSTORM_START2 = 838, 

            /// <summary>
            /// The thunderstor m_ duratio n 2.
            /// </summary>
            THUNDERSTORM_DURATION2 = 839, 

            /// <summary>
            /// The thunderstor m_ star t 3.
            /// </summary>
            THUNDERSTORM_START3 = 840, 

            /// <summary>
            /// The thunderstor m_ duratio n 3.
            /// </summary>
            THUNDERSTORM_DURATION3 = 841, 

            /// <summary>
            /// The thunderstor m_ star t 4.
            /// </summary>
            THUNDERSTORM_START4 = 842, 

            /// <summary>
            /// The thunderstor m_ duratio n 4.
            /// </summary>
            THUNDERSTORM_DURATION4 = 843, 

            /// <summary>
            /// The thunderstor m_ dow.
            /// </summary>
            THUNDERSTORM_DOW = 844, 

            /// <summary>
            /// The thunderstor m_ rndduration.
            /// </summary>
            THUNDERSTORM_RNDDURATION = 845, 

            /// <summary>
            /// The thunderstor m_ rndminwait.
            /// </summary>
            THUNDERSTORM_RNDMINWAIT = 846, 

            /// <summary>
            /// The thunderstor m_ rndmaxwait.
            /// </summary>
            THUNDERSTORM_RNDMAXWAIT = 847, 

            // temperaturabhängige Lichtreduzierung

            /// <summary>
            /// The tempdeplightre d_ illuminationchannels.
            /// </summary>
            TEMPDEPLIGHTRED_ILLUMINATIONCHANNELS = 848, 

            /// <summary>
            /// The tempdeplightre d_ deltatmin.
            /// </summary>
            TEMPDEPLIGHTRED_DELTATMIN = 849, 

            /// <summary>
            /// The tempdeplightre d_ deltatmax.
            /// </summary>
            TEMPDEPLIGHTRED_DELTATMAX = 850, 

            /// <summary>
            /// The tempdeplightre d_ sensorindex.
            /// </summary>
            TEMPDEPLIGHTRED_SENSORINDEX = 851, 

            /// <summary>
            /// The tempdeplightre d_ deltatshutoff.
            /// </summary>
            TEMPDEPLIGHTRED_DELTATSHUTOFF = 852, 

            // Anzeigeeinstellungen

            /// <summary>
            /// The displa y_ changetime.
            /// </summary>
            DISPLAY_CHANGETIME = 853, 

            /// <summary>
            /// The displa y_ showmas k 1.
            /// </summary>
            DISPLAY_SHOWMASK1 = 854, 

            /// <summary>
            /// The displa y_ showmas k 2.
            /// </summary>
            DISPLAY_SHOWMASK2 = 855, 

            /// <summary>
            /// The displa y_ datetimemode.
            /// </summary>
            DISPLAY_DATETIMEMODE = 856, 

            // Bank 1

            /// <summary>
            /// The displa y_ bright.
            /// </summary>
            DISPLAY_BRIGHT = 1853, 

            /// <summary>
            /// The displa y_ dark.
            /// </summary>
            DISPLAY_DARK = 1854, 

            /// <summary>
            /// The displa y_ darkstarttime.
            /// </summary>
            DISPLAY_DARKSTARTTIME = 1855, 

            /// <summary>
            /// The displa y_ darkendtime.
            /// </summary>
            DISPLAY_DARKENDTIME = 1856, 

            // Messwerterfassung (weitere unten)

            /// <summary>
            /// The measuremen t_ sampleperiod.
            /// </summary>
            MEASUREMENT_SAMPLEPERIOD = 857, 

            /// <summary>
            /// The measuremen t_ samplesourcemask.
            /// </summary>
            MEASUREMENT_SAMPLESOURCEMASK = 858, 

            /// <summary>
            /// The measuremen t_ maxmemorysize.
            /// </summary>
            MEASUREMENT_MAXMEMORYSIZE = 859, 

            // Stroemung

            /// <summary>
            /// The currentcontro l_ nightdecactive.
            /// </summary>
            CURRENTCONTROL_NIGHTDECACTIVE = 860, 

            /// <summary>
            /// The currentcontro l_ nightdecstart.
            /// </summary>
            CURRENTCONTROL_NIGHTDECSTART = 861, 

            /// <summary>
            /// The currentcontro l_ nightdecend.
            /// </summary>
            CURRENTCONTROL_NIGHTDECEND = 862, 

            // 2 frei

            /// <summary>
            /// The currentcontro l_ grou p 1 pumpcount.
            /// </summary>
            CURRENTCONTROL_GROUP1PUMPCOUNT = 865, 

            /// <summary>
            /// The currentcontro l_ grou p 1 mode.
            /// </summary>
            CURRENTCONTROL_GROUP1MODE = 866, 

            /// <summary>
            /// The currentcontro l_ grou p 1 minduration.
            /// </summary>
            CURRENTCONTROL_GROUP1MINDURATION = 867, 

            /// <summary>
            /// The currentcontro l_ grou p 1 maxduration.
            /// </summary>
            CURRENTCONTROL_GROUP1MAXDURATION = 868, 

            /// <summary>
            /// The currentcontro l_ grou p 1 minwaveduration.
            /// </summary>
            CURRENTCONTROL_GROUP1MINWAVEDURATION = 869, 

            /// <summary>
            /// The currentcontro l_ grou p 1 waveform.
            /// </summary>
            CURRENTCONTROL_GROUP1WAVEFORM = 870, 

            /// <summary>
            /// The currentcontro l_ grou p 1 rndreduction.
            /// </summary>
            CURRENTCONTROL_GROUP1RNDREDUCTION = 871, 

            /// <summary>
            /// The currentcontro l_ grou p 1 maxwaveduration.
            /// </summary>
            CURRENTCONTROL_GROUP1MAXWAVEDURATION = 872, 

            // 2 frei

            /// <summary>
            /// The currentcontro l_ grou p 2 pumpcount.
            /// </summary>
            CURRENTCONTROL_GROUP2PUMPCOUNT = 875, 

            /// <summary>
            /// The currentcontro l_ grou p 2 mode.
            /// </summary>
            CURRENTCONTROL_GROUP2MODE = 876, 

            /// <summary>
            /// The currentcontro l_ grou p 2 minduration.
            /// </summary>
            CURRENTCONTROL_GROUP2MINDURATION = 877, 

            /// <summary>
            /// The currentcontro l_ grou p 2 maxduration.
            /// </summary>
            CURRENTCONTROL_GROUP2MAXDURATION = 878, 

            /// <summary>
            /// The currentcontro l_ grou p 2 minwaveduration.
            /// </summary>
            CURRENTCONTROL_GROUP2MINWAVEDURATION = 879, 

            /// <summary>
            /// The currentcontro l_ grou p 2 waveform.
            /// </summary>
            CURRENTCONTROL_GROUP2WAVEFORM = 880, 

            /// <summary>
            /// The currentcontro l_ grou p 2 rndreduction.
            /// </summary>
            CURRENTCONTROL_GROUP2RNDREDUCTION = 881, 

            /// <summary>
            /// The currentcontro l_ grou p 2 maxwaveduration.
            /// </summary>
            CURRENTCONTROL_GROUP2MAXWAVEDURATION = 882, 

            // 2 frei

            /// <summary>
            /// The currentcontro l_ pum p 1 min.
            /// </summary>
            CURRENTCONTROL_PUMP1MIN = 885, 

            /// <summary>
            /// The currentcontro l_ pum p 1 max.
            /// </summary>
            CURRENTCONTROL_PUMP1MAX = 886, 

            /// <summary>
            /// The currentcontro l_ pum p 1 nightvalue.
            /// </summary>
            CURRENTCONTROL_PUMP1NIGHTVALUE = 887, 

            /// <summary>
            /// The currentcontro l_ pum p 1 thundersvalue.
            /// </summary>
            CURRENTCONTROL_PUMP1THUNDERSVALUE = 888, 

            /// <summary>
            /// The currentcontro l_ pum p 1 feedpausemode.
            /// </summary>
            CURRENTCONTROL_PUMP1FEEDPAUSEMODE = 889, 

            // 2 frei

            /// <summary>
            /// The currentcontro l_ pum p 2 min.
            /// </summary>
            CURRENTCONTROL_PUMP2MIN = 892, 

            /// <summary>
            /// The currentcontro l_ pum p 2 max.
            /// </summary>
            CURRENTCONTROL_PUMP2MAX = 893, 

            /// <summary>
            /// The currentcontro l_ pum p 2 nightvalue.
            /// </summary>
            CURRENTCONTROL_PUMP2NIGHTVALUE = 894, 

            /// <summary>
            /// The currentcontro l_ pum p 2 thundersvalue.
            /// </summary>
            CURRENTCONTROL_PUMP2THUNDERSVALUE = 895, 

            /// <summary>
            /// The currentcontro l_ pum p 2 feedpausemode.
            /// </summary>
            CURRENTCONTROL_PUMP2FEEDPAUSEMODE = 896, 

            // 2 frei

            /// <summary>
            /// The currentcontro l_ pum p 3 min.
            /// </summary>
            CURRENTCONTROL_PUMP3MIN = 899, 

            /// <summary>
            /// The currentcontro l_ pum p 3 max.
            /// </summary>
            CURRENTCONTROL_PUMP3MAX = 900, 

            /// <summary>
            /// The currentcontro l_ pum p 3 nightvalue.
            /// </summary>
            CURRENTCONTROL_PUMP3NIGHTVALUE = 901, 

            /// <summary>
            /// The currentcontro l_ pum p 3 thundersvalue.
            /// </summary>
            CURRENTCONTROL_PUMP3THUNDERSVALUE = 902, 

            /// <summary>
            /// The currentcontro l_ pum p 3 feedpausemode.
            /// </summary>
            CURRENTCONTROL_PUMP3FEEDPAUSEMODE = 903, 

            // 2 frei

            /// <summary>
            /// The currentcontro l_ pum p 4 min.
            /// </summary>
            CURRENTCONTROL_PUMP4MIN = 906, 

            /// <summary>
            /// The currentcontro l_ pum p 4 max.
            /// </summary>
            CURRENTCONTROL_PUMP4MAX = 907, 

            /// <summary>
            /// The currentcontro l_ pum p 4 nightvalue.
            /// </summary>
            CURRENTCONTROL_PUMP4NIGHTVALUE = 908, 

            /// <summary>
            /// The currentcontro l_ pum p 4 thundersvalue.
            /// </summary>
            CURRENTCONTROL_PUMP4THUNDERSVALUE = 909, 

            /// <summary>
            /// The currentcontro l_ pum p 4 feedpausemode.
            /// </summary>
            CURRENTCONTROL_PUMP4FEEDPAUSEMODE = 910, 

            // Variable Beleuchtung

            /// <summary>
            /// The variableilluminatio n 1_ l.
            /// </summary>
            VARIABLEILLUMINATION1_L = 911, 

            /// <summary>
            /// The variableilluminatio n 1_ h.
            /// </summary>
            VARIABLEILLUMINATION1_H = 912, 

            /// <summary>
            /// The variableilluminatio n 2_ l.
            /// </summary>
            VARIABLEILLUMINATION2_L = 913, 

            /// <summary>
            /// The variableilluminatio n 2_ h.
            /// </summary>
            VARIABLEILLUMINATION2_H = 914, 

            /// <summary>
            /// The variableilluminatio n 3_ l.
            /// </summary>
            VARIABLEILLUMINATION3_L = 915, 

            /// <summary>
            /// The variableilluminatio n 3_ h.
            /// </summary>
            VARIABLEILLUMINATION3_H = 916, 

            /// <summary>
            /// The variableilluminatio n 4_ l.
            /// </summary>
            VARIABLEILLUMINATION4_L = 917, 

            /// <summary>
            /// The variableilluminatio n 4_ h.
            /// </summary>
            VARIABLEILLUMINATION4_H = 918, 

            // digitale Eingänge

            /// <summary>
            /// The digitalinpu t 1_ function.
            /// </summary>
            DIGITALINPUT1_FUNCTION = 919, 

            /// <summary>
            /// The digitalinpu t 2_ function.
            /// </summary>
            DIGITALINPUT2_FUNCTION = 920, 

            /// <summary>
            /// The digitalinpu t 3_ function.
            /// </summary>
            DIGITALINPUT3_FUNCTION = 921, 

            /// <summary>
            /// The digitalinpu t 4_ function.
            /// </summary>
            DIGITALINPUT4_FUNCTION = 922, 

            // COM

            /// <summary>
            /// The co m 1_ baudrate.
            /// </summary>
            COM1_BAUDRATE = 923, 

            /// <summary>
            /// The co m 2_ baudrate.
            /// </summary>
            COM2_BAUDRATE = 924, 

            /// <summary>
            /// The co m 3_ baudrate.
            /// </summary>
            COM3_BAUDRATE = 925, 

            /// <summary>
            /// The co m 4_ baudrate.
            /// </summary>
            COM4_BAUDRATE = 926, 

            // Niveau-Regelung

            /// <summary>
            /// The leve l 1_ props.
            /// </summary>
            LEVEL1_PROPS = 927, 

            /// <summary>
            /// The leve l 1_ reactduration.
            /// </summary>
            LEVEL1_REACTDURATION = 928, 

            /// <summary>
            /// The leve l 1_ maxduration.
            /// </summary>
            LEVEL1_MAXDURATION = 929, 

            /// <summary>
            /// The leve l 2_ props.
            /// </summary>
            LEVEL2_PROPS = 930, 

            /// <summary>
            /// The leve l 2_ reactduration.
            /// </summary>
            LEVEL2_REACTDURATION = 931, 

            /// <summary>
            /// The leve l 2_ maxduration.
            /// </summary>
            LEVEL2_MAXDURATION = 932, 

            /// <summary>
            /// The leve l 3_ props.
            /// </summary>
            LEVEL3_PROPS = 933, 

            /// <summary>
            /// The leve l 3_ reactduration.
            /// </summary>
            LEVEL3_REACTDURATION = 934, 

            /// <summary>
            /// The leve l 3_ maxduration.
            /// </summary>
            LEVEL3_MAXDURATION = 935, 

            /// <summary>
            /// The feedpaus e_ duration.
            /// </summary>
            FEEDPAUSE_DURATION = 936, 

            /// <summary>
            /// The feedpaus e_ mode.
            /// </summary>
            FEEDPAUSE_MODE = 937, 

            /// <summary>
            /// The rainingday s_ dow.
            /// </summary>
            RAININGDAYS_DOW = 938, 

            /// <summary>
            /// The rainingday s_ darkening.
            /// </summary>
            RAININGDAYS_DARKENING = 939, 

            // Wolkensimulation

            /// <summary>
            /// The clou d_ probability.
            /// </summary>
            CLOUD_PROBABILITY = 940, 

            /// <summary>
            /// The clou d_ minduration.
            /// </summary>
            CLOUD_MINDURATION = 941, 

            /// <summary>
            /// The clou d_ maxduration.
            /// </summary>
            CLOUD_MAXDURATION = 942, 

            /// <summary>
            /// The clou d_ maxdarkening.
            /// </summary>
            CLOUD_MAXDARKENING = 943, 

            // Mondphasensimulation

            /// <summary>
            /// The moo n_ start.
            /// </summary>
            MOON_START = 944, 

            /// <summary>
            /// The moo n_ end.
            /// </summary>
            MOON_END = 945, 

            /// <summary>
            /// The dal i_ mindim.
            /// </summary>
            DALI_MINDIM = 946, 

            /// <summary>
            /// The aquailluminatio n_ available.
            /// </summary>
            AQUAILLUMINATION_AVAILABLE = 947, 

            /// <summary>
            /// The aquailluminatio n_ whitechannel.
            /// </summary>
            AQUAILLUMINATION_WHITECHANNEL = 948, 

            /// <summary>
            /// The aquailluminatio n_ bluechannel.
            /// </summary>
            AQUAILLUMINATION_BLUECHANNEL = 949, 

            // Codes für progr. Logik

            /// <summary>
            /// The proglogi c 1_ inpu t 1.
            /// </summary>
            PROGLOGIC1_INPUT1 = 950, 

            /// <summary>
            /// The proglogi c 1_ inpu t 2.
            /// </summary>
            PROGLOGIC1_INPUT2 = 951, 

            /// <summary>
            /// The proglogi c 1_ function.
            /// </summary>
            PROGLOGIC1_FUNCTION = 952, 

            /// <summary>
            /// The proglogi c 1_ time.
            /// </summary>
            PROGLOGIC1_TIME = 953, 

            /// <summary>
            /// The proglogi c 8_ time.
            /// </summary>
            PROGLOGIC8_TIME = 981, 

            /// <summary>
            /// The ca l 1 ad c_ tem p 1.
            /// </summary>
            CAL1ADC_TEMP1 = 982, 

            /// <summary>
            /// The ca l 2 ad c_ tem p 1.
            /// </summary>
            CAL2ADC_TEMP1 = 983, 

            // ACHTUNG: keine Codes ab inkl. 984 die im EEPROM gespeichert werden!!!
            // da kommen feste EEPROM-Adressen!!!

            // from here only volatile data (not stored in EEPROM)

            /// <summary>
            /// The sensorpar a 1_ actvalue.
            /// </summary>
            SENSORPARA1_ACTVALUE = 10000, 

            /// <summary>
            /// The sensorpar a 1_ actadc.
            /// </summary>
            SENSORPARA1_ACTADC = 10001, 

            /// <summary>
            /// The sensorpar a 1_ ohm.
            /// </summary>
            SENSORPARA1_OHM = 10002, 

            /// <summary>
            /// The sensorpar a 1_ actstate.
            /// </summary>
            SENSORPARA1_ACTSTATE = 10003, 

            /// <summary>
            /// The sensorpar a 1_ actanalogout.
            /// </summary>
            SENSORPARA1_ACTANALOGOUT = 10004, 

            /// <summary>
            /// The sensorpar a 1_ actre s 1.
            /// </summary>
            SENSORPARA1_ACTRES1 = 10005, 

            /// <summary>
            /// The sensorpar a 1_ actre s 2.
            /// </summary>
            SENSORPARA1_ACTRES2 = 10006, 

            /// <summary>
            /// The sensorpar a 1_ actre s 3.
            /// </summary>
            SENSORPARA1_ACTRES3 = 10007, 

            // ...

            /// <summary>
            /// The sensorpar a 8_ actre s 3.
            /// </summary>
            SENSORPARA8_ACTRES3 = 10063, 

            // PTC

            /// <summary>
            /// The progra m_ ptc.
            /// </summary>
            PROGRAM_PTC = 10065, 

            /// <summary>
            /// The leve l 1_ actstate.
            /// </summary>
            LEVEL1_ACTSTATE = 10070, 

            /// <summary>
            /// The leve l 2_ actstate.
            /// </summary>
            LEVEL2_ACTSTATE = 10071, 

            /// <summary>
            /// The leve l 3_ actstate.
            /// </summary>
            LEVEL3_ACTSTATE = 10072, 

            // neu seit 3.06: Akt. Werte Pumpen

            /// <summary>
            /// The currentpum p 1_ actpercent.
            /// </summary>
            CURRENTPUMP1_ACTPERCENT = 10080, 

            /// <summary>
            /// The currentpum p 2_ actpercent.
            /// </summary>
            CURRENTPUMP2_ACTPERCENT = 10081, 

            /// <summary>
            /// The currentpum p 3_ actpercent.
            /// </summary>
            CURRENTPUMP3_ACTPERCENT = 10082, 

            /// <summary>
            /// The currentpum p 4_ actpercent.
            /// </summary>
            CURRENTPUMP4_ACTPERCENT = 10083, 

            // Alarm

            /// <summary>
            /// The isalarm.
            /// </summary>
            ISALARM = 10090, 

            /// <summary>
            /// The digitalinputsstate.
            /// </summary>
            DIGITALINPUTSSTATE = 10091, 

            // Diagnose-/Inbetriebnahmeparameter

            /// <summary>
            /// The lightscenetesttime.
            /// </summary>
            LIGHTSCENETESTTIME = 10095, 

            /// <summary>
            /// The r w_ eeprom.
            /// </summary>
            RW_EEPROM = 10096, 

            /// <summary>
            /// The opmode.
            /// </summary>
            OPMODE = 10097, 

            /// <summary>
            /// The s p 1_ state.
            /// </summary>
            SP1_STATE = 10100, 

            // ...

            /// <summary>
            /// The s p 24_ state.
            /// </summary>
            SP24_STATE = 10123, 

            /// <summary>
            /// The s p_ al l_ state.
            /// </summary>
            SP_ALL_STATE = 10126,

            SP_ALL_CURRENT = 10127,

            /// <summary>
            /// The slotcount.
            /// </summary>
            SLOTCOUNT = 10129, 

            /// <summary>
            /// The ke y_ state.
            /// </summary>
            KEY_STATE = 10130, 

            /// <summary>
            /// The buzzerstate.
            /// </summary>
            BUZZERSTATE = 10131, 

            /// <summary>
            /// The alarmledstate.
            /// </summary>
            ALARMLEDSTATE = 10132, 

            /// <summary>
            /// The dcfstate.
            /// </summary>
            DCFSTATE = 10134, 

            /// <summary>
            /// The modu l 0 state.
            /// </summary>
            MODUL0STATE = 10137, 

            /// <summary>
            /// The modu l 1 state.
            /// </summary>
            MODUL1STATE = 10138, 

            /// <summary>
            /// The modu l 2 state.
            /// </summary>
            MODUL2STATE = 10139, 

            /// <summary>
            /// The rtctickcount.
            /// </summary>
            RTCTICKCOUNT = 10140, 

            /// <summary>
            /// The setlantronixconfi g_ slo t 0.
            /// </summary>
            SETLANTRONIXCONFIG_SLOT0 = 10141, 

            /// <summary>
            /// The setlantronixconfi g_ slo t 1.
            /// </summary>
            SETLANTRONIXCONFIG_SLOT1 = 10142, 

            /// <summary>
            /// The setlantronixconfi g_ slo t 2.
            /// </summary>
            SETLANTRONIXCONFIG_SLOT2 = 10143, 

            /// <summary>
            /// The setlantronixconfigsecurit y_ slo t 0.
            /// </summary>
            SETLANTRONIXCONFIGSECURITY_SLOT0 = 10144, 

            /// <summary>
            /// The setlantronixconfigsecurit y_ slo t 1.
            /// </summary>
            SETLANTRONIXCONFIGSECURITY_SLOT1 = 10145, 

            /// <summary>
            /// The setlantronixconfigsecurit y_ slo t 2.
            /// </summary>
            SETLANTRONIXCONFIGSECURITY_SLOT2 = 10146, 

            /// <summary>
            /// The sendkey.
            /// </summary>
            SENDKEY = 10150, 

            /// <summary>
            /// The getdisplaylin e 1.
            /// </summary>
            GETDISPLAYLINE1 = 10151, 

            /// <summary>
            /// The getdisplaylin e 2.
            /// </summary>
            GETDISPLAYLINE2 = 10152, 

            /// <summary>
            /// The getdisplaystate.
            /// </summary>
            GETDISPLAYSTATE = 10153, 

            /// <summary>
            /// The modu l 0 version.
            /// </summary>
            MODUL0VERSION = 10154, 

            /// <summary>
            /// The modu l 1 version.
            /// </summary>
            MODUL1VERSION = 10155, 

            /// <summary>
            /// The modu l 2 version.
            /// </summary>
            MODUL2VERSION = 10156, 

            /// <summary>
            /// The modu l 0 id.
            /// </summary>
            MODUL0ID = 10157, 

            /// <summary>
            /// The modu l 1 id.
            /// </summary>
            MODUL1ID = 10158, 

            /// <summary>
            /// The modu l 2 id.
            /// </summary>
            MODUL2ID = 10159, 

            // Messwerterfassung

            /// <summary>
            /// The measuremen t_ newdatacount.
            /// </summary>
            MEASUREMENT_NEWDATACOUNT = 10160, 

            /// <summary>
            /// The measuremen t_ usedmemorysize.
            /// </summary>
            MEASUREMENT_USEDMEMORYSIZE = 10162, 

            /// <summary>
            /// The measuremen t_ lastsampletime.
            /// </summary>
            MEASUREMENT_LASTSAMPLETIME = 10163, 

            /// <summary>
            /// The measuremen t_ lastsampledate.
            /// </summary>
            MEASUREMENT_LASTSAMPLEDATE = 10164, 

            /// <summary>
            /// The measuremen t_ validdatacount.
            /// </summary>
            MEASUREMENT_VALIDDATACOUNT = 10165, 

            /// <summary>
            /// The measuremen t_ getdatarecord.
            /// </summary>
            MEASUREMENT_GETDATARECORD = 10166, 

            /// <summary>
            /// The measuremen t_ increaddatapointer.
            /// </summary>
            MEASUREMENT_INCREADDATAPOINTER = 10167, 

            /// <summary>
            /// The l 1 t o 10 vin t 1_ pwmvalue.
            /// </summary>
            L1TO10VINT1_PWMVALUE = 10170, 

            // ...

            /// <summary>
            /// The l 1 t o 10 vin t 10_ pwmvalue.
            /// </summary>
            L1TO10VINT10_PWMVALUE = 10179, 

            // Debugging

            /// <summary>
            /// The tes t 1.
            /// </summary>
            TEST1 = 10200, 

            /// <summary>
            /// The tes t 2.
            /// </summary>
            TEST2 = 10201, 

            /// <summary>
            /// The tes t 3.
            /// </summary>
            TEST3 = 10202, 

            /// <summary>
            /// The tes t 4.
            /// </summary>
            TEST4 = 10203, 

            /// <summary>
            /// The tes t 5.
            /// </summary>
            TEST5 = 10204, 

            /// <summary>
            /// The tes t 6.
            /// </summary>
            TEST6 = 10205, 

            /// <summary>
            /// The tes t 7.
            /// </summary>
            TEST7 = 10206, 

            /// <summary>
            /// The tes t 8.
            /// </summary>
            TEST8 = 10207, 

            /// <summary>
            /// The tes t 9.
            /// </summary>
            TEST9 = 10208, 

            /// <summary>
            /// The tes t 10.
            /// </summary>
            TEST10 = 10209, 

            /// <summary>
            /// The freestack.
            /// </summary>
            FREESTACK = 10210, 

            // View

            /// <summary>
            /// The vie w_ pwmcontrast.
            /// </summary>
            VIEW_PWMCONTRAST = 10220, 

            /// <summary>
            /// The sensorpar a 1_ completestring.
            /// </summary>
            SENSORPARA1_COMPLETESTRING = 10250,

            MAINTENANCE_REMATINGTIME = 10270,

            MAINTENANCE_ISACTIVE = 10271,

            /// <summary>
            /// The sensorpar a 32_ completestring.
            /// </summary>
            SENSORPARA32_COMPLETESTRING = 10281, 

            /// <summary>
            /// The aquailluminatio n_ setbasetemp.
            /// </summary>
            AQUAILLUMINATION_SETBASETEMP = 10320, 

            /// <summary>
            /// The aquailluminatio n_ setflashint.
            /// </summary>
            AQUAILLUMINATION_SETFLASHINT = 10321,

            /// <summary>
            /// The dal i_ sendcommand.
            /// </summary>
            DALI_SENDCOMMAND = 10340, 

            /// <summary>
            /// The dal i_ getlastresponse.
            /// </summary>
            DALI_GETLASTRESPONSE = 10341, 

            ////#define BLOCKSIZE_ILLUMINATIONSTATES    4

            /// <summary>
            /// The illuminatio n 1_ actpercent.
            /// </summary>
            ILLUMINATION1_ACTPERCENT = 10350, 

            /// <summary>
            /// The illuminatio n 1_ ohm.
            /// </summary>
            ILLUMINATION1_OHM = 10351, 

            /// <summary>
            /// The illuminatio n 1_ re s 1.
            /// </summary>
            ILLUMINATION1_RES1 = 10352, 

            /// <summary>
            /// The illuminatio n 1_ re s 2.
            /// </summary>
            ILLUMINATION1_RES2 = 10353, 

            // ...

            /// <summary>
            /// The illuminatio n 8_ re s 2.
            /// </summary>
            ILLUMINATION8_RES2 = 10381, 

            /// <summary>
            /// The thunderstor m_ manuflash.
            /// </summary>
            THUNDERSTORM_MANUFLASH = 10400, 

            /// <summary>
            /// The progra m_ le d_ light.
            /// </summary>
            PROGRAM_LED_LIGHT = 10401, 

            /// <summary>
            /// The moo n_ actphase.
            /// </summary>
            MOON_ACTPHASE = 10402, 

            /// <summary>
            /// The setdigpbnumbering.
            /// </summary>
            SETDIGPBNUMBERING = 10403, 

            /// <summary>
            /// The setdigpbinistate.
            /// </summary>
            SETDIGPBINISTATE = 10404, 

            /// <summary>
            /// The invokespecialfunction.
            /// </summary>
            INVOKESPECIALFUNCTION = 10406, 

            // zweiter Code-Block - nur für ProfiLux 3

            /// <summary>
            /// The pabserialnumber.
            /// </summary>
            PABSERIALNUMBER = 10410, 

            /// <summary>
            /// The pabvalidationkey.
            /// </summary>
            PABVALIDATIONKEY = 10411, 

            /// <summary>
            /// The swmpabdirec t_ i d_ dc.
            /// </summary>
            SWMPABDIRECT_ID_DC = 10412, 

            /// <summary>
            /// The swmpabdirec t_ ra w 0.
            /// </summary>
            SWMPABDIRECT_RAW0 = 10413, 

            /// <summary>
            /// The swmpabdirec t_ ra w 1.
            /// </summary>
            SWMPABDIRECT_RAW1 = 10414, 

            /// <summary>
            /// The pa b_ reset.
            /// </summary>
            PAB_RESET = 10415, 

            /// <summary>
            /// The cantransparentmode.
            /// </summary>
            CANTRANSPARENTMODE = 10416, 

            /// <summary>
            /// The framtest.
            /// </summary>
            FRAMTEST = 10417, 

            /// <summary>
            /// The nictest.
            /// </summary>
            NICTEST = 10418, 

            DISPLAY1 = 10460,
            DISPLAY2 = 10461,
            DISPLAY3 = 10462,
            DISPLAY4 = 10463,
            DISPLAY5 = 10464,
            DISPLAY6 = 10465,
            DISPLAY7 = 10465,

            // Determine count of available resources

            /// <summary>
            /// The getsensorcount.
            /// </summary>
            GETSENSORCOUNT = 10500, 

            /// <summary>
            /// The getswitchcount.
            /// </summary>
            GETSWITCHCOUNT = 10501, 

            /// <summary>
            /// The getontetotenvintcount.
            /// </summary>
            GETONTETOTENVINTCOUNT = 10502, 

            /// <summary>
            /// The getlevelsensorcount.
            /// </summary>
            GETLEVELSENSORCOUNT = 10503, 

            /// <summary>
            /// The getserialinterfacecount.
            /// </summary>
            GETSERIALINTERFACECOUNT = 10504, 

            /// <summary>
            /// The getdigitalinputcount.
            /// </summary>
            GETDIGITALINPUTCOUNT = 10505, 

            /// <summary>
            /// The getreserve d 1 count.
            /// </summary>
            GETRESERVED1COUNT = 10506, 

            /// <summary>
            /// The getreserve d 2 count.
            /// </summary>
            GETRESERVED2COUNT = 10507, 

            /// <summary>
            /// The getreserve d 3 count.
            /// </summary>
            GETRESERVED3COUNT = 10508, 

            /// <summary>
            /// The getreserve d 4 count.
            /// </summary>
            GETRESERVED4COUNT = 10509, 

            /// <summary>
            /// The getilluminationcount.
            /// </summary>
            GETILLUMINATIONCOUNT = 10510, 

            /// <summary>
            /// The getremindercount.
            /// </summary>
            GETREMINDERCOUNT = 10511, 

            /// <summary>
            /// The gettimercount.
            /// </summary>
            GETTIMERCOUNT = 10512, 

            /// <summary>
            /// The getproglogiccount.
            /// </summary>
            GETPROGLOGICCOUNT = 10513, 

            /// <summary>
            /// The getcurrentpumpcount.
            /// </summary>
            GETCURRENTPUMPCOUNT = 10514, 

            /// <summary>
            /// The getvariableilluminationcount.
            /// </summary>
            GETVARIABLEILLUMINATIONCOUNT = 10515, 

            /// <summary>
            /// The co m_ portcount.
            /// </summary>
            COM_PORTCOUNT = 10600,

            SENSOR1_NAME = 18000,

            ILLUMINATION1_NAME = 18032,

            SWITCHPLUG1_NAME = 18064,

            /// <summary>
            /// The multiplecodeinf o_0.
            /// </summary>
            MULTIPLECODEINFO_0 = 20000, 

            /// <summary>
            /// The multiplecodeinf o_19999.
            /// </summary>
            MULTIPLECODEINFO_19999 = 39999, 

            // ReSharper restore InconsistentNaming
            // ReSharper restore UnusedMember.Global
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the alarm.
        /// </summary>
        /// <value>The alarm.</value>
        public CurrentState Alarm
        {
            get
            {
                return (CurrentState)this.Protocol.GetData((int)Code5.ISALARM);
            }
        }

        /// <summary>
        /// Gets the device address.
        /// </summary>
        /// <value>The device address.</value>
        public int DeviceAddress
        {
            get
            {
                return this.Protocol.GetData((int)Code5.ADDRESS);
            }
        }

        /// <summary>
        /// Gets the digital input count.
        /// </summary>
        /// <value>The digital input count.</value>
        public int DigitalInputCount
        {
            get
            {
                if (!this.digitalInputCount.HasValue)
                {
                    this.digitalInputCount = this.Protocol.GetData((int)Code5.GETDIGITALINPUTCOUNT);
                }

                return this.digitalInputCount.Value;
            }
        }

        /// <summary>
        /// Gets the digital input count.
        /// </summary>
        /// <value>The digital input count.</value>
        public int ProgrammableLogicCount
        {
            get
            {
                if (!this.programmableLogicCount.HasValue)
                {
                    this.programmableLogicCount = this.Protocol.GetData((int)Code5.GETPROGLOGICCOUNT);
                }

                return this.programmableLogicCount.Value;
            }
        }

        /// <summary>
        /// Gets the display image.
        /// </summary>
        /// <value>The display image.</value>
        public Image GetDisplayImage(int size)
        {
            var imageBits = new List<bool>();

            for (int i = 0; i < 8; i++)
            {
                bool[] imageArray = this.Protocol.GetDataBoolArray((int)Code5.DISPLAY1 + i);
                imageBits.AddRange(imageArray);
            }

            int x = 0;
            int y = 0;
            var image = new Bitmap(128 * size, 32 * size);
            foreach (var imageBit in imageBits)
            {
                for (var xpos = 0; xpos < size; xpos++)
                {
                    for (var ypos = 0; ypos < size; ypos++)
                    {
                        image.SetPixel(x + xpos, y + ypos, imageBit ? Color.White : Color.Blue);
                    }
                }
                
                x += size;

                if (x == (128 * size))
                {
                    x = 0;
                    y += size;
                }
            }

            return image;
        }

        /// <summary>
        /// Gets the display line1.
        /// </summary>
        /// <value>The display line1.</value>
        protected override string DisplayLine1
        {
            get
            {
                return this.Protocol.GetDataText((int)Code5.GETDISPLAYLINE1);
            }
        }

        /// <summary>
        /// Gets the display line2.
        /// </summary>
        /// <value>The display line2.</value>
        protected override string DisplayLine2
        {
            get
            {
                return this.Protocol.GetDataText((int)Code5.GETDISPLAYLINE2);
            }
        }

        /// <summary>
        /// Gets the L port count.
        /// </summary>
        /// <value>The L port count.</value>
        public int LPortCount
        {
            get
            {
                if (!this.lPortCount.HasValue)
                {
                    this.lPortCount = this.Protocol.GetData((int)Code5.GETONTETOTENVINTCOUNT);
                }

                return this.lPortCount.Value;
            }
        }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude
        {
            get
            {
                return this.Protocol.GetData((int)Code5.LOC_LATITUDE) * 0.1;
            }

            set
            {
                this.Protocol.SendData((int)Code5.LOC_LATITUDE, (int)(value / 0.1));
            }
        }

        /// <summary>
        /// Gets the level senosr count.
        /// </summary>
        /// <value>The level senosr count.</value>
        public int LevelSenosrCount
        {
            get
            {
                if (!this.levelSenosrCount.HasValue)
                {
                    this.levelSenosrCount = this.Protocol.GetData((int)Code5.GETLEVELSENSORCOUNT);
                }

                return this.levelSenosrCount.Value;
            }
        }

        /// <summary>
        /// Gets the light count.
        /// </summary>
        /// <value>The light count.</value>
        public int LightCount
        {
            get
            {
                if (!this.lightCount.HasValue)
                {
                    this.lightCount = this.Protocol.GetData((int)Code5.GETILLUMINATIONCOUNT);
                }

                return this.lightCount.Value;
            }
        }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude
        {
            get
            {
                return this.Protocol.GetData((int)Code5.LOC_LONGITUDE) * 0.1;
            }

            set
            {
                this.Protocol.SendData((int)Code5.LOC_LONGITUDE, (int)(value / 0.1));
            }
        }

        /// <summary>
        /// Gets the module count.
        /// </summary>
        /// <value>The module count.</value>
        public int ModuleCount
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Gets the moon phase.
        /// </summary>
        /// <value>The moon phase.</value>
        public double MoonPhase
        {
            get
            {
                return this.Protocol.GetData((int)Code5.MOON_ACTPHASE);
            }
        }

        /// <summary>
        /// Gets or sets the op mode.
        /// </summary>
        /// <value>The op mode.</value>
        public OperationMode OpMode
        {
            get
            {
                return (OperationMode)this.Protocol.GetData((int)Code5.OPMODE);
            }

            set
            {
                this.Protocol.SendData((int)Code5.OPMODE, (int)value);
            }
        }

        /// <summary>
        /// Gets the product id.
        /// </summary>
        /// <value>The product id.</value>
        public ProductId ProductId
        {
            get
            {
                return (ProductId)this.Protocol.GetData((int)Code5.PRODUCTID);
            }
        }

        /// <summary>
        /// Gets the reminder count.
        /// </summary>
        /// <value>The reminder count.</value>
        public int ReminderCount
        {
            get
            {
                if (!this.reminderCount.HasValue)
                {
                    this.reminderCount = this.Protocol.GetData((int)Code5.GETREMINDERCOUNT);
                }

                return this.reminderCount.Value;
            }
        }

        /// <summary>
        /// Gets the S port count.
        /// </summary>
        /// <value>The S port count.</value>
        public int SPortCount
        {
            get
            {
                if (!this.sPortCount.HasValue)
                {
                    this.sPortCount = this.Protocol.GetData((int)Code5.GETSWITCHCOUNT);
                }

                return this.sPortCount.Value;
            }
        }

        /// <summary>
        /// Gets the sensor count.
        /// </summary>
        /// <value>The sensor count.</value>
        public int SensorCount
        {
            get
            {
                if (!this.sensorCount.HasValue)
                {
                    this.sensorCount = this.Protocol.GetData((int)Code5.GETSENSORCOUNT);
                }

                return this.sensorCount.Value;
            }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        public int SerialNumber
        {
            get
            {
                return this.Protocol.GetData((int)Code5.SERIALNUMBER);
            }
        }

        /// <summary>
        /// Gets the software date.
        /// </summary>
        /// <value>The software date.</value>
        public DateTime SoftwareDate
        {
            get
            {
                return BasicProtocol.ConvertToDate(this.Protocol.GetData((int)Code5.SOFTWAREDATE));
            }
        }

        /// <summary>
        /// Gets the timer count.
        /// </summary>
        /// <value>The timer count.</value>
        public int TimerCount
        {
            get
            {
                if (!this.timerCount.HasValue)
                {
                    this.timerCount = this.Protocol.GetData((int)Code5.GETTIMERCOUNT);
                }

                return this.timerCount.Value;
            }
        }

        public int CurrentPumpCount
        {
            get
            {
                if (!this.currentPumpCount.HasValue)
                {
                    this.currentPumpCount = this.Protocol.GetData((int)Code5.GETCURRENTPUMPCOUNT);
                }

                return this.currentPumpCount.Value;
            }
        }

        /// <summary>
        /// Gets the view text line.
        /// </summary>
        /// <value>The view text line.</value>
        public override string ViewTextLine
        {
            get
            {
                return this.Protocol.GetDataText((int)Code5.PROFILUXVIEW);
            }
        }

        /// <summary>
        /// Gets the web server port.
        /// </summary>
        /// <value>The web server port.</value>
        public int WebServerPort
        {
            get
            {
                return this.Protocol.GetData((int)Code5.WEBSERVERPORT);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether [is sensor recorded] [the specified sensor index].
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is sensor recorded] [the specified sensor index]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSensorRecorded(int sensorIndex)
        {
            return (this.Protocol.GetData((int)Code5.SENSORPARA1_PROPS + GetSensorOffset(sensorIndex)) & 0x2) != 0;
        }

        #endregion

        #region Implemented Interfaces

        #region IProfilux

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void ClearLevelAlarm(int index)
        {
            this.Protocol.SendData((int)Code5.LEVEL1_ACTSTATE + GetOffset(index, 3, 1), 0);
        }

        /// <summary>
        /// Resets the reminder.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        public void ClearReminder(int reminder)
        {
            int offset = GetReminderOffset(reminder);
            this.Protocol.SendData((int)Code5.MEM1_NEXTMEM + offset, 0xFFFF);
        }

        /// <summary>
        /// Sets the Feed Pause
        /// </summary>
        /// <param name="activate">
        /// if it is active or not
        /// </param>
        public void FeedPause(bool activate)
        {
            int command = (0 << 8) | SfFeedpause;
            if (activate)
            {
                command = (1 << 8) | SfFeedpause;
            }

            this.Protocol.SendData((int)Code5.INVOKESPECIALFUNCTION, command);
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="probes">
        /// The probes.
        /// </param>
        /// <returns>
        /// a list of data points for the sensor
        /// </returns>
        public override Collection<ItemDataRow> GetDataPoints(
            IProgressCallback callback, IEnumerable<Probe> probes)
        {
            if (!this.Protocol.IsConnected)
            {
                throw new ProtocolException(500, "Unable to get data points Not Connected!");
            }

            var data = new Collection<ItemDataRow>();

            var sensorColumns = new Collection<Probe>();

            if (this.Version < 5.01)
            {
                int sourceMask = this.Protocol.GetData((int)Code5.MEASUREMENT_SAMPLESOURCEMASK);
                int columns = 0;
                int sensorIndex = 0;

                while (sourceMask != 0)
                {
                    if ((sourceMask & 0x01) == 1)
                    {
                        columns += sourceMask & 0x01;
                        var index = sensorIndex;
                        var probe = probes.FirstOrDefault(p => p.Index == index) ?? new Probe { Id = "Null" };

                        sensorColumns.Add(probe);

                        sensorIndex++;
                    }

                    sourceMask >>= 1;
                }

                if (columns == 0)
                {
                    return data;
                }
            }
            else
            {
                for (var i = 0; i < this.SensorCount; i++)
                {
                    if (!this.IsSensorRecorded(i))
                    {
                        continue;
                    }

                    var index = i;
                    var probe = probes.FirstOrDefault(p => p.Index == index) ?? new Probe { Id = "Null" };
                    sensorColumns.Add(probe);
                }
            }

            var sampleTime = this.Protocol.GetData((int)Code5.MEASUREMENT_LASTSAMPLETIME);
            var hour = sampleTime / 60;
            var minute = sampleTime % 60;

            var startTime = new DateTime(2000, 1, 1);
            var lastDataPointDate = startTime.AddDays(
                this.Protocol.GetData((int)Code5.MEASUREMENT_LASTSAMPLEDATE));
            var lastDataPointTime = new DateTime(
                lastDataPointDate.Year, lastDataPointDate.Month, lastDataPointDate.Day, hour, minute, 0);
            var newDataPoints = this.Protocol.GetData((int)Code5.MEASUREMENT_NEWDATACOUNT);
            var totalDataPoints = newDataPoints / sensorColumns.Count;

            var samplePeriod = this.Protocol.GetData((int)Code5.MEASUREMENT_SAMPLEPERIOD);
            var firstDataPointTime = lastDataPointTime.AddMinutes(-(totalDataPoints * samplePeriod));
            var datapointTime = firstDataPointTime;

            callback.Begin(0, totalDataPoints, Language.GetResource("strImport"));
            callback.SetText("Reading data points from Profilux");
            for (int i = 0; i < totalDataPoints; i++)
            {
                if (callback.IsAborting)
                {
                    break;
                }

                callback.StepTo(i);

                var values = new Collection<ItemDataRow.Item>();

                var dataPoints = this.Protocol.GetDataShortArray((int)Code5.MEASUREMENT_GETDATARECORD);

                var columnIndex = 0;
                foreach (var value in dataPoints)
                {
                    if (sensorColumns[columnIndex].Id != "Null")
                    {
                        values.Add(new ItemDataRow.Item(sensorColumns[columnIndex].ConvertFromInt(value), sensorColumns[columnIndex].Id));
                    }

                    columnIndex++;
                }

                this.Protocol.SendData((int)Code5.MEASUREMENT_INCREADDATAPOINTER, 0);

                data.Add(new ItemDataRow(datapointTime, values));
                datapointTime = datapointTime.AddMinutes(samplePeriod);
            }

            return data;
        }

        /// <summary>
        /// Gets the digital input function.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// the funtion of the digital input
        /// </returns>
        public DigitalInputFunction GetDigitalInputFunction(int sensorIndex)
        {
            int code = this.Protocol.GetData((int)Code5.DIGITALINPUT1_FUNCTION + GetOffset(sensorIndex, 4, 1));

            int function = (code >> 4) & 0xF;
            int def = code & 0xF;

            switch (function)
            {
                case 1:
                    return DigitalInputFunction.LevelSensor;
                case 2:
                    switch (def)
                    {
                        case 0:
                            return DigitalInputFunction.WaterChange;
                        case 1:
                            return DigitalInputFunction.Maintenance;
                        case 2:
                            return DigitalInputFunction.FeedingPause;
                        case 3:
                            return DigitalInputFunction.Thunderstorm;
                        default:
                            return DigitalInputFunction.NotUsed;
                    }

                default:
                    return DigitalInputFunction.NotUsed;
            }
        }

        /// <summary>
        /// Digitals the state of the input.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The current state of the input
        /// </returns>
        public CurrentState GetDigitalInputState(int index)
        {
            int state = this.Protocol.GetData((int)Code5.DIGITALINPUTSSTATE + GetOffset(index, 4, 0));
            CurrentState currentState = CurrentState.Off;
            switch (index % 4)
            {
                case 0:
                    currentState = (state & 0x1) != 0 ? CurrentState.On : CurrentState.Off;
                    break;
                case 1:
                    currentState = (state & 0x2) != 0 ? CurrentState.On : CurrentState.Off;
                    break;
                case 2:
                    currentState = (state & 0x4) != 0 ? CurrentState.On : CurrentState.Off;
                    break;
                case 3:
                    currentState = (state & 0x8) != 0 ? CurrentState.On : CurrentState.Off;
                    break;
            }

            return currentState;
        }

        /// <summary>
        /// Gets the dosing rate.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// the rate
        /// </returns>
        public int GetDosingRate(int channel)
        {
            return this.Protocol.GetData((int)Code5.TIMER1_RATEPERDOSING + GetOffset(channel, 12, 21));
        }

        /// <summary>
        /// Gets the L port function.
        /// </summary>
        /// <param name="portIndex">
        /// Index of the port.
        /// </param>
        /// <returns>
        /// LPort Funtion
        /// </returns>
        public PortMode GetLPortFunction(int portIndex)
        {
            int mode = this.Protocol.GetData((int)Code5.L1TO10VINT1_FUNCTION + GetOffset(portIndex, 10, 3));

            // mode format
            // 1234 1234
            // PPPT TTTT
            // I = Invert
            // B = Blackout time
            // P = Port Number
            // T = Type
            var portMode = new PortMode { BlackOut = mode & 0x003F };
            mode >>= 6;
            portMode.Port = (mode & 0x001F) + 1;
            mode >>= 5;
            portMode.DeviceMode = LPortModeToSocketType(mode & 0x003F);
            portMode.Invert = false;

            return portMode;
        }

        /// <summary>
        /// Gets the L port value.
        /// </summary>
        /// <param name="portIndex">
        /// Index of the port.
        /// </param>
        /// <returns>
        /// LPort Value
        /// </returns>
        public int GetLPortValue(int portIndex)
        {
            return this.Protocol.GetData((int)Code5.L1TO10VINT1_PWMVALUE + GetOffset(portIndex, 10, 1));
        }

        /// <summary>
        /// Gets the state of the current pump.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the state of the pump</returns>
        public int GetCurrentPumpValue(int index)
        {
            return this.Protocol.GetData((int)Code5.CURRENTPUMP1_ACTPERCENT + GetOffset(index, 4, 1));
        }

        /// <summary>
        /// Gets the level sensor mode.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// The Opertational Mode
        /// </returns>
        public LevelSensorOpertationMode GetLevelSensorMode(int sensorIndex)
        {
            int data = this.Protocol.GetData((int)Code5.LEVEL1_PROPS + GetOffset(sensorIndex, 3, 3));
            data >>= 13;
            return (LevelSensorOpertationMode)data;
        }

        /// <summary>
        /// Gets the state of the level sensor.
        /// </summary>
        /// <param name="levelSensorIndex">
        /// Index of the level sensor.
        /// </param>
        /// <returns>
        /// the current state of the sensor
        /// </returns>
        public LevelState GetLevelSensorState(int levelSensorIndex)
        {
            // 5432 1098 7654 3210
            // AIPD SWWW WRRR RRRR
            // A - Alarm
            // I
            // P
            // D
            // S - 
            // W - Water State
            // R - Reserverd
            var levelState = new LevelState();
            int state = this.Protocol.GetData((int)Code5.LEVEL1_ACTSTATE + GetOffset(levelSensorIndex, 3, 1));
            state >>= 7;
            levelState.WaterMode = (WaterMode)(state & 0xF);
            state >>= 4;

            ////bool switchState = (state & 0x1) == 1;
            state >>= 1;

            ////bool delyaedState = (state & 0x1) == 1;
            state >>= 1;

            ////bool previousDirectState = (state & 0x1) == 1;
            state >>= 1;
            levelState.State = (state & 0x1) == 1 ? CurrentState.On : CurrentState.Off;
            state >>= 1;
            levelState.Alarm = (state & 0x1) == 1 ? CurrentState.On : CurrentState.Off;

            return levelState;
        }

        /// <summary>
        /// Gets the light operation hours.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// &gt;The nuber of hours the light is in opeeration
        /// </returns>
        public int GetLightOperationHours(int channel)
        {
            return this.Protocol.GetData((int)Code5.ILLUMINATION1_OHM + GetOffset(channel, 8, 4));
        }

        /// <summary>
        /// Gets the light value.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// the current value of the light channel
        /// </returns>
        public double GetLightValue(int channel)
        {
            return this.Protocol.GetData((int)Code5.ILLUMINATION1_ACTPERCENT + GetOffset(channel, 8, 4));
        }

        /// <summary>
        /// Gets the module id.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// Module Id
        /// </returns>
        public int GetModuleId(int module)
        {
            return this.Protocol.GetData((int)Code5.MODUL0ID + module);
        }

        /// <summary>
        /// Gets the state of the module.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// Module State
        /// </returns>
        public int GetModuleState(int module)
        {
            return this.Protocol.GetData((int)Code5.MODUL0STATE + module);
        }

        /// <summary>
        /// Gets the module version.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// Module Version
        /// </returns>
        public int GetModuleVersion(int module)
        {
            return this.Protocol.GetData((int)Code5.MODUL0VERSION + module);
        }

        /// <summary>
        /// Gets the days to next reminder.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        /// <returns>
        /// the number of days to the next
        /// </returns>
        public DateTime? GetNextReminder(int reminder)
        {
            int offset = GetReminderOffset(reminder);
            int days = this.Protocol.GetData((int)Code5.MEM1_NEXTMEM + offset);
            if (days == 0xffff)
            {
                return null;
            }

            var nextReminder = new DateTime(2000, 1, 1);
            nextReminder = nextReminder.AddDays(days);

            return nextReminder;
        }

        /// <summary>
        /// Gets the program logic input1.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns></returns>
         public PortMode GetProgramLogicInput(int input, int portIndex)
         {
             var mode = this.Protocol.GetData((int)Code5.PROGLOGIC1_INPUT1 + input + GetOffset(portIndex, 8, 4));

            // mode format
            // 1234 1234 1234 1234
            // RRRR RRPP PPPT TTTT
            // R = Reserved
            // P = Port Number
            // T = Type
            var portMode = new PortMode();
            mode >>= 6;
            portMode.Port = (mode & 0x1F) + 1;
            mode >>= 5;
            portMode.DeviceMode = (DeviceMode)(mode & 0x1F);
            return portMode;
         }

         /// <summary>
         /// Gets the program logic function.
         /// </summary>
         /// <param name="portIndex">Index of the port.</param>
         /// <returns></returns>
         public LogicFunction GetProgramLogicFunction(int portIndex)
        {
            var mode = this.Protocol.GetData((int)Code5.PROGLOGIC1_FUNCTION + GetOffset(portIndex, 8, 4));

            // mode format
            // 1234 1234
            // RRMM MMMM
            // R = Reserved
            // P = Port Number
            // T = Type
             var invert2 = (mode & 0x1) == 1;
             mode >>= 1;
             var invert1 = (mode & 0x1) == 1;
             mode >>= 1;
             var logicMode = (LogicMode)(mode & 0x3F);
             return new LogicFunction() { Invert1 = invert1, Invert2 = invert2, LogicMode = logicMode };
        }

        /// <summary>
        /// Gets the probe operation hours.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The nuber of hours the probe is in opeeration
        /// </returns>
        public int GetProbeOperationHours(int index)
        {
            return this.Protocol.GetData((int)Code5.SENSORPARA1_OHM + GetOffset(index, 8, 8));
        }

        /// <summary>
        /// Gets the reminder period.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        /// <returns>
        /// The get reminder period.
        /// </returns>
        public int GetReminderPeriod(int reminder)
        {
            return this.Protocol.GetData((int)Code5.MEM1_DAYS + GetReminderOffset(reminder));
        }

        /// <summary>
        /// The get reminder repeats.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        /// <returns>
        /// The reminder repeats.
        /// </returns>
        public bool GetReminderRepeats(int reminder)
        {
            return this.Protocol.GetData((int)Code5.MEM1_REPEAT + GetReminderOffset(reminder)) != 0;
        }

        /// <summary>
        /// Gets the reminder text.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        /// <returns>
        /// Reminder Text
        /// </returns>
        public string GetReminderText(int reminder)
        {
            int offset = GetReminderOffset(reminder);
            string text = this.Protocol.GetDataText((int)Code5.MEM1_TEXT01 + offset);

            if (this.Version < 5.05)
            {
                try
                {
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT23 + offset);
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT45 + offset);
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT67 + offset);
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT89 + offset);
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT1011 + offset);
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT1213 + offset);
                    text += this.Protocol.GetDataText((int)Code5.MEM1_TEXT1415 + offset);
                }
                catch (ErrorCodeException)
                {
                }
            }

            return text.Trim();
        }

        /// <summary>
        /// Gets the S port function.
        /// </summary>
        /// <param name="portIndex">
        /// Index of the port.
        /// </param>
        /// <returns>
        /// the function
        /// </returns>
        public PortMode GetSPortFunction(int portIndex)
        {
            int mode = this.Protocol.GetData((int)Code5.SWITCHPLUG1_FUNCTION + GetOffset(portIndex, 24, 1));

            // mode format
            // 1234 1234 1234 1234
            // IBBB BBPP PPPT TTTT
            // I = Invert
            // B = Blackout time
            // P = Port Number
            // T = Type
            var portMode = new PortMode { Invert = (mode & 0x01) != 0 };
            mode >>= 1;
            portMode.BlackOut = mode & 0x1F;
            mode >>= 5;
            portMode.Port = (mode & 0x1F) + 1;
            mode >>= 5;
            portMode.DeviceMode = (DeviceMode)(mode & 0x1F);
            return portMode;
        }

        /// <summary>
        /// Gets the S port value.
        /// </summary>
        /// <param name="portIndex">
        /// Index of the port.
        /// </param>
        /// <returns>
        /// the value of the port
        /// </returns>
        public int GetSPortValue(int portIndex)
        {
            return this.Protocol.GetData((int)Code5.SP1_STATE + GetOffset(portIndex, 24, 1));
        }

        /// <summary>
        /// Gets the sensor active.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// If the sensor is active
        /// </returns>
        public bool GetSensorActive(int index)
        {
            if (this.Version >= 5.01)
            {
                return (this.Protocol.GetData((int)Code5.SENSORPARA1_PROPS + GetSensorOffset(index)) & 0x1) == 1;
            }

            return this.Protocol.GetData((int)Code5.SENSORPARA1_PROPS + GetSensorOffset(index)) == 1;
        }

        /// <summary>
        /// Gets the sensor alarm.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// true if the sensor is in alarm state
        /// </returns>
        public CurrentState GetSensorAlarm(int sensorIndex)
        {
            return (this.Protocol.GetData((int)Code5.SENSORPARA1_ACTSTATE + GetOffset(sensorIndex, 8, 8)) & 0x100) != 0
                       ? CurrentState.On
                       : CurrentState.Off;
        }

        /// <summary>
        /// Gets the sensor alarm deviation.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// Alarm Deviation
        /// </returns>
        public int GetSensorAlarmDeviation(int sensorIndex)
        {
            return this.Protocol.GetData((int)Code5.SENSORPARA1_ALARMDEVIATION + GetSensorOffset(sensorIndex));
        }

        /// <summary>
        /// Gets the sensor alarm enable.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// If the Alarm is Enable
        /// </returns>
        public bool GetSensorAlarmEnable(int sensorIndex)
        {
            return this.Protocol.GetData((int)Code5.SENSORPARA1_ALARMMODE + GetSensorOffset(sensorIndex)) != 0;
        }

        /// <summary>
        /// Gets the sensor format.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// Sensor Format
        /// </returns>
        public int GetSensorFormat(int sensorIndex)
        {
            return this.Protocol.GetData((int)Code5.SENSORPARA1_DISPLAYMODE + GetSensorOffset(sensorIndex)) & 0x0F;
        }

        /// <summary>
        /// Gets the sensor mode.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// the mode of the sensor
        /// </returns>
        public SensorMode GetSensorMode(int sensorIndex)
        {
            return
                (SensorMode)
                (this.Protocol.GetData((int)Code5.SENSORPARA1_CAL1ADC + GetSensorOffset(sensorIndex)) >> 12);
        }

        /// <summary>
        /// Gets the sensor nominal value.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// Nominal Value
        /// </returns>
        public int GetSensorNominalValue(int sensorIndex)
        {
            return this.Protocol.GetData((int)Code5.SENSORPARA1_DESVALUE + GetSensorOffset(sensorIndex));
        }

        /// <summary>
        /// Gets the type of the sensor.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// Sensor Type
        /// </returns>
        public SensorType GetSensorType(int sensorIndex)
        {
            return (SensorType)this.Protocol.GetData((int)Code5.SENSORPARA1_SENSORTYPE + GetSensorOffset(sensorIndex));
        }

        /// <summary>
        /// Gets the sensor value.
        /// </summary>
        /// <param name="sensorIndex">
        /// Index of the sensor.
        /// </param>
        /// <returns>
        /// Get Sensor Value
        /// </returns>
        public int GetSensorValue(int sensorIndex)
        {
            return this.Protocol.GetData((int)Code5.SENSORPARA1_ACTVALUE + GetOffset(sensorIndex, 8, 8));
        }

        /// <summary>
        /// Gets the timer mode.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// Timer settings
        /// </returns>
        public TimerSettings GetTimerSettings(int channel)
        {
            int config = this.Protocol.GetData((int)Code5.TIMER1_PROPS + GetOffset(channel, 12, 21));

            var setings = new TimerSettings
                {
                    Mode = (TimerMode)((config >> 7) & 0x7), 
                    SwitchingCount = config >> 11, 
                    FeedPauseIfActive = (config & 0x10) != 0, 
                    DayMode = (DayMode)((config >> 10) & 0x1)
                };
            return setings;
        }

        /// <summary>
        /// Determines whether [is light active] [the specified channel].
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is light active] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLightActive(int channel)
        {
            int config = this.Protocol.GetData((int)Code5.ILLUMINATION1_PROPS + GetOffset(channel, 8, 28));
            int pointCount = (config >> 4) & 0xF;
            return pointCount != 0;
        }

        /// <summary>
        /// Determines whether [is light dimmable] [the specified channel].
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is light dimmable] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLightDimmable(int channel)
        {
            int config = this.Protocol.GetData((int)Code5.ILLUMINATION1_PROPS + GetOffset(channel, 8, 28));
            return (config & 0x8) != 0;
        }

        /// <summary>
        /// Keys the command.
        /// </summary>
        /// <param name="faceplateKey">
        /// The faceplate key.
        /// </param>
        public void KeyCommand(FaceplateKey faceplateKey)
        {
            this.Protocol.SendData((int)Code5.SENDKEY, (int)faceplateKey);
        }

        /// <summary>
        /// Sets the Maintenace Mode
        /// </summary>
        /// <param name="activate">if it is to be activated or not</param>
        /// <param name="index"></param>
        public void Maintenace(bool activate, int index)
        {
            int command = (index << 16) | (0 << 8) | SfMaintenance;
            if (activate)
            {
                command = (index << 16) | (1 << 8) | SfMaintenance;
            }

            this.Protocol.SendData((int)Code5.INVOKESPECIALFUNCTION, command);
        }

        /// <summary>
        /// Resets the reminder.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        /// <param name="period">
        /// The period.
        /// </param>
        public void ResetReminder(int reminder, int period)
        {
            DateTime nextReminder = DateTime.Now;
            nextReminder = nextReminder.AddDays(period);
            var baseDate = new DateTime(2000, 1, 1);
            TimeSpan span = nextReminder - baseDate;
            var days = (int)span.TotalDays;
            int offset = GetReminderOffset(reminder);
            this.Protocol.SendData((int)Code5.MEM1_NEXTMEM + offset, days);
        }

        /// <summary>
        /// Sets the light operation hours.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetLightOperationHours(int channel, int value)
        {
            this.Protocol.SendData((int)Code5.ILLUMINATION1_OHM + GetOffset(channel, 8, 4), value);
        }

        /// <summary>
        /// Sets the state of the light.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetLightValue(int channel, double value)
        {
            this.Protocol.SendData((int)Code5.ILLUMINATION1_ACTPERCENT + GetOffset(channel, 8, 4), (int)value);
        }

        /// <summary>
        /// Sets the probe operation hours.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetProbeOperationHours(int index, int value)
        {
            this.Protocol.SendData((int)Code5.SENSORPARA1_OHM + GetOffset(index, 8, 8), value);
        }

        /// <summary>
        /// Sets the state of the socket.
        /// </summary>
        /// <param name="portNumber">
        /// The port number.
        /// </param>
        /// <param name="value">
        /// if set to <c>true</c> [value].
        /// </param>
        public void SetSocketState(int portNumber, bool value)
        {
            if (this.Version < 5.02)
            {
                var offset = (portNumber / 24) * MegablockSize;
                var index = portNumber % 24;
                var allState = this.Protocol.GetData((int)Code5.SP_ALL_STATE + offset);
                if (value)
                {
                    allState |= 1 << index;
                }
                else
                {
                    allState &= ~(1 << index);
                }

                this.Protocol.SendData((int)Code5.SP_ALL_STATE + offset, allState);
            }
            else
            {
                this.Protocol.SendData((int)Code5.SP1_STATE + GetOffset(portNumber, 24, 1), value ? 1 : 0);
            }
        }

        /// <summary>
        /// Perfoms a Thunderstorm
        /// </summary>
        /// <param name="duration">
        /// The duration.
        /// </param>
        public void Thunderstorm(int duration)
        {
            int command = (duration << 8) | SfThunderstorm;
            this.Protocol.SendData((int)Code5.INVOKESPECIALFUNCTION, command);
        }

        /// <summary>
        /// Updates the dosing rate.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="rate">
        /// The rate.
        /// </param>
        public void UpdateDosingRate(int channel, int rate)
        {
            this.Protocol.SendData((int)Code5.TIMER1_RATEPERDOSING + GetOffset(channel, 12, 21), rate);
        }

        /// <summary>
        /// Updates the timer settings.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public void UpdateTimerSettings(int channel, TimerSettings settings)
        {
            int config = settings.SwitchingCount << 11;
            config = (((int)settings.DayMode & 0x1) << 10) | config;
            config = (((int)settings.Mode & 0x7) << 7) | config;
            if (settings.FeedPauseIfActive)
            {
                config = config | 0x10;
            }

            this.Protocol.SendData((int)Code5.TIMER1_PROPS + GetOffset(channel, 12, 21), config);
        }

        /// <summary>
        /// Perfoms a Water Change
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void WaterChange(int index)
        {
            int command = index << 16 | SfWaterchange;
            this.Protocol.SendData((int)Code5.INVOKESPECIALFUNCTION, command);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="blockCount">
        /// The block count.
        /// </param>
        /// <param name="blockSize">
        /// Size of the block.
        /// </param>
        /// <returns>
        /// the offset
        /// </returns>
        private static int GetOffset(int index, int blockCount, int blockSize)
        {
            return ((index % blockCount) * blockSize) + ((index / blockCount) * MegablockSize);
        }

        /// <summary>
        /// Gets the reminder offset.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        /// <returns>
        /// the reminder offset
        /// </returns>
        private static int GetReminderOffset(int reminder)
        {
            return GetOffset(reminder, BlockitemsReminder, BlocksizeReminder);
        }

        /// <summary>
        /// Gets the sensor offset.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// the offset
        /// </returns>
        private static int GetSensorOffset(int index)
        {
            return GetOffset(index, 8, 24);
        }

        #endregion


        #region IProfilux Members


        public bool IsCurrentPumpAssinged(int i)
        {
            if (this.Version >= 5.05)
            {
                int group1Mask = this.Protocol.GetData((int)Code5.CURRENTCONTROL_GROUP1PUMPCOUNT);
                if ((group1Mask >> i & 0x1) == 1)
                {
                    return true;
                }

                int group2Mask = this.Protocol.GetData((int)Code5.CURRENTCONTROL_GROUP2PUMPCOUNT);
                if ((group2Mask >> i & 0x1) == 1)
                {
                    return true;
                }

                return false;
            }

            int totalPumps = this.Protocol.GetData((int)Code5.CURRENTCONTROL_GROUP2PUMPCOUNT) +
                             this.Protocol.GetData((int)Code5.CURRENTCONTROL_GROUP1PUMPCOUNT);
            return totalPumps > i;
        }

        /// <summary>
        /// Gets the S port current.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns></returns>
        public double GetSPortCurrent(int portIndex)
        {
            if (this.Version >= 5.05)
            {
                var dataByteArray =
                    this.Protocol.GetDataTwoByteArray((int)Code5.SP_ALL_CURRENT + (MegablockSize * (portIndex / 24)));

                if (dataByteArray.Count() == 24)
                {
                    var current = dataByteArray[portIndex % 24] * 0.1;

                    return current;
                }
            }

            return 0;
        }

        #endregion


        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public string GetLightName(int i)
        {
            if (this.Version >= 5.08)
            {
                return this.Protocol.GetDataText((int)Code5.ILLUMINATION1_NAME + GetOffset(i, 32, 1));
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the name of the light.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="text">The text.</param>
        public void SetLightName(int i, string text)
        {
            if (this.Version >= 5.08)
            {
                this.Protocol.SendText((int)Code5.ILLUMINATION1_NAME + GetOffset(i, 32, 1), text);
            }
        }

        /// <summary>
        /// Gets the name of the S port.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public string GetSPortName(int i)
        {
            if (this.Version >= 5.09)
            {
                return this.Protocol.GetDataText((int)Code5.SWITCHPLUG1_NAME + GetOffset(i, 64, 1));
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the name of the S port.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="text">The text.</param>
        public void SetSPortName(int i, string text)
        {
            if (this.Version >= 5.08)
            {
                this.Protocol.SendText((int)Code5.SWITCHPLUG1_NAME + GetOffset(i, 64, 1), text);
            }
        }

        /// <summary>
        /// Gets the name of the probe.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public string GetProbeName(int i)
        {
            if (this.Version >= 5.08)
            {
                return this.Protocol.GetDataText((int)Code5.SENSOR1_NAME + GetOffset(i, 32, 1));
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the probe.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public void SetProbeName(int i, string text)
        {
            if (this.Version >= 5.08)
            {
                this.Protocol.SendText((int)Code5.SENSOR1_NAME + GetOffset(i, 32, 1), text);
            }
        }

        /// <summary>
        /// Gets the maintenance is active.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public bool GetMaintenanceIsActive(int i)
        {
            return this.Protocol.GetData((int)Code5.MAINTENANCE_ISACTIVE + GetOffset(i, 1, 2)) == 1;
        }

        /// <summary>
        /// Gets the duration of the maintenance.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public int GetMaintenanceDuration(int i)
        {
            return this.Protocol.GetData((int)Code5.MAINTENANCE_TIMEOUT + GetOffset(i, 1, 27));
        }

        /// <summary>
        /// Gets the maintenance time left.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public int GetMaintenanceTimeLeft(int i)
        {
            return this.Protocol.GetData((int)Code5.MAINTENANCE_REMATINGTIME + GetOffset(i, 1, 2));
        }


        /// <summary>
        /// Sets the light test value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetLightTestValue(int value)
        {
            this.Protocol.SendData((int)Code5.LIGHTSCENETESTTIME, value);
        }

        /// <summary>
        /// Gets the light test time.
        /// </summary>
        public int LightTestTime
        {
            get
            {
                return this.Protocol.GetData((int)Code5.LIGHTSCENETESTTIME);
            }
        }
    }
}
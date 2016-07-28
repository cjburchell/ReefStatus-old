// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Profilux4Controler.cs" company="Redpoint Apps">
//   2009
// </copyright>
// <summary>
//   The Profilux interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using RedPoint.ReefStatus.Common.UI;

    /// <summary>
    /// Profilux 4 Controler
    /// </summary>
    public class Profilux4Controler : Protocol, IProfilux
    {
        /// <summary>
        /// The block size for reminders
        /// </summary>
        private const int ReminderBlockSize = 12;

        /// <summary>
        /// Sensor block size
        /// </summary>
        private const int SensorBlockSize = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="Profilux4Controler"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        public Profilux4Controler(IBasicProtocol protocol) : base(protocol)
        {
        }

        /// <summary>
        /// Profilux 4 codes
        /// </summary>
        private enum Code4
        {
            // Codes
            // Codes, welche nicht im EEPROM gespeichert werden, sind ab= 1000,

            // Codes für Kommunikation
            // Codes fuer allgem. Werte

            // ReSharper disable UnusedMember.Global
            // ReSharper disable InconsistentNaming

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

            /// <summary>
            /// The address.
            /// </summary>
            ADDRESS = 5,

            /// <summary>
            /// The serialnumber.
            /// </summary>
            SERIALNUMBER = 6,

            /// <summary>
            /// The profiluxview.
            /// </summary>
            PROFILUXVIEW = 9,

            // Code sensors (20 per sensor, 8 sensors) - new been added further down
            // #define BLOCKSIZE_SENSORPARA=20,

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
            SENSORPARA1_ENABLED = 21,

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

            // ...

            /// <summary>
            /// The sensorpar a 8_ pausewidth.
            /// </summary>
            SENSORPARA8_PAUSEWIDTH = 169,

            /// <summary>
            /// The time.
            /// </summary>
            TIME = 170,

            /// <summary>
            /// The date.
            /// </summary>
            DATE = 171,

            /// <summary>
            /// The dc f_ active.
            /// </summary>
            DCF_ACTIVE = 172,

            /// <summary>
            /// The mesmes z_ change.
            /// </summary>
            MESMESZ_CHANGE = 173,

            /// <summary>
            /// The changetime.
            /// </summary>
            CHANGETIME = 175,

            /// <summary>
            /// The userinterface.
            /// </summary>
            USERINTERFACE = 176,

            /// <summary>
            /// The lo c_ longitude.
            /// </summary>
            LOC_LONGITUDE = 177,

            /// <summary>
            /// The lo c_ latitude.
            /// </summary>
            LOC_LATITUDE = 178,

            // Codes Dimmkanäle (32 pro Dimmkanal, 8 Dimmkanäle)
            // #define BLOCKSIZE_ILLUMINATIONCHANNEL=32,

            /// <summary>
            /// The illuminatio n 1_ simulationmask.
            /// </summary>
            ILLUMINATION1_SIMULATIONMASK = 180,

            /// <summary>
            /// The illuminatio n 1_ nu m_ points.
            /// </summary>
            ILLUMINATION1_NUM_POINTS = 181,

            /// <summary>
            /// The illuminatio n 1_ star t 1.
            /// </summary>
            ILLUMINATION1_START1 = 182,

            /// <summary>
            /// The illuminatio n 1_ duratio n 1.
            /// </summary>
            ILLUMINATION1_DURATION1 = 183,

            /// <summary>
            /// The illuminatio n 1_ brigh t 1.
            /// </summary>
            ILLUMINATION1_BRIGHT1 = 184,

            /// <summary>
            /// The illuminatio n 1_ star t 2.
            /// </summary>
            ILLUMINATION1_START2 = 185,

            /// <summary>
            /// The illuminatio n 1_ duratio n 2.
            /// </summary>
            ILLUMINATION1_DURATION2 = 186,

            /// <summary>
            /// The illuminatio n 1_ brigh t 2.
            /// </summary>
            ILLUMINATION1_BRIGHT2 = 187,

            /// <summary>
            /// The illuminatio n 1_ star t 3.
            /// </summary>
            ILLUMINATION1_START3 = 188,

            /// <summary>
            /// The illuminatio n 1_ duratio n 3.
            /// </summary>
            ILLUMINATION1_DURATION3 = 189,

            /// <summary>
            /// The illuminatio n 1_ brigh t 3.
            /// </summary>
            ILLUMINATION1_BRIGHT3 = 190,

            /// <summary>
            /// The illuminatio n 1_ star t 4.
            /// </summary>
            ILLUMINATION1_START4 = 191,

            /// <summary>
            /// The illuminatio n 1_ duratio n 4.
            /// </summary>
            ILLUMINATION1_DURATION4 = 192,

            /// <summary>
            /// The illuminatio n 1_ brigh t 4.
            /// </summary>
            ILLUMINATION1_BRIGHT4 = 193,

            /// <summary>
            /// The illuminatio n 1_ star t 5.
            /// </summary>
            ILLUMINATION1_START5 = 194,

            /// <summary>
            /// The illuminatio n 1_ duratio n 5.
            /// </summary>
            ILLUMINATION1_DURATION5 = 195,

            /// <summary>
            /// The illuminatio n 1_ brigh t 5.
            /// </summary>
            ILLUMINATION1_BRIGHT5 = 196,

            /// <summary>
            /// The illuminatio n 1_ star t 6.
            /// </summary>
            ILLUMINATION1_START6 = 197,

            /// <summary>
            /// The illuminatio n 1_ duratio n 6.
            /// </summary>
            ILLUMINATION1_DURATION6 = 198,

            /// <summary>
            /// The illuminatio n 1_ brigh t 6.
            /// </summary>
            ILLUMINATION1_BRIGHT6 = 199,

            /// <summary>
            /// The illuminatio n 1_ star t 7.
            /// </summary>
            ILLUMINATION1_START7 = 200,

            /// <summary>
            /// The illuminatio n 1_ duratio n 7.
            /// </summary>
            ILLUMINATION1_DURATION7 = 201,

            /// <summary>
            /// The illuminatio n 1_ brigh t 7.
            /// </summary>
            ILLUMINATION1_BRIGHT7 = 202,

            /// <summary>
            /// The illuminatio n 1_ star t 8.
            /// </summary>
            ILLUMINATION1_START8 = 203,

            /// <summary>
            /// The illuminatio n 1_ duratio n 8.
            /// </summary>
            ILLUMINATION1_DURATION8 = 204,

            /// <summary>
            /// The illuminatio n 1_ brigh t 8.
            /// </summary>
            ILLUMINATION1_BRIGHT8 = 205,

            /// <summary>
            /// The illuminatio n 1_ burninduration.
            /// </summary>
            ILLUMINATION1_BURNINDURATION = 206,

            /// <summary>
            /// The illuminatio n 1_ type.
            /// </summary>
            ILLUMINATION1_TYPE = 207,

            // 208 ... 211 free

            /// <summary>
            /// The illuminatio n 2_ simulationmask.
            /// </summary>
            ILLUMINATION2_SIMULATIONMASK = 212,

            /// <summary>
            /// The illuminatio n 2_ nu m_ points.
            /// </summary>
            ILLUMINATION2_NUM_POINTS = 213,

            /// <summary>
            /// The illuminatio n 2_ star t 1.
            /// </summary>
            ILLUMINATION2_START1 = 214,

            /// <summary>
            /// The illuminatio n 2_ duratio n 1.
            /// </summary>
            ILLUMINATION2_DURATION1 = 215,

            /// <summary>
            /// The illuminatio n 2_ brigh t 1.
            /// </summary>
            ILLUMINATION2_BRIGHT1 = 216,

            /// <summary>
            /// The illuminatio n 2_ star t 2.
            /// </summary>
            ILLUMINATION2_START2 = 217,

            /// <summary>
            /// The illuminatio n 2_ duratio n 2.
            /// </summary>
            ILLUMINATION2_DURATION2 = 218,

            /// <summary>
            /// The illuminatio n 2_ brigh t 2.
            /// </summary>
            ILLUMINATION2_BRIGHT2 = 219,

            /// <summary>
            /// The illuminatio n 2_ star t 3.
            /// </summary>
            ILLUMINATION2_START3 = 220,

            /// <summary>
            /// The illuminatio n 2_ duratio n 3.
            /// </summary>
            ILLUMINATION2_DURATION3 = 221,

            /// <summary>
            /// The illuminatio n 2_ brigh t 3.
            /// </summary>
            ILLUMINATION2_BRIGHT3 = 222,

            /// <summary>
            /// The illuminatio n 2_ star t 4.
            /// </summary>
            ILLUMINATION2_START4 = 223,

            /// <summary>
            /// The illuminatio n 2_ duratio n 4.
            /// </summary>
            ILLUMINATION2_DURATION4 = 224,

            /// <summary>
            /// The illuminatio n 2_ brigh t 4.
            /// </summary>
            ILLUMINATION2_BRIGHT4 = 225,

            /// <summary>
            /// The illuminatio n 2_ star t 5.
            /// </summary>
            ILLUMINATION2_START5 = 226,

            /// <summary>
            /// The illuminatio n 2_ duratio n 5.
            /// </summary>
            ILLUMINATION2_DURATION5 = 227,

            /// <summary>
            /// The illuminatio n 2_ brigh t 5.
            /// </summary>
            ILLUMINATION2_BRIGHT5 = 228,

            /// <summary>
            /// The illuminatio n 2_ star t 6.
            /// </summary>
            ILLUMINATION2_START6 = 229,

            /// <summary>
            /// The illuminatio n 2_ duratio n 6.
            /// </summary>
            ILLUMINATION2_DURATION6 = 230,

            /// <summary>
            /// The illuminatio n 2_ brigh t 6.
            /// </summary>
            ILLUMINATION2_BRIGHT6 = 231,

            /// <summary>
            /// The illuminatio n 2_ star t 7.
            /// </summary>
            ILLUMINATION2_START7 = 232,

            /// <summary>
            /// The illuminatio n 2_ duratio n 7.
            /// </summary>
            ILLUMINATION2_DURATION7 = 233,

            /// <summary>
            /// The illuminatio n 2_ brigh t 7.
            /// </summary>
            ILLUMINATION2_BRIGHT7 = 234,

            /// <summary>
            /// The illuminatio n 2_ star t 8.
            /// </summary>
            ILLUMINATION2_START8 = 235,

            /// <summary>
            /// The illuminatio n 2_ duratio n 8.
            /// </summary>
            ILLUMINATION2_DURATION8 = 236,

            /// <summary>
            /// The illuminatio n 2_ brigh t 8.
            /// </summary>
            ILLUMINATION2_BRIGHT8 = 237,

            /// <summary>
            /// The illuminatio n 2_ burninduration.
            /// </summary>
            ILLUMINATION2_BURNINDURATION = 238,

            /// <summary>
            /// The illuminatio n 2_ type.
            /// </summary>
            ILLUMINATION2_TYPE = 239,

            // 240 ... 243 free

            /// <summary>
            /// The illuminatio n 3_ simulationmask.
            /// </summary>
            ILLUMINATION3_SIMULATIONMASK = 244,

            /// <summary>
            /// The illuminatio n 3_ nu m_ points.
            /// </summary>
            ILLUMINATION3_NUM_POINTS = 245,

            /// <summary>
            /// The illuminatio n 3_ star t 1.
            /// </summary>
            ILLUMINATION3_START1 = 246,

            /// <summary>
            /// The illuminatio n 3_ duratio n 1.
            /// </summary>
            ILLUMINATION3_DURATION1 = 247,

            /// <summary>
            /// The illuminatio n 3_ brigh t 1.
            /// </summary>
            ILLUMINATION3_BRIGHT1 = 248,

            /// <summary>
            /// The illuminatio n 3_ star t 2.
            /// </summary>
            ILLUMINATION3_START2 = 249,

            /// <summary>
            /// The illuminatio n 3_ duratio n 2.
            /// </summary>
            ILLUMINATION3_DURATION2 = 250,

            /// <summary>
            /// The illuminatio n 3_ brigh t 2.
            /// </summary>
            ILLUMINATION3_BRIGHT2 = 251,

            /// <summary>
            /// The illuminatio n 3_ star t 3.
            /// </summary>
            ILLUMINATION3_START3 = 252,

            /// <summary>
            /// The illuminatio n 3_ duratio n 3.
            /// </summary>
            ILLUMINATION3_DURATION3 = 253,

            /// <summary>
            /// The illuminatio n 3_ brigh t 3.
            /// </summary>
            ILLUMINATION3_BRIGHT3 = 254,

            /// <summary>
            /// The illuminatio n 3_ star t 4.
            /// </summary>
            ILLUMINATION3_START4 = 255,

            /// <summary>
            /// The illuminatio n 3_ duratio n 4.
            /// </summary>
            ILLUMINATION3_DURATION4 = 256,

            /// <summary>
            /// The illuminatio n 3_ brigh t 4.
            /// </summary>
            ILLUMINATION3_BRIGHT4 = 257,

            /// <summary>
            /// The illuminatio n 3_ star t 5.
            /// </summary>
            ILLUMINATION3_START5 = 258,

            /// <summary>
            /// The illuminatio n 3_ duratio n 5.
            /// </summary>
            ILLUMINATION3_DURATION5 = 259,

            /// <summary>
            /// The illuminatio n 3_ brigh t 5.
            /// </summary>
            ILLUMINATION3_BRIGHT5 = 260,

            /// <summary>
            /// The illuminatio n 3_ star t 6.
            /// </summary>
            ILLUMINATION3_START6 = 261,

            /// <summary>
            /// The illuminatio n 3_ duratio n 6.
            /// </summary>
            ILLUMINATION3_DURATION6 = 262,

            /// <summary>
            /// The illuminatio n 3_ brigh t 6.
            /// </summary>
            ILLUMINATION3_BRIGHT6 = 263,

            /// <summary>
            /// The illuminatio n 3_ star t 7.
            /// </summary>
            ILLUMINATION3_START7 = 264,

            /// <summary>
            /// The illuminatio n 3_ duratio n 7.
            /// </summary>
            ILLUMINATION3_DURATION7 = 265,

            /// <summary>
            /// The illuminatio n 3_ brigh t 7.
            /// </summary>
            ILLUMINATION3_BRIGHT7 = 266,

            /// <summary>
            /// The illuminatio n 3_ star t 8.
            /// </summary>
            ILLUMINATION3_START8 = 267,

            /// <summary>
            /// The illuminatio n 3_ duratio n 8.
            /// </summary>
            ILLUMINATION3_DURATION8 = 268,

            /// <summary>
            /// The illuminatio n 3_ brigh t 8.
            /// </summary>
            ILLUMINATION3_BRIGHT8 = 269,

            /// <summary>
            /// The illuminatio n 3_ burninduration.
            /// </summary>
            ILLUMINATION3_BURNINDURATION = 270,

            /// <summary>
            /// The illuminatio n 3_ type.
            /// </summary>
            ILLUMINATION3_TYPE = 271,

            // 272 ... 275 free

            /// <summary>
            /// The illuminatio n 4_ simulationmask.
            /// </summary>
            ILLUMINATION4_SIMULATIONMASK = 276,

            /// <summary>
            /// The illuminatio n 4_ nu m_ points.
            /// </summary>
            ILLUMINATION4_NUM_POINTS = 277,

            /// <summary>
            /// The illuminatio n 4_ star t 1.
            /// </summary>
            ILLUMINATION4_START1 = 278,

            /// <summary>
            /// The illuminatio n 4_ duratio n 1.
            /// </summary>
            ILLUMINATION4_DURATION1 = 279,

            /// <summary>
            /// The illuminatio n 4_ brigh t 1.
            /// </summary>
            ILLUMINATION4_BRIGHT1 = 280,

            /// <summary>
            /// The illuminatio n 4_ star t 2.
            /// </summary>
            ILLUMINATION4_START2 = 281,

            /// <summary>
            /// The illuminatio n 4_ duratio n 2.
            /// </summary>
            ILLUMINATION4_DURATION2 = 282,

            /// <summary>
            /// The illuminatio n 4_ brigh t 2.
            /// </summary>
            ILLUMINATION4_BRIGHT2 = 283,

            /// <summary>
            /// The illuminatio n 4_ star t 3.
            /// </summary>
            ILLUMINATION4_START3 = 284,

            /// <summary>
            /// The illuminatio n 4_ duratio n 3.
            /// </summary>
            ILLUMINATION4_DURATION3 = 285,

            /// <summary>
            /// The illuminatio n 4_ brigh t 3.
            /// </summary>
            ILLUMINATION4_BRIGHT3 = 286,

            /// <summary>
            /// The illuminatio n 4_ star t 4.
            /// </summary>
            ILLUMINATION4_START4 = 287,

            /// <summary>
            /// The illuminatio n 4_ duratio n 4.
            /// </summary>
            ILLUMINATION4_DURATION4 = 288,

            /// <summary>
            /// The illuminatio n 4_ brigh t 4.
            /// </summary>
            ILLUMINATION4_BRIGHT4 = 289,

            /// <summary>
            /// The illuminatio n 4_ star t 5.
            /// </summary>
            ILLUMINATION4_START5 = 290,

            /// <summary>
            /// The illuminatio n 4_ duratio n 5.
            /// </summary>
            ILLUMINATION4_DURATION5 = 291,

            /// <summary>
            /// The illuminatio n 4_ brigh t 5.
            /// </summary>
            ILLUMINATION4_BRIGHT5 = 292,

            /// <summary>
            /// The illuminatio n 4_ star t 6.
            /// </summary>
            ILLUMINATION4_START6 = 293,

            /// <summary>
            /// The illuminatio n 4_ duratio n 6.
            /// </summary>
            ILLUMINATION4_DURATION6 = 294,

            /// <summary>
            /// The illuminatio n 4_ brigh t 6.
            /// </summary>
            ILLUMINATION4_BRIGHT6 = 295,

            /// <summary>
            /// The illuminatio n 4_ star t 7.
            /// </summary>
            ILLUMINATION4_START7 = 296,

            /// <summary>
            /// The illuminatio n 4_ duratio n 7.
            /// </summary>
            ILLUMINATION4_DURATION7 = 297,

            /// <summary>
            /// The illuminatio n 4_ brigh t 7.
            /// </summary>
            ILLUMINATION4_BRIGHT7 = 298,

            /// <summary>
            /// The illuminatio n 4_ star t 8.
            /// </summary>
            ILLUMINATION4_START8 = 299,

            /// <summary>
            /// The illuminatio n 4_ duratio n 8.
            /// </summary>
            ILLUMINATION4_DURATION8 = 300,

            /// <summary>
            /// The illuminatio n 4_ brigh t 8.
            /// </summary>
            ILLUMINATION4_BRIGHT8 = 301,

            /// <summary>
            /// The illuminatio n 4_ burninduration.
            /// </summary>
            ILLUMINATION4_BURNINDURATION = 302,

            /// <summary>
            /// The illuminatio n 4_ type.
            /// </summary>
            ILLUMINATION4_TYPE = 303,

            // 304 ... 307 free

            /// <summary>
            /// The illuminatio n 5_ simulationmask.
            /// </summary>
            ILLUMINATION5_SIMULATIONMASK = 308,

            /// <summary>
            /// The illuminatio n 5_ nu m_ points.
            /// </summary>
            ILLUMINATION5_NUM_POINTS = 309,

            /// <summary>
            /// The illuminatio n 5_ star t 1.
            /// </summary>
            ILLUMINATION5_START1 = 310,

            /// <summary>
            /// The illuminatio n 5_ duratio n 1.
            /// </summary>
            ILLUMINATION5_DURATION1 = 311,

            /// <summary>
            /// The illuminatio n 5_ brigh t 1.
            /// </summary>
            ILLUMINATION5_BRIGHT1 = 312,

            /// <summary>
            /// The illuminatio n 5_ star t 2.
            /// </summary>
            ILLUMINATION5_START2 = 313,

            /// <summary>
            /// The illuminatio n 5_ duratio n 2.
            /// </summary>
            ILLUMINATION5_DURATION2 = 314,

            /// <summary>
            /// The illuminatio n 5_ brigh t 2.
            /// </summary>
            ILLUMINATION5_BRIGHT2 = 315,

            /// <summary>
            /// The illuminatio n 5_ star t 3.
            /// </summary>
            ILLUMINATION5_START3 = 316,

            /// <summary>
            /// The illuminatio n 5_ duratio n 3.
            /// </summary>
            ILLUMINATION5_DURATION3 = 317,

            /// <summary>
            /// The illuminatio n 5_ brigh t 3.
            /// </summary>
            ILLUMINATION5_BRIGHT3 = 318,

            /// <summary>
            /// The illuminatio n 5_ star t 4.
            /// </summary>
            ILLUMINATION5_START4 = 319,

            /// <summary>
            /// The illuminatio n 5_ duratio n 4.
            /// </summary>
            ILLUMINATION5_DURATION4 = 320,

            /// <summary>
            /// The illuminatio n 5_ brigh t 4.
            /// </summary>
            ILLUMINATION5_BRIGHT4 = 321,

            /// <summary>
            /// The illuminatio n 5_ star t 5.
            /// </summary>
            ILLUMINATION5_START5 = 322,

            /// <summary>
            /// The illuminatio n 5_ duratio n 5.
            /// </summary>
            ILLUMINATION5_DURATION5 = 323,

            /// <summary>
            /// The illuminatio n 5_ brigh t 5.
            /// </summary>
            ILLUMINATION5_BRIGHT5 = 324,

            /// <summary>
            /// The illuminatio n 5_ star t 6.
            /// </summary>
            ILLUMINATION5_START6 = 325,

            /// <summary>
            /// The illuminatio n 5_ duratio n 6.
            /// </summary>
            ILLUMINATION5_DURATION6 = 326,

            /// <summary>
            /// The illuminatio n 5_ brigh t 6.
            /// </summary>
            ILLUMINATION5_BRIGHT6 = 327,

            /// <summary>
            /// The illuminatio n 5_ star t 7.
            /// </summary>
            ILLUMINATION5_START7 = 328,

            /// <summary>
            /// The illuminatio n 5_ duratio n 7.
            /// </summary>
            ILLUMINATION5_DURATION7 = 329,

            /// <summary>
            /// The illuminatio n 5_ brigh t 7.
            /// </summary>
            ILLUMINATION5_BRIGHT7 = 330,

            /// <summary>
            /// The illuminatio n 5_ star t 8.
            /// </summary>
            ILLUMINATION5_START8 = 331,

            /// <summary>
            /// The illuminatio n 5_ duratio n 8.
            /// </summary>
            ILLUMINATION5_DURATION8 = 332,

            /// <summary>
            /// The illuminatio n 5_ brigh t 8.
            /// </summary>
            ILLUMINATION5_BRIGHT8 = 333,

            /// <summary>
            /// The illuminatio n 5_ burninduration.
            /// </summary>
            ILLUMINATION5_BURNINDURATION = 334,

            /// <summary>
            /// The illuminatio n 5_ type.
            /// </summary>
            ILLUMINATION5_TYPE = 335,

            // 336 ... 339 free

            /// <summary>
            /// The illuminatio n 6_ simulationmask.
            /// </summary>
            ILLUMINATION6_SIMULATIONMASK = 340,

            /// <summary>
            /// The illuminatio n 6_ nu m_ points.
            /// </summary>
            ILLUMINATION6_NUM_POINTS = 341,

            /// <summary>
            /// The illuminatio n 6_ star t 1.
            /// </summary>
            ILLUMINATION6_START1 = 342,

            /// <summary>
            /// The illuminatio n 6_ duratio n 1.
            /// </summary>
            ILLUMINATION6_DURATION1 = 343,

            /// <summary>
            /// The illuminatio n 6_ brigh t 1.
            /// </summary>
            ILLUMINATION6_BRIGHT1 = 344,

            /// <summary>
            /// The illuminatio n 6_ star t 2.
            /// </summary>
            ILLUMINATION6_START2 = 345,

            /// <summary>
            /// The illuminatio n 6_ duratio n 2.
            /// </summary>
            ILLUMINATION6_DURATION2 = 346,

            /// <summary>
            /// The illuminatio n 6_ brigh t 2.
            /// </summary>
            ILLUMINATION6_BRIGHT2 = 347,

            /// <summary>
            /// The illuminatio n 6_ star t 3.
            /// </summary>
            ILLUMINATION6_START3 = 348,

            /// <summary>
            /// The illuminatio n 6_ duratio n 3.
            /// </summary>
            ILLUMINATION6_DURATION3 = 349,

            /// <summary>
            /// The illuminatio n 6_ brigh t 3.
            /// </summary>
            ILLUMINATION6_BRIGHT3 = 350,

            /// <summary>
            /// The illuminatio n 6_ star t 4.
            /// </summary>
            ILLUMINATION6_START4 = 351,

            /// <summary>
            /// The illuminatio n 6_ duratio n 4.
            /// </summary>
            ILLUMINATION6_DURATION4 = 352,

            /// <summary>
            /// The illuminatio n 6_ brigh t 4.
            /// </summary>
            ILLUMINATION6_BRIGHT4 = 353,

            /// <summary>
            /// The illuminatio n 6_ star t 5.
            /// </summary>
            ILLUMINATION6_START5 = 354,

            /// <summary>
            /// The illuminatio n 6_ duratio n 5.
            /// </summary>
            ILLUMINATION6_DURATION5 = 355,

            /// <summary>
            /// The illuminatio n 6_ brigh t 5.
            /// </summary>
            ILLUMINATION6_BRIGHT5 = 356,

            /// <summary>
            /// The illuminatio n 6_ star t 6.
            /// </summary>
            ILLUMINATION6_START6 = 357,

            /// <summary>
            /// The illuminatio n 6_ duratio n 6.
            /// </summary>
            ILLUMINATION6_DURATION6 = 358,

            /// <summary>
            /// The illuminatio n 6_ brigh t 6.
            /// </summary>
            ILLUMINATION6_BRIGHT6 = 359,

            /// <summary>
            /// The illuminatio n 6_ star t 7.
            /// </summary>
            ILLUMINATION6_START7 = 360,

            /// <summary>
            /// The illuminatio n 6_ duratio n 7.
            /// </summary>
            ILLUMINATION6_DURATION7 = 361,

            /// <summary>
            /// The illuminatio n 6_ brigh t 7.
            /// </summary>
            ILLUMINATION6_BRIGHT7 = 362,

            /// <summary>
            /// The illuminatio n 6_ star t 8.
            /// </summary>
            ILLUMINATION6_START8 = 363,

            /// <summary>
            /// The illuminatio n 6_ duratio n 8.
            /// </summary>
            ILLUMINATION6_DURATION8 = 364,

            /// <summary>
            /// The illuminatio n 6_ brigh t 8.
            /// </summary>
            ILLUMINATION6_BRIGHT8 = 365,

            /// <summary>
            /// The illuminatio n 6_ burninduration.
            /// </summary>
            ILLUMINATION6_BURNINDURATION = 366,

            /// <summary>
            /// The illuminatio n 6_ type.
            /// </summary>
            ILLUMINATION6_TYPE = 367,

            // 368 ... 371 free

            /// <summary>
            /// The illuminatio n 7_ simulationmask.
            /// </summary>
            ILLUMINATION7_SIMULATIONMASK = 372,

            /// <summary>
            /// The illuminatio n 7_ nu m_ points.
            /// </summary>
            ILLUMINATION7_NUM_POINTS = 373,

            /// <summary>
            /// The illuminatio n 7_ star t 1.
            /// </summary>
            ILLUMINATION7_START1 = 374,

            /// <summary>
            /// The illuminatio n 7_ duratio n 1.
            /// </summary>
            ILLUMINATION7_DURATION1 = 375,

            /// <summary>
            /// The illuminatio n 7_ brigh t 1.
            /// </summary>
            ILLUMINATION7_BRIGHT1 = 376,

            /// <summary>
            /// The illuminatio n 7_ star t 2.
            /// </summary>
            ILLUMINATION7_START2 = 377,

            /// <summary>
            /// The illuminatio n 7_ duratio n 2.
            /// </summary>
            ILLUMINATION7_DURATION2 = 378,

            /// <summary>
            /// The illuminatio n 7_ brigh t 2.
            /// </summary>
            ILLUMINATION7_BRIGHT2 = 379,

            /// <summary>
            /// The illuminatio n 7_ star t 3.
            /// </summary>
            ILLUMINATION7_START3 = 380,

            /// <summary>
            /// The illuminatio n 7_ duratio n 3.
            /// </summary>
            ILLUMINATION7_DURATION3 = 381,

            /// <summary>
            /// The illuminatio n 7_ brigh t 3.
            /// </summary>
            ILLUMINATION7_BRIGHT3 = 382,

            /// <summary>
            /// The illuminatio n 7_ star t 4.
            /// </summary>
            ILLUMINATION7_START4 = 383,

            /// <summary>
            /// The illuminatio n 7_ duratio n 4.
            /// </summary>
            ILLUMINATION7_DURATION4 = 384,

            /// <summary>
            /// The illuminatio n 7_ brigh t 4.
            /// </summary>
            ILLUMINATION7_BRIGHT4 = 385,

            /// <summary>
            /// The illuminatio n 7_ star t 5.
            /// </summary>
            ILLUMINATION7_START5 = 386,

            /// <summary>
            /// The illuminatio n 7_ duratio n 5.
            /// </summary>
            ILLUMINATION7_DURATION5 = 387,

            /// <summary>
            /// The illuminatio n 7_ brigh t 5.
            /// </summary>
            ILLUMINATION7_BRIGHT5 = 388,

            /// <summary>
            /// The illuminatio n 7_ star t 6.
            /// </summary>
            ILLUMINATION7_START6 = 389,

            /// <summary>
            /// The illuminatio n 7_ duratio n 6.
            /// </summary>
            ILLUMINATION7_DURATION6 = 390,

            /// <summary>
            /// The illuminatio n 7_ brigh t 6.
            /// </summary>
            ILLUMINATION7_BRIGHT6 = 391,

            /// <summary>
            /// The illuminatio n 7_ star t 7.
            /// </summary>
            ILLUMINATION7_START7 = 392,

            /// <summary>
            /// The illuminatio n 7_ duratio n 7.
            /// </summary>
            ILLUMINATION7_DURATION7 = 393,

            /// <summary>
            /// The illuminatio n 7_ brigh t 7.
            /// </summary>
            ILLUMINATION7_BRIGHT7 = 394,

            /// <summary>
            /// The illuminatio n 7_ star t 8.
            /// </summary>
            ILLUMINATION7_START8 = 395,

            /// <summary>
            /// The illuminatio n 7_ duratio n 8.
            /// </summary>
            ILLUMINATION7_DURATION8 = 396,

            /// <summary>
            /// The illuminatio n 7_ brigh t 8.
            /// </summary>
            ILLUMINATION7_BRIGHT8 = 397,

            /// <summary>
            /// The illuminatio n 7_ burninduration.
            /// </summary>
            ILLUMINATION7_BURNINDURATION = 398,

            /// <summary>
            /// The illuminatio n 7_ type.
            /// </summary>
            ILLUMINATION7_TYPE = 399,

            // 400 ... 403 free

            /// <summary>
            /// The illuminatio n 8_ simulationmask.
            /// </summary>
            ILLUMINATION8_SIMULATIONMASK = 404,

            /// <summary>
            /// The illuminatio n 8_ nu m_ points.
            /// </summary>
            ILLUMINATION8_NUM_POINTS = 405,

            /// <summary>
            /// The illuminatio n 8_ star t 1.
            /// </summary>
            ILLUMINATION8_START1 = 406,

            /// <summary>
            /// The illuminatio n 8_ duratio n 1.
            /// </summary>
            ILLUMINATION8_DURATION1 = 407,

            /// <summary>
            /// The illuminatio n 8_ brigh t 1.
            /// </summary>
            ILLUMINATION8_BRIGHT1 = 408,

            /// <summary>
            /// The illuminatio n 8_ star t 2.
            /// </summary>
            ILLUMINATION8_START2 = 409,

            /// <summary>
            /// The illuminatio n 8_ duratio n 2.
            /// </summary>
            ILLUMINATION8_DURATION2 = 410,

            /// <summary>
            /// The illuminatio n 8_ brigh t 2.
            /// </summary>
            ILLUMINATION8_BRIGHT2 = 411,

            /// <summary>
            /// The illuminatio n 8_ star t 3.
            /// </summary>
            ILLUMINATION8_START3 = 412,

            /// <summary>
            /// The illuminatio n 8_ duratio n 3.
            /// </summary>
            ILLUMINATION8_DURATION3 = 413,

            /// <summary>
            /// The illuminatio n 8_ brigh t 3.
            /// </summary>
            ILLUMINATION8_BRIGHT3 = 414,

            /// <summary>
            /// The illuminatio n 8_ star t 4.
            /// </summary>
            ILLUMINATION8_START4 = 415,

            /// <summary>
            /// The illuminatio n 8_ duratio n 4.
            /// </summary>
            ILLUMINATION8_DURATION4 = 416,

            /// <summary>
            /// The illuminatio n 8_ brigh t 4.
            /// </summary>
            ILLUMINATION8_BRIGHT4 = 417,

            /// <summary>
            /// The illuminatio n 8_ star t 5.
            /// </summary>
            ILLUMINATION8_START5 = 418,

            /// <summary>
            /// The illuminatio n 8_ duratio n 5.
            /// </summary>
            ILLUMINATION8_DURATION5 = 419,

            /// <summary>
            /// The illuminatio n 8_ brigh t 5.
            /// </summary>
            ILLUMINATION8_BRIGHT5 = 420,

            /// <summary>
            /// The illuminatio n 8_ star t 6.
            /// </summary>
            ILLUMINATION8_START6 = 421,

            /// <summary>
            /// The illuminatio n 8_ duratio n 6.
            /// </summary>
            ILLUMINATION8_DURATION6 = 422,

            /// <summary>
            /// The illuminatio n 8_ brigh t 6.
            /// </summary>
            ILLUMINATION8_BRIGHT6 = 423,

            /// <summary>
            /// The illuminatio n 8_ star t 7.
            /// </summary>
            ILLUMINATION8_START7 = 424,

            /// <summary>
            /// The illuminatio n 8_ duratio n 7.
            /// </summary>
            ILLUMINATION8_DURATION7 = 425,

            /// <summary>
            /// The illuminatio n 8_ brigh t 7.
            /// </summary>
            ILLUMINATION8_BRIGHT7 = 426,

            /// <summary>
            /// The illuminatio n 8_ star t 8.
            /// </summary>
            ILLUMINATION8_START8 = 427,

            /// <summary>
            /// The illuminatio n 8_ duratio n 8.
            /// </summary>
            ILLUMINATION8_DURATION8 = 428,

            /// <summary>
            /// The illuminatio n 8_ brigh t 8.
            /// </summary>
            ILLUMINATION8_BRIGHT8 = 429,

            /// <summary>
            /// The illuminatio n 8_ burninduration.
            /// </summary>
            ILLUMINATION8_BURNINDURATION = 430,

            /// <summary>
            /// The illuminatio n 8_ type.
            /// </summary>
            ILLUMINATION8_TYPE = 431,

            // 432 ... 435 free

            // Aqua Illumination settings

            /// <summary>
            /// The aquailluminatio n_ available.
            /// </summary>
            AQUAILLUMINATION_AVAILABLE = 436,

            /// <summary>
            /// The aquailluminatio n_ whitechannel.
            /// </summary>
            AQUAILLUMINATION_WHITECHANNEL = 437,

            /// <summary>
            /// The aquailluminatio n_ bluechannel.
            /// </summary>
            AQUAILLUMINATION_BLUECHANNEL = 438,

            // 439 free

            // Codes Erinnerung (12 pro Erinnerung, 4 Erinnerungen)
            // #define  BLOCKSIZE_MEMORY=   12,

            /// <summary>
            /// The me m 1_ nextmem.
            /// </summary>
            MEM1_NEXTMEM = 440,

            /// <summary>
            /// The me m 1_ repeat.
            /// </summary>
            MEM1_REPEAT = 441,

            /// <summary>
            /// The me m 1_ days.
            /// </summary>
            MEM1_DAYS = 442,

            /// <summary>
            /// The me m 1_ tex t 01.
            /// </summary>
            MEM1_TEXT01 = 443,

            /// <summary>
            /// The me m 1_ tex t 23.
            /// </summary>
            MEM1_TEXT23 = 444,

            /// <summary>
            /// The me m 1_ tex t 45.
            /// </summary>
            MEM1_TEXT45 = 445,

            /// <summary>
            /// The me m 1_ tex t 67.
            /// </summary>
            MEM1_TEXT67 = 446,

            /// <summary>
            /// The me m 1_ tex t 89.
            /// </summary>
            MEM1_TEXT89 = 447,

            /// <summary>
            /// The me m 1_ tex t 1011.
            /// </summary>
            MEM1_TEXT1011 = 448,

            /// <summary>
            /// The me m 1_ tex t 1213.
            /// </summary>
            MEM1_TEXT1213 = 449,

            /// <summary>
            /// The me m 1_ tex t 1415.
            /// </summary>
            MEM1_TEXT1415 = 450,

            // 451 free

            /// <summary>
            /// The me m 2_ nextmem.
            /// </summary>
            MEM2_NEXTMEM = 452,

            /// <summary>
            /// The me m 2_ repeat.
            /// </summary>
            MEM2_REPEAT = 453,

            /// <summary>
            /// The me m 2_ days.
            /// </summary>
            MEM2_DAYS = 454,

            /// <summary>
            /// The me m 2_ tex t 01.
            /// </summary>
            MEM2_TEXT01 = 455,

            /// <summary>
            /// The me m 2_ tex t 23.
            /// </summary>
            MEM2_TEXT23 = 456,

            /// <summary>
            /// The me m 2_ tex t 45.
            /// </summary>
            MEM2_TEXT45 = 457,

            /// <summary>
            /// The me m 2_ tex t 67.
            /// </summary>
            MEM2_TEXT67 = 458,

            /// <summary>
            /// The me m 2_ tex t 89.
            /// </summary>
            MEM2_TEXT89 = 459,

            /// <summary>
            /// The me m 2_ tex t 1011.
            /// </summary>
            MEM2_TEXT1011 = 460,

            /// <summary>
            /// The me m 2_ tex t 1213.
            /// </summary>
            MEM2_TEXT1213 = 461,

            /// <summary>
            /// The me m 2_ tex t 1415.
            /// </summary>
            MEM2_TEXT1415 = 462,

            // 463 free

            /// <summary>
            /// The me m 3_ nextmem.
            /// </summary>
            MEM3_NEXTMEM = 464,

            /// <summary>
            /// The me m 3_ repeat.
            /// </summary>
            MEM3_REPEAT = 465,

            /// <summary>
            /// The me m 3_ days.
            /// </summary>
            MEM3_DAYS = 466,

            /// <summary>
            /// The me m 3_ tex t 01.
            /// </summary>
            MEM3_TEXT01 = 467,

            /// <summary>
            /// The me m 3_ tex t 23.
            /// </summary>
            MEM3_TEXT23 = 468,

            /// <summary>
            /// The me m 3_ tex t 45.
            /// </summary>
            MEM3_TEXT45 = 469,

            /// <summary>
            /// The me m 3_ tex t 67.
            /// </summary>
            MEM3_TEXT67 = 470,

            /// <summary>
            /// The me m 3_ tex t 89.
            /// </summary>
            MEM3_TEXT89 = 471,

            /// <summary>
            /// The me m 3_ tex t 1011.
            /// </summary>
            MEM3_TEXT1011 = 472,

            /// <summary>
            /// The me m 3_ tex t 1213.
            /// </summary>
            MEM3_TEXT1213 = 473,

            /// <summary>
            /// The me m 3_ tex t 1415.
            /// </summary>
            MEM3_TEXT1415 = 474,

            // 475 free

            /// <summary>
            /// The me m 4_ nextmem.
            /// </summary>
            MEM4_NEXTMEM = 476,

            /// <summary>
            /// The me m 4_ repeat.
            /// </summary>
            MEM4_REPEAT = 477,

            /// <summary>
            /// The me m 4_ days.
            /// </summary>
            MEM4_DAYS = 478,

            /// <summary>
            /// The me m 4_ tex t 01.
            /// </summary>
            MEM4_TEXT01 = 479,

            /// <summary>
            /// The me m 4_ tex t 23.
            /// </summary>
            MEM4_TEXT23 = 480,

            /// <summary>
            /// The me m 4_ tex t 45.
            /// </summary>
            MEM4_TEXT45 = 481,

            /// <summary>
            /// The me m 4_ tex t 67.
            /// </summary>
            MEM4_TEXT67 = 482,

            /// <summary>
            /// The me m 4_ tex t 89.
            /// </summary>
            MEM4_TEXT89 = 483,

            /// <summary>
            /// The me m 4_ tex t 1011.
            /// </summary>
            MEM4_TEXT1011 = 484,

            /// <summary>
            /// The me m 4_ tex t 1213.
            /// </summary>
            MEM4_TEXT1213 = 485,

            /// <summary>
            /// The me m 4_ tex t 1415.
            /// </summary>
            MEM4_TEXT1415 = 486,

            // 487-488 free

            // PIN

            /// <summary>
            /// The pin.
            /// </summary>
            PIN = 489,

            // Wolkensimulation

            /// <summary>
            /// The clou d_ probability.
            /// </summary>
            CLOUD_PROBABILITY = 490,

            /// <summary>
            /// The clou d_ minduration.
            /// </summary>
            CLOUD_MINDURATION = 491,

            /// <summary>
            /// The clou d_ maxduration.
            /// </summary>
            CLOUD_MAXDURATION = 492,

            /// <summary>
            /// The clou d_ maxdarkening.
            /// </summary>
            CLOUD_MAXDARKENING = 493,

            // Mondphasensimulation

            /// <summary>
            /// The moo n_ illuminationchannels.
            /// </summary>
            MOON_ILLUMINATIONCHANNELS = 495,

            /// <summary>
            /// The moo n_ start.
            /// </summary>
            MOON_START = 496,

            /// <summary>
            /// The moo n_ end.
            /// </summary>
            MOON_END = 497,

            /// <summary>
            /// The moo n_ actphase.
            /// </summary>
            MOON_ACTPHASE = 498,

            // Codes Zeitschaltuhren (20 pro Zeitschaltuhr, 8 Zeitschaltuhren)
            // Zeitschaltuhr= 1,
            // #define BLOCKSIZE_TIMER= 20,

            /// <summary>
            /// The time r 1_ mode.
            /// </summary>
            TIMER1_MODE = 500,

            /// <summary>
            /// The time r 1_ count.
            /// </summary>
            TIMER1_COUNT = 501,

            /// <summary>
            /// The time r 1_ offset.
            /// </summary>
            TIMER1_OFFSET = 502,

            /// <summary>
            /// The time r 1_ do w_ int.
            /// </summary>
            TIMER1_DOW_INT = 503,

            /// <summary>
            /// The time r 1_ star t 1.
            /// </summary>
            TIMER1_START1 = 504,

            /// <summary>
            /// The time r 1_ en d 1.
            /// </summary>
            TIMER1_END1 = 505,

            /// <summary>
            /// The time r 1_ star t 2.
            /// </summary>
            TIMER1_START2 = 506,

            /// <summary>
            /// The time r 1_ en d 2.
            /// </summary>
            TIMER1_END2 = 507,

            /// <summary>
            /// The time r 1_ star t 3.
            /// </summary>
            TIMER1_START3 = 508,

            /// <summary>
            /// The time r 1_ en d 3.
            /// </summary>
            TIMER1_END3 = 509,

            /// <summary>
            /// The time r 1_ star t 4.
            /// </summary>
            TIMER1_START4 = 510,

            /// <summary>
            /// The time r 1_ en d 4.
            /// </summary>
            TIMER1_END4 = 511,

            /// <summary>
            /// The time r 1_ star t 5.
            /// </summary>
            TIMER1_START5 = 512,

            /// <summary>
            /// The time r 1_ en d 5.
            /// </summary>
            TIMER1_END5 = 513,

            /// <summary>
            /// The time r 1_ star t 6.
            /// </summary>
            TIMER1_START6 = 514,

            /// <summary>
            /// The time r 1_ en d 6.
            /// </summary>
            TIMER1_END6 = 515,

            /// <summary>
            /// The time r 1_ star t 7.
            /// </summary>
            TIMER1_START7 = 516,

            /// <summary>
            /// The time r 1_ en d 7.
            /// </summary>
            TIMER1_END7 = 517,

            /// <summary>
            /// The time r 1_ star t 8.
            /// </summary>
            TIMER1_START8 = 518,

            /// <summary>
            /// The time r 1_ en d 8.
            /// </summary>
            TIMER1_END8 = 519,

            /// <summary>
            /// The time r 2_ mode.
            /// </summary>
            TIMER2_MODE = 520,

            /// <summary>
            /// The time r 2_ count.
            /// </summary>
            TIMER2_COUNT = 521,

            /// <summary>
            /// The time r 2_ offset.
            /// </summary>
            TIMER2_OFFSET = 522,

            /// <summary>
            /// The time r 2_ do w_ int.
            /// </summary>
            TIMER2_DOW_INT = 523,

            /// <summary>
            /// The time r 2_ star t 1.
            /// </summary>
            TIMER2_START1 = 524,

            /// <summary>
            /// The time r 2_ en d 1.
            /// </summary>
            TIMER2_END1 = 525,

            /// <summary>
            /// The time r 2_ star t 2.
            /// </summary>
            TIMER2_START2 = 526,

            /// <summary>
            /// The time r 2_ en d 2.
            /// </summary>
            TIMER2_END2 = 527,

            /// <summary>
            /// The time r 2_ star t 3.
            /// </summary>
            TIMER2_START3 = 528,

            /// <summary>
            /// The time r 2_ en d 3.
            /// </summary>
            TIMER2_END3 = 529,

            /// <summary>
            /// The time r 2_ star t 4.
            /// </summary>
            TIMER2_START4 = 530,

            /// <summary>
            /// The time r 2_ en d 4.
            /// </summary>
            TIMER2_END4 = 531,

            /// <summary>
            /// The time r 2_ star t 5.
            /// </summary>
            TIMER2_START5 = 532,

            /// <summary>
            /// The time r 2_ en d 5.
            /// </summary>
            TIMER2_END5 = 533,

            /// <summary>
            /// The time r 2_ star t 6.
            /// </summary>
            TIMER2_START6 = 534,

            /// <summary>
            /// The time r 2_ en d 6.
            /// </summary>
            TIMER2_END6 = 535,

            /// <summary>
            /// The time r 2_ star t 7.
            /// </summary>
            TIMER2_START7 = 536,

            /// <summary>
            /// The time r 2_ en d 7.
            /// </summary>
            TIMER2_END7 = 537,

            /// <summary>
            /// The time r 2_ star t 8.
            /// </summary>
            TIMER2_START8 = 538,

            /// <summary>
            /// The time r 2_ en d 8.
            /// </summary>
            TIMER2_END8 = 539,

            /// <summary>
            /// The time r 3_ mode.
            /// </summary>
            TIMER3_MODE = 540,

            /// <summary>
            /// The time r 3_ count.
            /// </summary>
            TIMER3_COUNT = 541,

            /// <summary>
            /// The time r 3_ offset.
            /// </summary>
            TIMER3_OFFSET = 542,

            /// <summary>
            /// The time r 3_ do w_ int.
            /// </summary>
            TIMER3_DOW_INT = 543,

            /// <summary>
            /// The time r 3_ star t 1.
            /// </summary>
            TIMER3_START1 = 544,

            /// <summary>
            /// The time r 3_ en d 1.
            /// </summary>
            TIMER3_END1 = 545,

            /// <summary>
            /// The time r 3_ star t 2.
            /// </summary>
            TIMER3_START2 = 546,

            /// <summary>
            /// The time r 3_ en d 2.
            /// </summary>
            TIMER3_END2 = 547,

            /// <summary>
            /// The time r 3_ star t 3.
            /// </summary>
            TIMER3_START3 = 548,

            /// <summary>
            /// The time r 3_ en d 3.
            /// </summary>
            TIMER3_END3 = 549,

            /// <summary>
            /// The time r 3_ star t 4.
            /// </summary>
            TIMER3_START4 = 550,

            /// <summary>
            /// The time r 3_ en d 4.
            /// </summary>
            TIMER3_END4 = 551,

            /// <summary>
            /// The time r 3_ star t 5.
            /// </summary>
            TIMER3_START5 = 552,

            /// <summary>
            /// The time r 3_ en d 5.
            /// </summary>
            TIMER3_END5 = 553,

            /// <summary>
            /// The time r 3_ star t 6.
            /// </summary>
            TIMER3_START6 = 554,

            /// <summary>
            /// The time r 3_ en d 6.
            /// </summary>
            TIMER3_END6 = 555,

            /// <summary>
            /// The time r 3_ star t 7.
            /// </summary>
            TIMER3_START7 = 556,

            /// <summary>
            /// The time r 3_ en d 7.
            /// </summary>
            TIMER3_END7 = 557,

            /// <summary>
            /// The time r 3_ star t 8.
            /// </summary>
            TIMER3_START8 = 558,

            /// <summary>
            /// The time r 3_ en d 8.
            /// </summary>
            TIMER3_END8 = 559,

            /// <summary>
            /// The time r 4_ mode.
            /// </summary>
            TIMER4_MODE = 560,

            /// <summary>
            /// The time r 4_ count.
            /// </summary>
            TIMER4_COUNT = 561,

            /// <summary>
            /// The time r 4_ offset.
            /// </summary>
            TIMER4_OFFSET = 562,

            /// <summary>
            /// The time r 4_ do w_ int.
            /// </summary>
            TIMER4_DOW_INT = 563,

            /// <summary>
            /// The time r 4_ star t 1.
            /// </summary>
            TIMER4_START1 = 564,

            /// <summary>
            /// The time r 4_ en d 1.
            /// </summary>
            TIMER4_END1 = 565,

            /// <summary>
            /// The time r 4_ star t 2.
            /// </summary>
            TIMER4_START2 = 566,

            /// <summary>
            /// The time r 4_ en d 2.
            /// </summary>
            TIMER4_END2 = 567,

            /// <summary>
            /// The time r 4_ star t 3.
            /// </summary>
            TIMER4_START3 = 568,

            /// <summary>
            /// The time r 4_ en d 3.
            /// </summary>
            TIMER4_END3 = 569,

            /// <summary>
            /// The time r 4_ star t 4.
            /// </summary>
            TIMER4_START4 = 570,

            /// <summary>
            /// The time r 4_ en d 4.
            /// </summary>
            TIMER4_END4 = 571,

            /// <summary>
            /// The time r 4_ star t 5.
            /// </summary>
            TIMER4_START5 = 572,

            /// <summary>
            /// The time r 4_ en d 5.
            /// </summary>
            TIMER4_END5 = 573,

            /// <summary>
            /// The time r 4_ star t 6.
            /// </summary>
            TIMER4_START6 = 574,

            /// <summary>
            /// The time r 4_ en d 6.
            /// </summary>
            TIMER4_END6 = 575,

            /// <summary>
            /// The time r 4_ star t 7.
            /// </summary>
            TIMER4_START7 = 576,

            /// <summary>
            /// The time r 4_ en d 7.
            /// </summary>
            TIMER4_END7 = 577,

            /// <summary>
            /// The time r 4_ star t 8.
            /// </summary>
            TIMER4_START8 = 578,

            /// <summary>
            /// The time r 4_ en d 8.
            /// </summary>
            TIMER4_END8 = 579,

            /// <summary>
            /// The time r 5_ mode.
            /// </summary>
            TIMER5_MODE = 580,

            /// <summary>
            /// The time r 5_ count.
            /// </summary>
            TIMER5_COUNT = 581,

            /// <summary>
            /// The time r 5_ offset.
            /// </summary>
            TIMER5_OFFSET = 582,

            /// <summary>
            /// The time r 5_ do w_ int.
            /// </summary>
            TIMER5_DOW_INT = 583,

            /// <summary>
            /// The time r 5_ star t 1.
            /// </summary>
            TIMER5_START1 = 584,

            /// <summary>
            /// The time r 5_ en d 1.
            /// </summary>
            TIMER5_END1 = 585,

            /// <summary>
            /// The time r 5_ star t 2.
            /// </summary>
            TIMER5_START2 = 586,

            /// <summary>
            /// The time r 5_ en d 2.
            /// </summary>
            TIMER5_END2 = 587,

            /// <summary>
            /// The time r 5_ star t 3.
            /// </summary>
            TIMER5_START3 = 588,

            /// <summary>
            /// The time r 5_ en d 3.
            /// </summary>
            TIMER5_END3 = 589,

            /// <summary>
            /// The time r 5_ star t 4.
            /// </summary>
            TIMER5_START4 = 590,

            /// <summary>
            /// The time r 5_ en d 4.
            /// </summary>
            TIMER5_END4 = 591,

            /// <summary>
            /// The time r 5_ star t 5.
            /// </summary>
            TIMER5_START5 = 592,

            /// <summary>
            /// The time r 5_ en d 5.
            /// </summary>
            TIMER5_END5 = 593,

            /// <summary>
            /// The time r 5_ star t 6.
            /// </summary>
            TIMER5_START6 = 594,

            /// <summary>
            /// The time r 5_ en d 6.
            /// </summary>
            TIMER5_END6 = 595,

            /// <summary>
            /// The time r 5_ star t 7.
            /// </summary>
            TIMER5_START7 = 596,

            /// <summary>
            /// The time r 5_ en d 7.
            /// </summary>
            TIMER5_END7 = 597,

            /// <summary>
            /// The time r 5_ star t 8.
            /// </summary>
            TIMER5_START8 = 598,

            /// <summary>
            /// The time r 5_ en d 8.
            /// </summary>
            TIMER5_END8 = 599,

            /// <summary>
            /// The time r 6_ mode.
            /// </summary>
            TIMER6_MODE = 600,

            /// <summary>
            /// The time r 6_ count.
            /// </summary>
            TIMER6_COUNT = 601,

            /// <summary>
            /// The time r 6_ offset.
            /// </summary>
            TIMER6_OFFSET = 602,

            /// <summary>
            /// The time r 6_ do w_ int.
            /// </summary>
            TIMER6_DOW_INT = 603,

            /// <summary>
            /// The time r 6_ star t 1.
            /// </summary>
            TIMER6_START1 = 604,

            /// <summary>
            /// The time r 6_ en d 1.
            /// </summary>
            TIMER6_END1 = 605,

            /// <summary>
            /// The time r 6_ star t 2.
            /// </summary>
            TIMER6_START2 = 606,

            /// <summary>
            /// The time r 6_ en d 2.
            /// </summary>
            TIMER6_END2 = 607,

            /// <summary>
            /// The time r 6_ star t 3.
            /// </summary>
            TIMER6_START3 = 608,

            /// <summary>
            /// The time r 6_ en d 3.
            /// </summary>
            TIMER6_END3 = 609,

            /// <summary>
            /// The time r 6_ star t 4.
            /// </summary>
            TIMER6_START4 = 610,

            /// <summary>
            /// The time r 6_ en d 4.
            /// </summary>
            TIMER6_END4 = 611,

            /// <summary>
            /// The time r 6_ star t 5.
            /// </summary>
            TIMER6_START5 = 612,

            /// <summary>
            /// The time r 6_ en d 5.
            /// </summary>
            TIMER6_END5 = 613,

            /// <summary>
            /// The time r 6_ star t 6.
            /// </summary>
            TIMER6_START6 = 614,

            /// <summary>
            /// The time r 6_ en d 6.
            /// </summary>
            TIMER6_END6 = 615,

            /// <summary>
            /// The time r 6_ star t 7.
            /// </summary>
            TIMER6_START7 = 616,

            /// <summary>
            /// The time r 6_ en d 7.
            /// </summary>
            TIMER6_END7 = 617,

            /// <summary>
            /// The time r 6_ star t 8.
            /// </summary>
            TIMER6_START8 = 618,

            /// <summary>
            /// The time r 6_ en d 8.
            /// </summary>
            TIMER6_END8 = 619,

            /// <summary>
            /// The time r 7_ mode.
            /// </summary>
            TIMER7_MODE = 620,

            /// <summary>
            /// The time r 7_ count.
            /// </summary>
            TIMER7_COUNT = 621,

            /// <summary>
            /// The time r 7_ offset.
            /// </summary>
            TIMER7_OFFSET = 622,

            /// <summary>
            /// The time r 7_ do w_ int.
            /// </summary>
            TIMER7_DOW_INT = 623,

            /// <summary>
            /// The time r 7_ star t 1.
            /// </summary>
            TIMER7_START1 = 624,

            /// <summary>
            /// The time r 7_ en d 1.
            /// </summary>
            TIMER7_END1 = 625,

            /// <summary>
            /// The time r 7_ star t 2.
            /// </summary>
            TIMER7_START2 = 626,

            /// <summary>
            /// The time r 7_ en d 2.
            /// </summary>
            TIMER7_END2 = 627,

            /// <summary>
            /// The time r 7_ star t 3.
            /// </summary>
            TIMER7_START3 = 628,

            /// <summary>
            /// The time r 7_ en d 3.
            /// </summary>
            TIMER7_END3 = 629,

            /// <summary>
            /// The time r 7_ star t 4.
            /// </summary>
            TIMER7_START4 = 630,

            /// <summary>
            /// The time r 7_ en d 4.
            /// </summary>
            TIMER7_END4 = 631,

            /// <summary>
            /// The time r 7_ star t 5.
            /// </summary>
            TIMER7_START5 = 632,

            /// <summary>
            /// The time r 7_ en d 5.
            /// </summary>
            TIMER7_END5 = 633,

            /// <summary>
            /// The time r 7_ star t 6.
            /// </summary>
            TIMER7_START6 = 634,

            /// <summary>
            /// The time r 7_ en d 6.
            /// </summary>
            TIMER7_END6 = 635,

            /// <summary>
            /// The time r 7_ star t 7.
            /// </summary>
            TIMER7_START7 = 636,

            /// <summary>
            /// The time r 7_ en d 7.
            /// </summary>
            TIMER7_END7 = 637,

            /// <summary>
            /// The time r 7_ star t 8.
            /// </summary>
            TIMER7_START8 = 638,

            /// <summary>
            /// The time r 7_ en d 8.
            /// </summary>
            TIMER7_END8 = 639,

            /// <summary>
            /// The time r 8_ mode.
            /// </summary>
            TIMER8_MODE = 640,

            /// <summary>
            /// The time r 8_ count.
            /// </summary>
            TIMER8_COUNT = 641,

            /// <summary>
            /// The time r 8_ offset.
            /// </summary>
            TIMER8_OFFSET = 642,

            /// <summary>
            /// The time r 8_ do w_ int.
            /// </summary>
            TIMER8_DOW_INT = 643,

            /// <summary>
            /// The time r 8_ star t 1.
            /// </summary>
            TIMER8_START1 = 644,

            /// <summary>
            /// The time r 8_ en d 1.
            /// </summary>
            TIMER8_END1 = 645,

            /// <summary>
            /// The time r 8_ star t 2.
            /// </summary>
            TIMER8_START2 = 646,

            /// <summary>
            /// The time r 8_ en d 2.
            /// </summary>
            TIMER8_END2 = 647,

            /// <summary>
            /// The time r 8_ star t 3.
            /// </summary>
            TIMER8_START3 = 648,

            /// <summary>
            /// The time r 8_ en d 3.
            /// </summary>
            TIMER8_END3 = 649,

            /// <summary>
            /// The time r 8_ star t 4.
            /// </summary>
            TIMER8_START4 = 650,

            /// <summary>
            /// The time r 8_ en d 4.
            /// </summary>
            TIMER8_END4 = 651,

            /// <summary>
            /// The time r 8_ star t 5.
            /// </summary>
            TIMER8_START5 = 652,

            /// <summary>
            /// The time r 8_ en d 5.
            /// </summary>
            TIMER8_END5 = 653,

            /// <summary>
            /// The time r 8_ star t 6.
            /// </summary>
            TIMER8_START6 = 654,

            /// <summary>
            /// The time r 8_ en d 6.
            /// </summary>
            TIMER8_END6 = 655,

            /// <summary>
            /// The time r 8_ star t 7.
            /// </summary>
            TIMER8_START7 = 656,

            /// <summary>
            /// The time r 8_ en d 7.
            /// </summary>
            TIMER8_END7 = 657,

            /// <summary>
            /// The time r 8_ star t 8.
            /// </summary>
            TIMER8_START8 = 658,

            /// <summary>
            /// The time r 8_ en d 8.
            /// </summary>
            TIMER8_END8 = 659,

            // Configuration 1-10V-Schnittstellen
            // #define  ONETOTENVINTPARABLOCKSIZE=3,

            /// <summary>
            /// The l 1 t o 10 vin t 1_ umin.
            /// </summary>
            L1TO10VINT1_UMIN = 660,

            /// <summary>
            /// The l 1 t o 10 vin t 1_ umax.
            /// </summary>
            L1TO10VINT1_UMAX = 661,

            /// <summary>
            /// The l 1 t o 10 vin t 1_ function.
            /// </summary>
            L1TO10VINT1_FUNCTION = 662,

            /// <summary>
            /// The l 1 t o 10 vin t 2_ umin.
            /// </summary>
            L1TO10VINT2_UMIN = 663,

            /// <summary>
            /// The l 1 t o 10 vin t 2_ umax.
            /// </summary>
            L1TO10VINT2_UMAX = 664,

            /// <summary>
            /// The l 1 t o 10 vin t 2_ function.
            /// </summary>
            L1TO10VINT2_FUNCTION = 665,

            /// <summary>
            /// The l 1 t o 10 vin t 3_ umin.
            /// </summary>
            L1TO10VINT3_UMIN = 666,

            /// <summary>
            /// The l 1 t o 10 vin t 3_ umax.
            /// </summary>
            L1TO10VINT3_UMAX = 667,

            /// <summary>
            /// The l 1 t o 10 vin t 3_ function.
            /// </summary>
            L1TO10VINT3_FUNCTION = 668,

            /// <summary>
            /// The l 1 t o 10 vin t 4_ umin.
            /// </summary>
            L1TO10VINT4_UMIN = 669,

            /// <summary>
            /// The l 1 t o 10 vin t 4_ umax.
            /// </summary>
            L1TO10VINT4_UMAX = 670,

            /// <summary>
            /// The l 1 t o 10 vin t 4_ function.
            /// </summary>
            L1TO10VINT4_FUNCTION = 671,

            /// <summary>
            /// The l 1 t o 10 vin t 5_ umin.
            /// </summary>
            L1TO10VINT5_UMIN = 672,

            /// <summary>
            /// The l 1 t o 10 vin t 5_ umax.
            /// </summary>
            L1TO10VINT5_UMAX = 673,

            /// <summary>
            /// The l 1 t o 10 vin t 5_ function.
            /// </summary>
            L1TO10VINT5_FUNCTION = 674,

            /// <summary>
            /// The l 1 t o 10 vin t 6_ umin.
            /// </summary>
            L1TO10VINT6_UMIN = 675,

            /// <summary>
            /// The l 1 t o 10 vin t 6_ umax.
            /// </summary>
            L1TO10VINT6_UMAX = 676,

            /// <summary>
            /// The l 1 t o 10 vin t 6_ function.
            /// </summary>
            L1TO10VINT6_FUNCTION = 677,

            /// <summary>
            /// The l 1 t o 10 vin t 7_ umin.
            /// </summary>
            L1TO10VINT7_UMIN = 678,

            /// <summary>
            /// The l 1 t o 10 vin t 7_ umax.
            /// </summary>
            L1TO10VINT7_UMAX = 679,

            /// <summary>
            /// The l 1 t o 10 vin t 7_ function.
            /// </summary>
            L1TO10VINT7_FUNCTION = 680,

            /// <summary>
            /// The l 1 t o 10 vin t 8_ umin.
            /// </summary>
            L1TO10VINT8_UMIN = 681,

            /// <summary>
            /// The l 1 t o 10 vin t 8_ umax.
            /// </summary>
            L1TO10VINT8_UMAX = 682,

            /// <summary>
            /// The l 1 t o 10 vin t 8_ function.
            /// </summary>
            L1TO10VINT8_FUNCTION = 683,

            // Configuration Steckdosen

            /// <summary>
            /// The switchplu g 1_ function.
            /// </summary>
            SWITCHPLUG1_FUNCTION = 684,

            /// <summary>
            /// The switchplu g 2_ function.
            /// </summary>
            SWITCHPLUG2_FUNCTION = 685,

            /// <summary>
            /// The switchplu g 3_ function.
            /// </summary>
            SWITCHPLUG3_FUNCTION = 686,

            /// <summary>
            /// The switchplu g 4_ function.
            /// </summary>
            SWITCHPLUG4_FUNCTION = 687,

            /// <summary>
            /// The switchplu g 5_ function.
            /// </summary>
            SWITCHPLUG5_FUNCTION = 688,

            /// <summary>
            /// The switchplu g 6_ function.
            /// </summary>
            SWITCHPLUG6_FUNCTION = 689,

            /// <summary>
            /// The switchplu g 7_ function.
            /// </summary>
            SWITCHPLUG7_FUNCTION = 690,

            /// <summary>
            /// The switchplu g 8_ function.
            /// </summary>
            SWITCHPLUG8_FUNCTION = 691,

            /// <summary>
            /// The switchplu g 9_ function.
            /// </summary>
            SWITCHPLUG9_FUNCTION = 692,

            /// <summary>
            /// The switchplu g 10_ function.
            /// </summary>
            SWITCHPLUG10_FUNCTION = 693,

            /// <summary>
            /// The switchplu g 11_ function.
            /// </summary>
            SWITCHPLUG11_FUNCTION = 694,

            /// <summary>
            /// The switchplu g 12_ function.
            /// </summary>
            SWITCHPLUG12_FUNCTION = 695,

            /// <summary>
            /// The switchplu g 13_ function.
            /// </summary>
            SWITCHPLUG13_FUNCTION = 696,

            /// <summary>
            /// The switchplu g 14_ function.
            /// </summary>
            SWITCHPLUG14_FUNCTION = 697,

            /// <summary>
            /// The switchplu g 15_ function.
            /// </summary>
            SWITCHPLUG15_FUNCTION = 698,

            /// <summary>
            /// The switchplu g 16_ function.
            /// </summary>
            SWITCHPLUG16_FUNCTION = 699,

            /// <summary>
            /// The alarmbeepmode.
            /// </summary>
            ALARMBEEPMODE = 701,

            /// <summary>
            /// The alarmbeepstarttime.
            /// </summary>
            ALARMBEEPSTARTTIME = 702,

            /// <summary>
            /// The alarmbeependtime.
            /// </summary>
            ALARMBEEPENDTIME = 703,

            /// <summary>
            /// The feedpaus e_ duration.
            /// </summary>
            FEEDPAUSE_DURATION = 710,

            /// <summary>
            /// The feedpaus e_ mode.
            /// </summary>
            FEEDPAUSE_MODE = 711,

            /// <summary>
            /// The rainingday s_ dow.
            /// </summary>
            RAININGDAYS_DOW = 712,

            /// <summary>
            /// The rainingday s_ illuminationchannels.
            /// </summary>
            RAININGDAYS_ILLUMINATIONCHANNELS = 713,

            /// <summary>
            /// The rainingday s_ darkening.
            /// </summary>
            RAININGDAYS_DARKENING = 714,

            /// <summary>
            /// The variableilluminatio n 1_ l.
            /// </summary>
            VARIABLEILLUMINATION1_L = 715,

            /// <summary>
            /// The variableilluminatio n 1_ h.
            /// </summary>
            VARIABLEILLUMINATION1_H = 716,

            /// <summary>
            /// The variableilluminatio n 2_ l.
            /// </summary>
            VARIABLEILLUMINATION2_L = 717,

            /// <summary>
            /// The variableilluminatio n 2_ h.
            /// </summary>
            VARIABLEILLUMINATION2_H = 718,

            /// <summary>
            /// The variableilluminatio n 3_ l.
            /// </summary>
            VARIABLEILLUMINATION3_L = 719,

            /// <summary>
            /// The variableilluminatio n 3_ h.
            /// </summary>
            VARIABLEILLUMINATION3_H = 720,

            /// <summary>
            /// The variableilluminatio n 4_ l.
            /// </summary>
            VARIABLEILLUMINATION4_L = 721,

            /// <summary>
            /// The variableilluminatio n 4_ h.
            /// </summary>
            VARIABLEILLUMINATION4_H = 722,

            /// <summary>
            /// The digitalinpu t 1_ function.
            /// </summary>
            DIGITALINPUT1_FUNCTION = 723,

            /// <summary>
            /// The digitalinpu t 2_ function.
            /// </summary>
            DIGITALINPUT2_FUNCTION = 724,

            /// <summary>
            /// The digitalinpu t 3_ function.
            /// </summary>
            DIGITALINPUT3_FUNCTION = 725,

            /// <summary>
            /// The digitalinpu t 4_ function.
            /// </summary>
            DIGITALINPUT4_FUNCTION = 726,

            // 727-734 free

            // COM (weitere unten)

            /// <summary>
            /// The co m_ isrouter.
            /// </summary>
            COM_ISROUTER = 735,

            /// <summary>
            /// The co m 1_ baudrate.
            /// </summary>
            COM1_BAUDRATE = 736,

            /// <summary>
            /// The co m 2_ baudrate.
            /// </summary>
            COM2_BAUDRATE = 737,

            /// <summary>
            /// The co m 3_ baudrate.
            /// </summary>
            COM3_BAUDRATE = 738,

            /// <summary>
            /// The co m 4_ baudrate.
            /// </summary>
            COM4_BAUDRATE = 739,

            // Gewitter (weitere unten)

            /// <summary>
            /// The thunderstor m_ intensity.
            /// </summary>
            THUNDERSTORM_INTENSITY = 740,

            /// <summary>
            /// The thunderstor m_ darkening.
            /// </summary>
            THUNDERSTORM_DARKENING = 741,

            /// <summary>
            /// The thunderstor m_ count.
            /// </summary>
            THUNDERSTORM_COUNT = 742,

            /// <summary>
            /// The thunderstor m_ star t 1.
            /// </summary>
            THUNDERSTORM_START1 = 743,

            /// <summary>
            /// The thunderstor m_ duratio n 1.
            /// </summary>
            THUNDERSTORM_DURATION1 = 744,

            /// <summary>
            /// The thunderstor m_ star t 2.
            /// </summary>
            THUNDERSTORM_START2 = 745,

            /// <summary>
            /// The thunderstor m_ duratio n 2.
            /// </summary>
            THUNDERSTORM_DURATION2 = 746,

            /// <summary>
            /// The thunderstor m_ star t 3.
            /// </summary>
            THUNDERSTORM_START3 = 747,

            /// <summary>
            /// The thunderstor m_ duratio n 3.
            /// </summary>
            THUNDERSTORM_DURATION3 = 748,

            /// <summary>
            /// The thunderstor m_ star t 4.
            /// </summary>
            THUNDERSTORM_START4 = 749,

            /// <summary>
            /// The thunderstor m_ duratio n 4.
            /// </summary>
            THUNDERSTORM_DURATION4 = 750,

            /// <summary>
            /// The thunderstor m_ dow.
            /// </summary>
            THUNDERSTORM_DOW = 751,

            /// <summary>
            /// The thunderstor m_ rndduration.
            /// </summary>
            THUNDERSTORM_RNDDURATION = 752,

            /// <summary>
            /// The thunderstor m_ rndminwait.
            /// </summary>
            THUNDERSTORM_RNDMINWAIT = 753,

            /// <summary>
            /// The thunderstor m_ rndmaxwait.
            /// </summary>
            THUNDERSTORM_RNDMAXWAIT = 754,

            // temperaturabhängige Lichtreduzierung

            /// <summary>
            /// The tempdeplightre d_ illuminationchannels.
            /// </summary>
            TEMPDEPLIGHTRED_ILLUMINATIONCHANNELS = 755,

            /// <summary>
            /// The tempdeplightre d_ deltatmin.
            /// </summary>
            TEMPDEPLIGHTRED_DELTATMIN = 756,

            /// <summary>
            /// The tempdeplightre d_ deltatmax.
            /// </summary>
            TEMPDEPLIGHTRED_DELTATMAX = 757,

            /// <summary>
            /// The tempdeplightre d_ sensorindex.
            /// </summary>
            TEMPDEPLIGHTRED_SENSORINDEX = 758,

            /// <summary>
            /// The tempdeplightre d_ deltatshutoff.
            /// </summary>
            TEMPDEPLIGHTRED_DELTATSHUTOFF = 759,

            // Level control (weitere unten)

            /// <summary>
            /// The leve l_ opmode.
            /// </summary>
            LEVEL_OPMODE = 760,

            /// <summary>
            /// The leve l_ reactduratio n 1.
            /// </summary>
            LEVEL_REACTDURATION1 = 761,

            /// <summary>
            /// The leve l_ reactduratio n 2.
            /// </summary>
            LEVEL_REACTDURATION2 = 762,

            /// <summary>
            /// The leve l_ maxduratio n 1.
            /// </summary>
            LEVEL_MAXDURATION1 = 763,

            /// <summary>
            /// The leve l_ maxduratio n 2.
            /// </summary>
            LEVEL_MAXDURATION2 = 764,

            /// <summary>
            /// The leve l_ alarmenabled.
            /// </summary>
            LEVEL_ALARMENABLED = 765,

            /// <summary>
            /// The leve l_ selectedtimer.
            /// </summary>
            LEVEL_SELECTEDTIMER = 766,

            // 767 free seit 4.00
            // ab 3.04

            /// <summary>
            /// The leve l_ reactduratio n 3.
            /// </summary>
            LEVEL_REACTDURATION3 = 768,

            /// <summary>
            /// The leve l_ maxduratio n 3.
            /// </summary>
            LEVEL_MAXDURATION3 = 769,

            // Indicator attitudes

            /// <summary>
            /// The displa y_ changetime.
            /// </summary>
            DISPLAY_CHANGETIME = 770,

            /// <summary>
            /// The displa y_ showmas k 1.
            /// </summary>
            DISPLAY_SHOWMASK1 = 771,

            /// <summary>
            /// The displa y_ showmas k 2.
            /// </summary>
            DISPLAY_SHOWMASK2 = 772,

            /// <summary>
            /// The displa y_ datetimemode.
            /// </summary>
            DISPLAY_DATETIMEMODE = 773,

            // Recording of measurement (further down)

            /// <summary>
            /// The measuremen t_ sampleperiod.
            /// </summary>
            MEASUREMENT_SAMPLEPERIOD = 780,

            /// <summary>
            /// The measuremen t_ samplesourcemask.
            /// </summary>
            MEASUREMENT_SAMPLESOURCEMASK = 781,

            /// <summary>
            /// The measuremen t_ maxmemorysize.
            /// </summary>
            MEASUREMENT_MAXMEMORYSIZE = 782,

            // 783 ... 789 free

            // Fernsteuerung

            /// <summary>
            /// The remot e_ comport.
            /// </summary>
            REMOTE_COMPORT = 790,

            /// <summary>
            /// The remot e_ timesync.
            /// </summary>
            REMOTE_TIMESYNC = 791,

            // 792 ... 799 free

            // Maintenance

            /// <summary>
            /// The maintenanc e_ spselmas k 1.
            /// </summary>
            MAINTENANCE_SPSELMASK1 = 800,

            /// <summary>
            /// The maintenanc e_ spselmas k 2.
            /// </summary>
            MAINTENANCE_SPSELMASK2 = 801,

            /// <summary>
            /// The maintenanc e_ spstatemas k 1.
            /// </summary>
            MAINTENANCE_SPSTATEMASK1 = 802,

            /// <summary>
            /// The maintenanc e_ spstatemas k 2.
            /// </summary>
            MAINTENANCE_SPSTATEMASK2 = 803,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 1.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT1 = 804,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 2.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT2 = 805,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 3.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT3 = 806,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 4.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT4 = 807,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 5.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT5 = 808,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 6.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT6 = 809,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 7.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT7 = 810,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 8.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT8 = 811,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 9.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT9 = 812,

            /// <summary>
            /// The maintenanc e_ onetotenpercen t 10.
            /// </summary>
            MAINTENANCE_ONETOTENPERCENT10 = 813,

            /// <summary>
            /// The maintenanc e_ onetotenselmask.
            /// </summary>
            MAINTENANCE_ONETOTENSELMASK = 814,

            /// <summary>
            /// The maintenanc e_ timeout.
            /// </summary>
            MAINTENANCE_TIMEOUT = 815,

            /// <summary>
            /// The digitalpowerbaravailable.
            /// </summary>
            DIGITALPOWERBARAVAILABLE = 819,

            /// <summary>
            /// The switchplu g 17_ function.
            /// </summary>
            SWITCHPLUG17_FUNCTION = 820,

            /// <summary>
            /// The switchplu g 18_ function.
            /// </summary>
            SWITCHPLUG18_FUNCTION = 821,

            /// <summary>
            /// The switchplu g 19_ function.
            /// </summary>
            SWITCHPLUG19_FUNCTION = 822,

            /// <summary>
            /// The switchplu g 20_ function.
            /// </summary>
            SWITCHPLUG20_FUNCTION = 823,

            /// <summary>
            /// The switchplu g 21_ function.
            /// </summary>
            SWITCHPLUG21_FUNCTION = 824,

            /// <summary>
            /// The switchplu g 22_ function.
            /// </summary>
            SWITCHPLUG22_FUNCTION = 825,

            /// <summary>
            /// The switchplu g 23_ function.
            /// </summary>
            SWITCHPLUG23_FUNCTION = 826,

            /// <summary>
            /// The switchplu g 24_ function.
            /// </summary>
            SWITCHPLUG24_FUNCTION = 827,

            // Codes Dosierpumpen (3 pro Dosierpumpe, 8 Dosierpumpen)
            // #define  DOSINGPUMPPARABLOCKSIZE=3,
            // Dosierpumpe= 1,

            /// <summary>
            /// The dosingpum p 1_ flowrate.
            /// </summary>
            DOSINGPUMP1_FLOWRATE = 828,

            /// <summary>
            /// The dosingpum p 1_ rateperdosing.
            /// </summary>
            DOSINGPUMP1_RATEPERDOSING = 829,

            /// <summary>
            /// The dosingpum p 1_ dosingsperday.
            /// </summary>
            DOSINGPUMP1_DOSINGSPERDAY = 830,

            /// <summary>
            /// The dosingpum p 2_ flowrate.
            /// </summary>
            DOSINGPUMP2_FLOWRATE = 831,

            /// <summary>
            /// The dosingpum p 2_ rateperdosing.
            /// </summary>
            DOSINGPUMP2_RATEPERDOSING = 832,

            /// <summary>
            /// The dosingpum p 2_ dosingsperday.
            /// </summary>
            DOSINGPUMP2_DOSINGSPERDAY = 833,

            /// <summary>
            /// The dosingpum p 3_ flowrate.
            /// </summary>
            DOSINGPUMP3_FLOWRATE = 834,

            /// <summary>
            /// The dosingpum p 3_ rateperdosing.
            /// </summary>
            DOSINGPUMP3_RATEPERDOSING = 835,

            /// <summary>
            /// The dosingpum p 3_ dosingsperday.
            /// </summary>
            DOSINGPUMP3_DOSINGSPERDAY = 836,

            /// <summary>
            /// The dosingpum p 4_ flowrate.
            /// </summary>
            DOSINGPUMP4_FLOWRATE = 837,

            /// <summary>
            /// The dosingpum p 4_ rateperdosing.
            /// </summary>
            DOSINGPUMP4_RATEPERDOSING = 838,

            /// <summary>
            /// The dosingpum p 4_ dosingsperday.
            /// </summary>
            DOSINGPUMP4_DOSINGSPERDAY = 839,

            /// <summary>
            /// The dosingpum p 5_ flowrate.
            /// </summary>
            DOSINGPUMP5_FLOWRATE = 840,

            /// <summary>
            /// The dosingpum p 5_ rateperdosing.
            /// </summary>
            DOSINGPUMP5_RATEPERDOSING = 841,

            /// <summary>
            /// The dosingpum p 5_ dosingsperday.
            /// </summary>
            DOSINGPUMP5_DOSINGSPERDAY = 842,

            /// <summary>
            /// The dosingpum p 6_ flowrate.
            /// </summary>
            DOSINGPUMP6_FLOWRATE = 843,

            /// <summary>
            /// The dosingpum p 6_ rateperdosing.
            /// </summary>
            DOSINGPUMP6_RATEPERDOSING = 844,

            /// <summary>
            /// The dosingpum p 6_ dosingsperday.
            /// </summary>
            DOSINGPUMP6_DOSINGSPERDAY = 845,

            /// <summary>
            /// The dosingpum p 7_ flowrate.
            /// </summary>
            DOSINGPUMP7_FLOWRATE = 846,

            /// <summary>
            /// The dosingpum p 7_ rateperdosing.
            /// </summary>
            DOSINGPUMP7_RATEPERDOSING = 847,

            /// <summary>
            /// The dosingpum p 7_ dosingsperday.
            /// </summary>
            DOSINGPUMP7_DOSINGSPERDAY = 848,

            /// <summary>
            /// The dosingpum p 8_ flowrate.
            /// </summary>
            DOSINGPUMP8_FLOWRATE = 849,

            /// <summary>
            /// The dosingpum p 8_ rateperdosing.
            /// </summary>
            DOSINGPUMP8_RATEPERDOSING = 850,

            /// <summary>
            /// The dosingpum p 8_ dosingsperday.
            /// </summary>
            DOSINGPUMP8_DOSINGSPERDAY = 851,

            /// <summary>
            /// The dal i_ mindim.
            /// </summary>
            DALI_MINDIM = 853,

            /// <summary>
            /// The l 1 t o 10 vin t 9_ umin.
            /// </summary>
            L1TO10VINT9_UMIN = 854,

            /// <summary>
            /// The l 1 t o 10 vin t 9_ umax.
            /// </summary>
            L1TO10VINT9_UMAX = 855,

            /// <summary>
            /// The l 1 t o 10 vin t 9_ function.
            /// </summary>
            L1TO10VINT9_FUNCTION = 856,

            /// <summary>
            /// The l 1 t o 10 vin t 10_ umin.
            /// </summary>
            L1TO10VINT10_UMIN = 857,

            /// <summary>
            /// The l 1 t o 10 vin t 10_ umax.
            /// </summary>
            L1TO10VINT10_UMAX = 858,

            /// <summary>
            /// The l 1 t o 10 vin t 10_ function.
            /// </summary>
            L1TO10VINT10_FUNCTION = 859,

            // Current

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

            // 2 free

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
            /// The currentcontro l_ grou p 1 waveduration.
            /// </summary>
            CURRENTCONTROL_GROUP1WAVEDURATION = 869,

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

            // 2 free

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
            /// The currentcontro l_ grou p 2 waveduration.
            /// </summary>
            CURRENTCONTROL_GROUP2WAVEDURATION = 879,

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

            // 2 free

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

            // 2 free

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

            // 2 free

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

            // 2 free

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

            // ab 4.00
            // Code for progr. Logic

            /// <summary>
            /// The proglogi c 1_ input.
            /// </summary>
            PROGLOGIC1_INPUT = 912,

            /// <summary>
            /// The proglogi c 1_ function.
            /// </summary>
            PROGLOGIC1_FUNCTION = 913,

            /// <summary>
            /// The proglogi c 1_ time.
            /// </summary>
            PROGLOGIC1_TIME = 914,

            /// <summary>
            /// The proglogi c 2_ input.
            /// </summary>
            PROGLOGIC2_INPUT = 915,

            /// <summary>
            /// The proglogi c 2_ function.
            /// </summary>
            PROGLOGIC2_FUNCTION = 916,

            /// <summary>
            /// The proglogi c 2_ time.
            /// </summary>
            PROGLOGIC2_TIME = 917,

            /// <summary>
            /// The proglogi c 3_ input.
            /// </summary>
            PROGLOGIC3_INPUT = 918,

            /// <summary>
            /// The proglogi c 3_ function.
            /// </summary>
            PROGLOGIC3_FUNCTION = 919,

            /// <summary>
            /// The proglogi c 3_ time.
            /// </summary>
            PROGLOGIC3_TIME = 920,

            /// <summary>
            /// The proglogi c 4_ input.
            /// </summary>
            PROGLOGIC4_INPUT = 921,

            /// <summary>
            /// The proglogi c 4_ function.
            /// </summary>
            PROGLOGIC4_FUNCTION = 922,

            /// <summary>
            /// The proglogi c 4_ time.
            /// </summary>
            PROGLOGIC4_TIME = 923,

            /// <summary>
            /// The proglogi c 5_ input.
            /// </summary>
            PROGLOGIC5_INPUT = 924,

            /// <summary>
            /// The proglogi c 5_ function.
            /// </summary>
            PROGLOGIC5_FUNCTION = 925,

            /// <summary>
            /// The proglogi c 5_ time.
            /// </summary>
            PROGLOGIC5_TIME = 926,

            /// <summary>
            /// The proglogi c 6_ input.
            /// </summary>
            PROGLOGIC6_INPUT = 927,

            /// <summary>
            /// The proglogi c 6_ function.
            /// </summary>
            PROGLOGIC6_FUNCTION = 928,

            /// <summary>
            /// The proglogi c 6_ time.
            /// </summary>
            PROGLOGIC6_TIME = 929,

            /// <summary>
            /// The proglogi c 7_ input.
            /// </summary>
            PROGLOGIC7_INPUT = 930,

            /// <summary>
            /// The proglogi c 7_ function.
            /// </summary>
            PROGLOGIC7_FUNCTION = 931,

            /// <summary>
            /// The proglogi c 7_ time.
            /// </summary>
            PROGLOGIC7_TIME = 932,

            /// <summary>
            /// The proglogi c 8_ input.
            /// </summary>
            PROGLOGIC8_INPUT = 933,

            /// <summary>
            /// The proglogi c 8_ function.
            /// </summary>
            PROGLOGIC8_FUNCTION = 934,

            /// <summary>
            /// The proglogi c 8_ time.
            /// </summary>
            PROGLOGIC8_TIME = 935,

            // further codes for sensors
            // #define BLOCK2SIZE_SENSORPARA=4,

            /// <summary>
            /// The sensorpar a 1_ controllingmode.
            /// </summary>
            SENSORPARA1_CONTROLLINGMODE = 948,

            // ab 3.04

            /// <summary>
            /// The sensorpar a 1_ par a 4.
            /// </summary>
            SENSORPARA1_PARA4 = 949,

            /// <summary>
            /// The sensorpar a 1_ par a 5.
            /// </summary>
            SENSORPARA1_PARA5 = 950,

            // 951 free
            // ...

            /// <summary>
            /// The sensorpar a 8_ par a 5.
            /// </summary>
            SENSORPARA8_PARA5 = 978,

            // 979 free

            /// <summary>
            /// The ca l 1 ad c_ tem p 1.
            /// </summary>
            CAL1ADC_TEMP1 = 982,

            /// <summary>
            /// The ca l 2 ad c_ tem p 1.
            /// </summary>
            CAL2ADC_TEMP1 = 983,

            // no codes starting from 984 are stored in the EEPROM!!! 
            // da kommen feste EEPROM-Adressen!!!

            /// <summary>
            /// The sensorpar a 1_ actvalue.
            /// </summary>
            SENSORPARA1_ACTVALUE = 1000,

            /// <summary>
            /// The sensorpar a 1_ actadc.
            /// </summary>
            SENSORPARA1_ACTADC = 1001,

            /// <summary>
            /// The sensorpar a 1_ ohm.
            /// </summary>
            SENSORPARA1_OHM = 1002,

            /// <summary>
            /// The sensorpar a 1_ actstate.
            /// </summary>
            SENSORPARA1_ACTSTATE = 1003,

            // ...

            /// <summary>
            /// The sensorpar a 8_ actstate.
            /// </summary>
            SENSORPARA8_ACTSTATE = 1031,

            /// <summary>
            /// The illuminatio n 1_ actpercent.
            /// </summary>
            ILLUMINATION1_ACTPERCENT = 1032,

            /// <summary>
            /// The illuminatio n 1_ ohm.
            /// </summary>
            ILLUMINATION1_OHM = 1033,

            /// <summary>
            /// The illuminatio n 2_ actpercent.
            /// </summary>
            ILLUMINATION2_ACTPERCENT = 1034,

            /// <summary>
            /// The illuminatio n 2_ ohm.
            /// </summary>
            ILLUMINATION2_OHM = 1035,

            /// <summary>
            /// The illuminatio n 3_ actpercent.
            /// </summary>
            ILLUMINATION3_ACTPERCENT = 1036,

            /// <summary>
            /// The illuminatio n 3_ ohm.
            /// </summary>
            ILLUMINATION3_OHM = 1037,

            /// <summary>
            /// The illuminatio n 4_ actpercent.
            /// </summary>
            ILLUMINATION4_ACTPERCENT = 1038,

            /// <summary>
            /// The illuminatio n 4_ ohm.
            /// </summary>
            ILLUMINATION4_OHM = 1039,

            /// <summary>
            /// The illuminatio n 5_ actpercent.
            /// </summary>
            ILLUMINATION5_ACTPERCENT = 1040,

            /// <summary>
            /// The illuminatio n 5_ ohm.
            /// </summary>
            ILLUMINATION5_OHM = 1041,

            /// <summary>
            /// The illuminatio n 6_ actpercent.
            /// </summary>
            ILLUMINATION6_ACTPERCENT = 1042,

            /// <summary>
            /// The illuminatio n 6_ ohm.
            /// </summary>
            ILLUMINATION6_OHM = 1043,

            /// <summary>
            /// The illuminatio n 7_ actpercent.
            /// </summary>
            ILLUMINATION7_ACTPERCENT = 1044,

            /// <summary>
            /// The illuminatio n 7_ ohm.
            /// </summary>
            ILLUMINATION7_OHM = 1045,

            /// <summary>
            /// The illuminatio n 8_ actpercent.
            /// </summary>
            ILLUMINATION8_ACTPERCENT = 1046,

            /// <summary>
            /// The illuminatio n 8_ ohm.
            /// </summary>
            ILLUMINATION8_OHM = 1047,

            /// <summary>
            /// The thunderstor m_ manustart.
            /// </summary>
            THUNDERSTORM_MANUSTART = 1050,

            /// <summary>
            /// The thunderstor m_ manuflash.
            /// </summary>
            THUNDERSTORM_MANUFLASH = 1051,

            /// <summary>
            /// The progra m_ le d_ light.
            /// </summary>
            PROGRAM_LED_LIGHT = 1052,

            // new since 3.05:

            /// <summary>
            /// The setdigpbnumbering.
            /// </summary>
            SETDIGPBNUMBERING = 1054,

            /// <summary>
            /// The setdigpbinistate.
            /// </summary>
            SETDIGPBINISTATE = 1055,

            // COM

            /// <summary>
            /// The co m_ portcount.
            /// </summary>
            COM_PORTCOUNT = 1056,

            // View

            /// <summary>
            /// The vie w_ pwmcontrast.
            /// </summary>
            VIEW_PWMCONTRAST = 1060,

            // Level regulation

            /// <summary>
            /// The leve l_ actstat e 1.
            /// </summary>
            LEVEL_ACTSTATE1 = 1070,

            /// <summary>
            /// The leve l_ actstat e 2.
            /// </summary>
            LEVEL_ACTSTATE2 = 1071,

            /// <summary>
            /// The leve l_ startwaterchange.
            /// </summary>
            LEVEL_STARTWATERCHANGE = 1072,

            // new since 3.06: Act. Values pumps

            /// <summary>
            /// The currentpum p 1_ actpercent.
            /// </summary>
            CURRENTPUMP1_ACTPERCENT = 1080,

            /// <summary>
            /// The currentpum p 2_ actpercent.
            /// </summary>
            CURRENTPUMP2_ACTPERCENT = 1081,

            /// <summary>
            /// The currentpum p 3_ actpercent.
            /// </summary>
            CURRENTPUMP3_ACTPERCENT = 1082,

            /// <summary>
            /// The currentpum p 4_ actpercent.
            /// </summary>
            CURRENTPUMP4_ACTPERCENT = 1083,

            // Alarm

            /// <summary>
            /// The isalarm.
            /// </summary>
            ISALARM = 1090,

            // Diagnose-/Start-up parameter

            /// <summary>
            /// The lightscenetesttime.
            /// </summary>
            LIGHTSCENETESTTIME = 1099,

            /// <summary>
            /// The r w_ eeprom.
            /// </summary>
            RW_EEPROM = 1100,

            /// <summary>
            /// The opmode.
            /// </summary>
            OPMODE = 1101,

            /// <summary>
            /// The s p 1_ state.
            /// </summary>
            SP1_STATE = 1102,

            /// <summary>
            /// The s p 2_ state.
            /// </summary>
            SP2_STATE = 1103,

            /// <summary>
            /// The s p 3_ state.
            /// </summary>
            SP3_STATE = 1104,

            /// <summary>
            /// The s p 4_ state.
            /// </summary>
            SP4_STATE = 1105,

            /// <summary>
            /// The s p 5_ state.
            /// </summary>
            SP5_STATE = 1106,

            /// <summary>
            /// The s p 6_ state.
            /// </summary>
            SP6_STATE = 1107,

            /// <summary>
            /// The s p 7_ state.
            /// </summary>
            SP7_STATE = 1108,

            /// <summary>
            /// The s p 8_ state.
            /// </summary>
            SP8_STATE = 1109,

            /// <summary>
            /// The s p 9_ state.
            /// </summary>
            SP9_STATE = 1110,

            /// <summary>
            /// The s p 10_ state.
            /// </summary>
            SP10_STATE = 1111,

            /// <summary>
            /// The s p 11_ state.
            /// </summary>
            SP11_STATE = 1112,

            /// <summary>
            /// The s p 12_ state.
            /// </summary>
            SP12_STATE = 1113,

            /// <summary>
            /// The s p 13_ state.
            /// </summary>
            SP13_STATE = 1114,

            /// <summary>
            /// The s p 14_ state.
            /// </summary>
            SP14_STATE = 1115,

            /// <summary>
            /// The s p 15_ state.
            /// </summary>
            SP15_STATE = 1116,

            /// <summary>
            /// The s p 16_ state.
            /// </summary>
            SP16_STATE = 1117,

            /// <summary>
            /// The l 1 t o 10 vin t 1_ pwmvalue.
            /// </summary>
            L1TO10VINT1_PWMVALUE = 1120,

            /// <summary>
            /// The l 1 t o 10 vin t 2_ pwmvalue.
            /// </summary>
            L1TO10VINT2_PWMVALUE = 1121,

            /// <summary>
            /// The l 1 t o 10 vin t 3_ pwmvalue.
            /// </summary>
            L1TO10VINT3_PWMVALUE = 1122,

            /// <summary>
            /// The l 1 t o 10 vin t 4_ pwmvalue.
            /// </summary>
            L1TO10VINT4_PWMVALUE = 1123,

            /// <summary>
            /// The l 1 t o 10 vin t 5_ pwmvalue.
            /// </summary>
            L1TO10VINT5_PWMVALUE = 1124,

            /// <summary>
            /// The l 1 t o 10 vin t 6_ pwmvalue.
            /// </summary>
            L1TO10VINT6_PWMVALUE = 1125,

            /// <summary>
            /// The l 1 t o 10 vin t 7_ pwmvalue.
            /// </summary>
            L1TO10VINT7_PWMVALUE = 1126,

            /// <summary>
            /// The l 1 t o 10 vin t 8_ pwmvalue.
            /// </summary>
            L1TO10VINT8_PWMVALUE = 1127,

            /// <summary>
            /// The leve l 2 state.
            /// </summary>
            LEVEL2STATE = 1128,

            /// <summary>
            /// The slotcount.
            /// </summary>
            SLOTCOUNT = 1129,

            /// <summary>
            /// The ke y_ state.
            /// </summary>
            KEY_STATE = 1130,

            /// <summary>
            /// The buzzerstate.
            /// </summary>
            BUZZERSTATE = 1131,

            /// <summary>
            /// The alarmledstate.
            /// </summary>
            ALARMLEDSTATE = 1132,

            /// <summary>
            /// The dcfstate.
            /// </summary>
            DCFSTATE = 1134,

            /// <summary>
            /// The leve l 0 state.
            /// </summary>
            LEVEL0STATE = 1135,

            /// <summary>
            /// The leve l 1 state.
            /// </summary>
            LEVEL1STATE = 1136,

            /// <summary>
            /// The modu l 0 state.
            /// </summary>
            MODUL0STATE = 1137,

            /// <summary>
            /// The modu l 1 state.
            /// </summary>
            MODUL1STATE = 1138,

            /// <summary>
            /// The modu l 2 state.
            /// </summary>
            MODUL2STATE = 1139,

            /// <summary>
            /// The rtctickcount.
            /// </summary>
            RTCTICKCOUNT = 1140,

            /// <summary>
            /// The setlantronixconfi g_ slo t 0.
            /// </summary>
            SETLANTRONIXCONFIG_SLOT0 = 1141,

            /// <summary>
            /// The setlantronixconfi g_ slo t 1.
            /// </summary>
            SETLANTRONIXCONFIG_SLOT1 = 1142,

            /// <summary>
            /// The setlantronixconfi g_ slo t 2.
            /// </summary>
            SETLANTRONIXCONFIG_SLOT2 = 1143,

            /// <summary>
            /// The setlantronixconfigsecurit y_ slo t 0.
            /// </summary>
            SETLANTRONIXCONFIGSECURITY_SLOT0 = 1144,

            /// <summary>
            /// The setlantronixconfigsecurit y_ slo t 1.
            /// </summary>
            SETLANTRONIXCONFIGSECURITY_SLOT1 = 1145,

            /// <summary>
            /// The setlantronixconfigsecurit y_ slo t 2.
            /// </summary>
            SETLANTRONIXCONFIGSECURITY_SLOT2 = 1146,

            /// <summary>
            /// The sendkey.
            /// </summary>
            SENDKEY = 1150,

            /// <summary>
            /// The getdisplaylin e 1.
            /// </summary>
            GETDISPLAYLINE1 = 1151,

            /// <summary>
            /// The getdisplaylin e 2.
            /// </summary>
            GETDISPLAYLINE2 = 1152,

            /// <summary>
            /// The getdisplaystate.
            /// </summary>
            GETDISPLAYSTATE = 1153,

            /// <summary>
            /// The modu l 0 version.
            /// </summary>
            MODUL0VERSION = 1154,

            /// <summary>
            /// The modu l 1 version.
            /// </summary>
            MODUL1VERSION = 1155,

            /// <summary>
            /// The modu l 2 version.
            /// </summary>
            MODUL2VERSION = 1156,

            /// <summary>
            /// The modu l 0 id.
            /// </summary>
            MODUL0ID = 1157,

            /// <summary>
            /// The modu l 1 id.
            /// </summary>
            MODUL1ID = 1158,

            /// <summary>
            /// The modu l 2 id.
            /// </summary>
            MODUL2ID = 1159,

            // Recording of measurement

            /// <summary>
            /// The measuremen t_ newdatacount.
            /// </summary>
            MEASUREMENT_NEWDATACOUNT = 1160,

            /// <summary>
            /// The measuremen t_ usedmemorysize.
            /// </summary>
            MEASUREMENT_USEDMEMORYSIZE = 1162,

            /// <summary>
            /// The measuremen t_ lastsampletime.
            /// </summary>
            MEASUREMENT_LASTSAMPLETIME = 1163,

            /// <summary>
            /// The measuremen t_ lastsampledate.
            /// </summary>
            MEASUREMENT_LASTSAMPLEDATE = 1164,

            /// <summary>
            /// The measuremen t_ validdatacount.
            /// </summary>
            MEASUREMENT_VALIDDATACOUNT = 1165,

            /// <summary>
            /// The measuremen t_ getdatarecord.
            /// </summary>
            MEASUREMENT_GETDATARECORD = 1166,

            /// <summary>
            /// The measuremen t_ increaddatapointer.
            /// </summary>
            MEASUREMENT_INCREADDATAPOINTER = 1167,

            /// <summary>
            /// The s p 17_ state.
            /// </summary>
            SP17_STATE = 1180,

            /// <summary>
            /// The s p 18_ state.
            /// </summary>
            SP18_STATE = 1181,

            /// <summary>
            /// The s p 19_ state.
            /// </summary>
            SP19_STATE = 1182,

            /// <summary>
            /// The s p 20_ state.
            /// </summary>
            SP20_STATE = 1183,

            /// <summary>
            /// The s p 21_ state.
            /// </summary>
            SP21_STATE = 1184,

            /// <summary>
            /// The s p 22_ state.
            /// </summary>
            SP22_STATE = 1185,

            /// <summary>
            /// The s p 23_ state.
            /// </summary>
            SP23_STATE = 1186,

            /// <summary>
            /// The s p 24_ state.
            /// </summary>
            SP24_STATE = 1187,

            /// <summary>
            /// The s p_ al l_ state.
            /// </summary>
            SP_ALL_STATE = 1188,

            // Debugging

            /// <summary>
            /// The tes t 1.
            /// </summary>
            TEST1 = 1200,

            /// <summary>
            /// The tes t 2.
            /// </summary>
            TEST2 = 1201,

            /// <summary>
            /// The tes t 3.
            /// </summary>
            TEST3 = 1202,

            /// <summary>
            /// The tes t 4.
            /// </summary>
            TEST4 = 1203,

            /// <summary>
            /// The tes t 5.
            /// </summary>
            TEST5 = 1204,

            /// <summary>
            /// The tes t 6.
            /// </summary>
            TEST6 = 1205,

            /// <summary>
            /// The tes t 7.
            /// </summary>
            TEST7 = 1206,

            /// <summary>
            /// The tes t 8.
            /// </summary>
            TEST8 = 1207,

            /// <summary>
            /// The tes t 9.
            /// </summary>
            TEST9 = 1208,

            /// <summary>
            /// The tes t 10.
            /// </summary>
            TEST10 = 1209,

            /// <summary>
            /// The freestack.
            /// </summary>
            FREESTACK = 1210,

            // further codes for sensors

            /// <summary>
            /// The sensorpar a 1_ actanalogout.
            /// </summary>
            SENSORPARA1_ACTANALOGOUT = 1220,

            /// <summary>
            /// The sensorpar a 2_ actanalogout.
            /// </summary>
            SENSORPARA2_ACTANALOGOUT = 1221,

            /// <summary>
            /// The sensorpar a 3_ actanalogout.
            /// </summary>
            SENSORPARA3_ACTANALOGOUT = 1222,

            /// <summary>
            /// The sensorpar a 4_ actanalogout.
            /// </summary>
            SENSORPARA4_ACTANALOGOUT = 1223,

            /// <summary>
            /// The sensorpar a 5_ actanalogout.
            /// </summary>
            SENSORPARA5_ACTANALOGOUT = 1224,

            /// <summary>
            /// The sensorpar a 6_ actanalogout.
            /// </summary>
            SENSORPARA6_ACTANALOGOUT = 1225,

            /// <summary>
            /// The sensorpar a 7_ actanalogout.
            /// </summary>
            SENSORPARA7_ACTANALOGOUT = 1226,

            /// <summary>
            /// The sensorpar a 8_ actanalogout.
            /// </summary>
            SENSORPARA8_ACTANALOGOUT = 1227,

            /// <summary>
            /// The sensorpar a 1_ completestring.
            /// </summary>
            SENSORPARA1_COMPLETESTRING = 1228,

            /// <summary>
            /// The sensorpar a 2_ completestring.
            /// </summary>
            SENSORPARA2_COMPLETESTRING = 1229,

            /// <summary>
            /// The sensorpar a 3_ completestring.
            /// </summary>
            SENSORPARA3_COMPLETESTRING = 1230,

            /// <summary>
            /// The sensorpar a 4_ completestring.
            /// </summary>
            SENSORPARA4_COMPLETESTRING = 1231,

            /// <summary>
            /// The sensorpar a 5_ completestring.
            /// </summary>
            SENSORPARA5_COMPLETESTRING = 1232,

            /// <summary>
            /// The sensorpar a 6_ completestring.
            /// </summary>
            SENSORPARA6_COMPLETESTRING = 1233,

            /// <summary>
            /// The sensorpar a 7_ completestring.
            /// </summary>
            SENSORPARA7_COMPLETESTRING = 1234,

            /// <summary>
            /// The sensorpar a 8_ completestring.
            /// </summary>
            SENSORPARA8_COMPLETESTRING = 1235,

            /// <summary>
            /// The universalbuffe r 0.
            /// </summary>
            UNIVERSALBUFFER0 = 1300,

            /// <summary>
            /// The universalbuffe r 1.
            /// </summary>
            UNIVERSALBUFFER1 = 1301,

            /// <summary>
            /// The universalbuffe r 2.
            /// </summary>
            UNIVERSALBUFFER2 = 1302,

            /// <summary>
            /// The universalbuffe r 3.
            /// </summary>
            UNIVERSALBUFFER3 = 1303,

            /// <summary>
            /// The universalbuffe r 4.
            /// </summary>
            UNIVERSALBUFFER4 = 1304,

            /// <summary>
            /// The universalbuffe r 5.
            /// </summary>
            UNIVERSALBUFFER5 = 1305,

            /// <summary>
            /// The universalbuffe r 6.
            /// </summary>
            UNIVERSALBUFFER6 = 1306,

            /// <summary>
            /// The universalbuffe r 7.
            /// </summary>
            UNIVERSALBUFFER7 = 1307,

            // ReSharper restore UnusedMember.Global
            // ReSharper restore InconsistentNaming
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
        /// Gets the reminder count.
        /// </summary>
        /// <value>The reminder count.</value>
        public int ReminderCount
        {
            get
            {
                return 4;
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
                return 8;
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
                if (this.ProductId == ProductId.ProfiLuxPlusIIEx)
                {
                    return 3;
                }

                return 2;
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
                return 24;
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
                return 8;
            }
        }

        /// <summary>
        /// Gets the digital input count.
        /// </summary>
        /// <value>The digital input count.</value>
        public int DigitalInputCount
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets or sets the op mode.
        /// </summary>
        /// <value>The op mode.</value>
        public OperationMode OpMode
        {
            get
            {
                return (OperationMode)this.Controler.GetData((int)Code4.OPMODE); 
            }

            set
            {
                this.Controler.SendData((int)Code4.OPMODE, (int)value);
            }
        }

        /// <summary>
        /// Gets the light count.
        /// </summary>
        /// <value>The light count.</value>
        public int LightCount
        {
            get { return 8; }
        }

        /// <summary>
        /// Gets the timer count.
        /// </summary>
        /// <value>The timer count.</value>
        public int TimerCount
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the alarm.
        /// </summary>
        /// <value>The alarm.</value>
        public CurrentState Alarm
        {
            get
            {
                return (CurrentState)this.Controler.GetData((int)Code4.ISALARM);
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
                return (ProductId)this.Controler.GetData((int)Code4.PRODUCTID);
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
                return this.Controler.GetData((int)Code4.SERIALNUMBER);
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
                return BasicProtocol.ConvertToDate(this.Controler.GetData((int)Code4.SOFTWAREDATE));
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
                return this.Controler.GetData((int)Code4.ADDRESS);
            }
        }

        /// <summary>
        /// Gets the display line1.
        /// </summary>
        /// <value>The display line1.</value>
        protected override string DisplayLine1
        {
            get
            {
                return this.Controler.GetDataText((int)Code4.GETDISPLAYLINE1);
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
                return this.Controler.GetDataText((int)Code4.GETDISPLAYLINE2);
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
                return this.Controler.GetDataText((int)Code4.PROFILUXVIEW);
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
                return 0;
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
                return this.Controler.GetData((int)Code4.LOC_LATITUDE) * 0.1;
            }

            set
            {
                this.Controler.SendData((int)Code4.LOC_LATITUDE, (int)(value / 0.1));
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
                return this.Controler.GetData((int)Code4.LOC_LONGITUDE) * 0.1;
            }

            set
            {
                this.Controler.SendData((int)Code4.LOC_LONGITUDE, (int)(value / 0.1));
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
                return this.Controler.GetData((int)Code4.MOON_ACTPHASE);
            }
        }

        /// <summary>
        /// Gets the sensor active.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>If the sensor is active</returns>
        public bool GetSensorActive(int index)
        {
            return this.Controler.GetData((int)Code4.SENSORPARA1_ENABLED + (index * SensorBlockSize)) == 1;
        }

        /// <summary>
        /// Gets the module version.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Module Version</returns>
        public int GetModuleVersion(int module)
        {
            return Controler.GetData((int)Code4.MODUL0VERSION + module);
        }

        /// <summary>
        /// Gets the state of the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Module State</returns>
        public int GetModuleState(int module)
        {
            return Controler.GetData((int)Code4.MODUL0STATE + module);
        }

        /// <summary>
        /// Gets the module id.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Module Id</returns>
        public int GetModuleId(int module)
        {
            return Controler.GetData((int)Code4.MODUL0ID + module);
        }

        /// <summary>
        /// Gets the reminder text.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>Reminder Text</returns>
        public string GetReminderText(int reminder)
        {
            int offset = reminder * ReminderBlockSize;
            string text = Controler.GetDataText((int)Code4.MEM1_TEXT01 + offset);
            try
            {
                text += Controler.GetDataText((int)Code4.MEM1_TEXT23 + offset);
                text += Controler.GetDataText((int)Code4.MEM1_TEXT45 + offset);
                text += Controler.GetDataText((int)Code4.MEM1_TEXT67 + offset);
                text += Controler.GetDataText((int)Code4.MEM1_TEXT89 + offset);
                text += Controler.GetDataText((int)Code4.MEM1_TEXT1011 + offset);
                text += Controler.GetDataText((int)Code4.MEM1_TEXT1213 + offset);
                text += Controler.GetDataText((int)Code4.MEM1_TEXT1415 + offset);
            }
            catch (ErrorCodeException)
            {
            }

            return text;
        }

        /// <summary>
        /// Gets the days to next reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>the number of days to the next</returns>
        public DateTime? GetNextReminder(int reminder)
        {
            int offset = reminder * ReminderBlockSize;
            int days = Controler.GetData((int)Code4.MEM1_NEXTMEM + offset);
            if (days == 0xffff)
            {
                return null;
            }

            DateTime nextReminder = new DateTime(2000, 1, 1);
            nextReminder = nextReminder.AddDays(days);

            return nextReminder;
        }

        /// <summary>
        /// Gets the type of the sensor.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Sensor Type</returns>
        public SensorType GetSensorType(int sensorIndex)
        {
            return (SensorType)Controler.GetData((int)Code4.SENSORPARA1_SENSORTYPE + (sensorIndex * SensorBlockSize));
        }

        /// <summary>
        /// Gets the sensor format.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Sensor Format</returns>
        public int GetSensorFormat(int sensorIndex)
        {
            return Controler.GetData((int)Code4.SENSORPARA1_DISPLAYMODE + (sensorIndex * SensorBlockSize));
        }

        /// <summary>
        /// Gets the sensor nominal value.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Nominal Value</returns>
        public int GetSensorNominalValue(int sensorIndex)
        {
            return Controler.GetData((int)Code4.SENSORPARA1_DESVALUE + (sensorIndex * SensorBlockSize)); 
        }

        /// <summary>
        /// Gets the sensor alarm deviation.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Alarm Deviation</returns>
        public int GetSensorAlarmDeviation(int sensorIndex)
        {
            return Controler.GetData((int)Code4.SENSORPARA1_ALARMDEVIATION + (sensorIndex * SensorBlockSize)); 
        }

        /// <summary>
        /// Gets the sensor alarm enable.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>If the Alarm is Enable</returns>
        public bool GetSensorAlarmEnable(int sensorIndex)
        {
            return Controler.GetData((int)Code4.SENSORPARA1_ALARMMODE + (sensorIndex * SensorBlockSize)) != 0; 
        }

        /// <summary>
        /// Gets the sensor value.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Get Sensor Value</returns>
        public int GetSensorValue(int sensorIndex)
        {
            return Controler.GetData((int)Code4.SENSORPARA1_ACTVALUE + (sensorIndex * 4));
        }

        /// <summary>
        /// Gets the S port function.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>the function</returns>
        public PortMode GetSPortFunction(int portIndex)
        {
            int mode = portIndex < 16 ? this.Controler.GetData((int)Code4.SWITCHPLUG1_FUNCTION + portIndex) : this.Controler.GetData((int)Code4.SWITCHPLUG17_FUNCTION + (portIndex - 16));
            
            // mode format
            // 1234 1234 1234 1234
            // IBBB BBBB PPPT TTTT
            // I = Invert
            // B = Blackout time
            // P = Port Number
            // T = Type
            PortMode portMode = new PortMode
                                      {
                                          DeviceMode = (DeviceMode)(mode & 0x001F),
                                          Port = ((mode & 0x00E0) >> 5) + 1,
                                          BlackOut = (mode & 0x7F00) >> 8,
                                          Invert = (mode & 0x8000) != 0
                                      };

            return portMode;
        }

        /// <summary>
        /// Gets the S port value.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>the value of the port</returns>
        public int GetSPortValue(int portIndex)
        {
            if (portIndex < 16)
            {
                return Controler.GetData((int) Code4.SP1_STATE + portIndex);
            }

            return this.Controler.GetData((int)Code4.SP17_STATE + (portIndex - 16));
        }

        /// <summary>
        /// Gets the L port value.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>LPort Value</returns>
        public int GetLPortValue(int portIndex)
        {
            return Controler.GetData((int)Code4.L1TO10VINT1_PWMVALUE + portIndex);
        }

        /// <summary>
        /// Gets the L port function.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>LPort Funtion</returns>
        public PortMode GetLPortFunction(int portIndex)
        {
            int mode = Controler.GetData((int)Code4.L1TO10VINT1_FUNCTION + (portIndex * 3));

            // mode format
            // 1234 1234
            // PPPT TTTT
            // I = Invert
            // B = Blackout time
            // P = Port Number
            // T = Type
            PortMode portMode = new PortMode
                {
                    DeviceMode = LPortModeToSocketType(mode & 0x001F),
                    Port = ((mode & 0x00E0) >> 5) + 1,
                    BlackOut = 0,
                    Invert = false
                };

            return portMode;
        }

        /// <summary>
        /// Digitals the state of the input.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The current Input state</returns>
        public CurrentState GetDigitalInputState(int index)
        {
            return CurrentState.Off;
        }

        /// <summary>
        /// Starts a Thunderstorm.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public void Thunderstorm(int duration)
        {
            this.Controler.SendData((int)Code4.THUNDERSTORM_MANUSTART, duration);
        }

        /// <summary>
        /// Sets the Feed Pause
        /// </summary>
        /// <param name="activate">activates the feed pasue</param>
        public void FeedPause(bool activate)
        {
        }

        /// <summary>
        /// Sets the Maintenace Mode
        /// </summary>
        /// <param name="activate">activates the Maintaincne mode</param>
        public void Maintenace(bool activate)
        {
        }

        /// <summary>
        /// Perfoms a Water Change
        /// </summary>
        /// <param name="index">The index.</param>
        public void WaterChange(int index)
        {
        }

        /// <summary>
        /// Gets the state of the level sensor.
        /// </summary>
        /// <param name="levelSensorIndex">Index of the level sensor.</param>
        /// <returns>Sensor state</returns>
        public LevelState GetLevelSensorState(int levelSensorIndex)
        {
            int state;
            switch (levelSensorIndex)
            {
                case 0:
                    state = this.Controler.GetData((int)Code4.LEVEL0STATE);
                    break;
                case 1:
                    state = this.Controler.GetData((int)Code4.LEVEL1STATE);
                    break;
                default:
                    state = this.Controler.GetData((int)Code4.LEVEL2STATE);
                    break;
            }

            LevelState levelState = new LevelState
                {
                    State = state == 0 ? CurrentState.Off : CurrentState.On, 
                    Alarm = CurrentState.Off
                };

            return levelState;
        }

        /// <summary>
        /// Gets the level sensor mode.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>The Opertational Mode</returns>
        public LevelSensorOpertationMode GetLevelSensorMode(int sensorIndex)
        {
            return LevelSensorOpertationMode.NotEnabled;
        }

        /// <summary>
        /// Gets the digital input function.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>the funtion of the digital input</returns>
        public DigitalInputFunction GetDigitalInputFunction(int sensorIndex)
        {
            return DigitalInputFunction.NotUsed;
        }

        /// <summary>
        /// Gets the sensor alarm.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>true if the sensor is in alarm state</returns>
        public CurrentState GetSensorAlarm(int sensorIndex)
        {
            return CurrentState.Off;
        }

        /// <summary>
        /// Gets the reminder period.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>the period</returns>
        public int GetReminderPeriod(int reminder)
        {
            return 0;
        }

        /// <summary>
        /// Gets the reminder repeats.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>if the reminder repeats</returns>
        public bool GetReminderRepeats(int reminder)
        {
            return false;
        }

        /// <summary>
        /// Resets the reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <param name="period">The period.</param>
        public void ResetReminder(int reminder, int period)
        {
        }

        /// <summary>
        /// Clears the reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        public void ClearReminder(int reminder)
        {
        }

        /// <summary>
        /// Determines whether [is light active] [the specified channel].
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        ///     <c>true</c> if [is light active] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLightActive(int channel)
        {
            return false;
        }

        /// <summary>
        /// Determines whether [is light dimmable] [the specified channel].
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        ///     <c>true</c> if [is light dimmable] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLightDimmable(int channel)
        {
            return false;
        }

        /// <summary>
        /// Gets the light value.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>the current value of the light channel</returns>
        public double GetLightValue(int channel)
        {
            return this.Controler.GetData((int)Code4.ILLUMINATION1_ACTPERCENT + (channel * 2));
        }

        /// <summary>
        /// Sets the state of the light.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The value.</param>
        public void SetLightValue(int channel, double value)
        {
            this.Controler.SendData((int)Code4.ILLUMINATION1_ACTPERCENT + (channel * 2), (int)value);
        }

        /// <summary>
        /// Gets the dosing rate.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>the dosing rate</returns>
        public int GetDosingRate(int channel)
        {
            return 0;
        }

        /// <summary>
        /// Gets the timer mode.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns> the timer setteings</returns>
        public TimerSettings GetTimerSettings(int channel)
        {
            return new TimerSettings();
        }

        /// <summary>
        /// Updates the timer settings.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="settings">The settings.</param>
        public void UpdateTimerSettings(int channel, TimerSettings settings)
        {
        }

        /// <summary>
        /// Updates the dosing rate.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="rate">The rate.</param>
        public void UpdateDosingRate(int channel, int rate)
        {
        }

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        /// <param name="index">The index.</param>
        public void ClearLevelAlarm(int index)
        {
        }

        /// <summary>
        /// Gets the sensor mode.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>the mode of the sensor</returns>
        public SensorMode GetSensorMode(int sensorIndex)
        {
            return
                (SensorMode)
                (this.Controler.GetData((int)Code4.SENSORPARA1_CAL1ADC + (sensorIndex * SensorBlockSize)) >> 12);
        }

        /// <summary>
        /// Sets the state of the socket.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetSocketState(int portIndex, bool value)
        {
            int index = portIndex % 24;
            int allState = this.Controler.GetData((int)Code4.SP_ALL_STATE);
            if (value)
            {
                allState |= 1 << index;
            }
            else
            {
                allState &= ~(1 << index);
            }

            this.Controler.SendData((int)Code4.SP_ALL_STATE, allState);
        }

        /// <summary>
        /// Gets the probe operation hours.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The nuber of hours the probe is in opeeration
        /// </returns>
        public int GetProbeOperationHours(int index)
        {
            return this.Controler.GetData((int)Code4.SENSORPARA1_OHM + (index * 4));
        }

        /// <summary>
        /// Sets the probe operation hours.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void SetProbeOperationHours(int index, int value)
        {
            this.Controler.SendData((int)Code4.SENSORPARA1_OHM + (index * 4), value);
        }

        /// <summary>
        /// Gets the light operation hours.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        /// &gt;The nuber of hours the light is in opeeration
        /// </returns>
        public int GetLightOperationHours(int channel)
        {
            return this.Controler.GetData((int)Code4.ILLUMINATION1_OHM + (channel * 2));
        }

        /// <summary>
        /// Sets the light operation hours.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The value.</param>
        public void SetLightOperationHours(int channel, int value)
        {
            this.Controler.SendData((int)Code4.ILLUMINATION1_OHM + (channel * 2), value);
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="probes">The probes.</param>
        /// <returns>a list of data points for the sensor</returns>
        public override Collection<ItemDataRow> GetDataPoints(IProgressCallback callback, ReadOnlyCollection<Probe> probes)
        {
            if (!this.Controler.IsConnected)
            {
                throw new ProtocolException(500, "Unable to get data points Not Connected!");
            }

            Collection<ItemDataRow> data = new Collection<ItemDataRow>();

            int sourceMask = this.Controler.GetData((int)Code4.MEASUREMENT_SAMPLESOURCEMASK);
            int columns = 0;
            int sensorIndex = 0;

            Collection<Probe> sensorColumns = new Collection<Probe>();

            while (sourceMask != 0)
            {
                if ((sourceMask & 0x01) == 1)
                {
                    columns += sourceMask & 0x01;
                    int index = sensorIndex;
                    Probe probe = probes.FirstOrDefault(p => p.Index == index) ?? new Probe { Id = "Null" };

                    sensorColumns.Add(probe);

                    sensorIndex++;
                }

                sourceMask >>= 1;
            }

            if (columns == 0)
            {
                return data;
            }

            int sampleTime = this.Controler.GetData((int)Code4.MEASUREMENT_LASTSAMPLETIME);
            int hour = sampleTime / 60;
            int minute = sampleTime % 60;

            DateTime startTime = new DateTime(2000, 1, 1);
            DateTime lastDataPointDate = startTime.AddDays(this.Controler.GetData((int)Code4.MEASUREMENT_LASTSAMPLEDATE));
            DateTime lastDataPointTime = new DateTime(lastDataPointDate.Year, lastDataPointDate.Month, lastDataPointDate.Day, hour, minute, 0);
            int newDataPoints = this.Controler.GetData((int)Code4.MEASUREMENT_NEWDATACOUNT);
            int totalDataPoints = newDataPoints / columns;

            int samplePeriod = this.Controler.GetData((int)Code4.MEASUREMENT_SAMPLEPERIOD);
            DateTime firstDataPointTime = lastDataPointTime.AddMinutes(-(totalDataPoints * samplePeriod));
            DateTime datapointTime = firstDataPointTime;

            callback.Begin(0, totalDataPoints, Language.GetResource("strImport"));
            callback.SetText("Reading data points from Profilux");
            for (int i = 0; i < totalDataPoints; i++)
            {
                if (callback.IsAborting)
                {
                    break;
                }

                callback.StepTo(i);

                Collection<ItemDataRow.Item> values = new Collection<ItemDataRow.Item>();

                short[] dataPoints = this.Controler.GetDataShortArray((int)Code4.MEASUREMENT_GETDATARECORD);

                int columnIndex = 0;
                foreach (short value in dataPoints)
                {
                    if (sensorColumns[columnIndex].Id != "Null")
                    {
                        values.Add(
                            new ItemDataRow.Item(sensorColumns[columnIndex].ConvertFromInt(value), sensorColumns[columnIndex].Id));
                    }

                    columnIndex++;
                }

                this.Controler.SendData((int)Code4.MEASUREMENT_INCREADDATAPOINTER, 0);

                data.Add(new ItemDataRow(datapointTime, values));
                datapointTime = datapointTime.AddMinutes(samplePeriod);
            }

            return data;
        }

        /// <summary>
        /// Keys the command.
        /// </summary>
        /// <param name="faceplateKey">The faceplate key.</param>
        public void KeyCommand(FaceplateKey faceplateKey)
        {
            this.Controler.SendData((int)Code4.SENDKEY, (int)faceplateKey);
        }

        #region IProfilux Members


        public System.Drawing.Image DisplayImage
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IProfilux Members


        /// <summary>
        /// Gets the current pump count.
        /// </summary>
        /// <value>The current pump count.</value>
        public int CurrentPumpCount
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets the state of the current pump.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the state of the pump</returns>
        public int GetCurrentPumpValue(int index)
        {
            return this.Controler.GetData((int)Code4.CURRENTPUMP1_ACTPERCENT + index);
        }

        #endregion

        #region IProfilux Members


        public bool IsCurrentPumpAssinged(int i)
        {
            int totalPumps = this.Controler.GetData((int)Code4.CURRENTCONTROL_GROUP2PUMPCOUNT) + this.Controler.GetData((int)Code4.CURRENTCONTROL_GROUP1PUMPCOUNT);
            return totalPumps > i;
        }

        #endregion

        #region IProfilux Members


        public double GetSPortCurrent(int portIndex)
        {
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
            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the S port.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public string GetSPortName(int i)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the probe.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public string GetProbeName(int i)
        {
            return string.Empty;
        }
    }
}

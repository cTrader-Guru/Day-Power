/*  CTRADER GURU --> Template 1.0.6

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/cTraderGURU/
    TOS         : https://ctrader.guru/termini-del-servizio/

*/

using System;
using cAlgo.API;
using System.Collections.Generic;

namespace cAlgo
{

    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class DayPower : Indicator
    {

        #region Enums & Class

        /// <summary>
        /// Mi permette di scegliere tra il rettangolo o le linee
        /// </summary>
        public enum _DrawMode
        {

            Rectangle,
            Line

        }

        /// <summary>
        /// La posizione di calcolo di Fibonacci
        /// </summary>
        public enum _FiboMode
        {

            Linear,
            Reverse,
            Disabled

        }

        /// <summary>
        /// Una rappresentazione di una barra daily su un timeframe inferiore
        /// </summary>
        public class DailyBar
        {

            /// <summary>
            /// Il prezzo d'apertura della candela
            /// </summary>
            public double Open { get; private set; }

            /// <summary>
            /// L'orario di apertura della candela
            /// </summary>
            public DateTime OpenTime { get; private set; }

            /// <summary>
            /// Il prezzo di chiusura della candela
            /// </summary>
            public double Close { get; private set; }

            /// <summary>
            /// Il prezzo massimo della candela
            /// </summary>
            public double High { get; private set; }

            /// <summary>
            /// Il prezzo minimo della candela
            /// </summary>
            public double Low { get; private set; }

            /// <summary>
            /// Costruttore della classe
            /// </summary>
            /// <param name="pOpen">Il prezzo di apertura della giornata in esame</param>
            /// <param name="pOpenTime">L'orario di apertura della candela sotto esame</param>
            /// <param name="pClose">Il prezzo di chiusura della giornata in esame</param>
            /// <param name="pHigh">Il prezzo massimo della giornata in esame</param>
            /// <param name="pLow">Il prezzo minimo della giornata in esame</param>
            public DailyBar(double pOpen, DateTime pOpenTime, double pClose, double pHigh, double pLow)
            {

                Open = pOpen;
                OpenTime = pOpenTime;
                Close = pClose;
                High = pHigh;
                Low = pLow;

            }

            /// <summary>
            /// Calcola il body della candela
            /// </summary>
            /// <returns>La differenza tra chiusura e apertura, -1 in caso di dati non inizializzati, -2 dati errati</returns>
            public double Body()
            {

                if (Open == 0 || Close == 0)
                    return -1;

                return (Open > Close) ? Open - Close : (Close > Open) ? Close - Open : -2;

            }

            /// <summary>
            /// Calcola la shadow della candela
            /// </summary>
            /// <returns>La differenza tra massimo e minimo, -1 in caso di dati non inizializzati, -2 dati errati</returns>
            public double Shadow()
            {

                if (High == 0 || Low == 0)
                    return -1;

                return (High > Low) ? High - Low : -2;

            }

        }

        #endregion

        #region Identity
        
        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Day Power";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.1";

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/day-power/")]
        public string ProductInfo { get; set; }

        /// <summary>
        /// Il numero di candele daily da controllare
        /// </summary>
        [Parameter("Period", Group = "Params", DefaultValue = 7, MinValue = 1, Step = 1)]
        public int BoxPeriod { get; set; }

        /// <summary>
        /// L'ora in cui apre la candela daily
        /// </summary>
        [Parameter("Daily Start Hour", Group = "Params", DefaultValue = 0, MaxValue = 23, MinValue = 0, Step = 1)]
        public int DayStartHour { get; set; }

        /// <summary>
        /// Il minuto in cui apre la candela daily
        /// </summary>
        [Parameter("Daily Start Minute", Group = "Params", DefaultValue = 0, MaxValue = 59, MinValue = 0, Step = 1)]
        public int DayStartMinute { get; set; }

        /// <summary>
        /// Opzione che mi permette di visualizzare solo l'ultimo range
        /// </summary>
        [Parameter("Show Only Last Power ?", Group = "Params", DefaultValue = true)]
        public bool OnlyLast { get; set; }

        /// <summary>
        /// La posizione di Fibonacci
        /// </summary>
        [Parameter("Fibo Mode", Group = "Params", DefaultValue = _FiboMode.Linear)]
        public _FiboMode FiboMode { get; set; }

        /// <summary>
        /// Fibo editabile
        /// </summary>
        [Parameter("Fibo Editable ?", Group = "Params", DefaultValue = false)]
        public bool FiboEditable { get; set; }

        /// <summary>
        /// Fibo il prezzo
        /// </summary>
        [Parameter("Fibo Price ?", Group = "Params", DefaultValue = false)]
        public bool FiboPrice { get; set; }

        /// <summary>
        /// Il Box, lo stile del bordo
        /// </summary>
        [Parameter("Draw Mode", Group = "Params", DefaultValue = _DrawMode.Rectangle)]
        public _DrawMode BoxDrawMode { get; set; }

        /// <summary>
        /// Il Box, lo stile del bordo
        /// </summary>
        [Parameter("Line Style Box", Group = "Styles", DefaultValue = LineStyle.DotsRare)]
        public LineStyle LineStyleBox { get; set; }

        /// <summary>
        /// Il Box, lo spessore del bordo
        /// </summary>
        [Parameter("Tickness", Group = "Styles", DefaultValue = 1, MaxValue = 5, MinValue = 1, Step = 1)]
        public int TicknessBox { get; set; }

        /// <summary>
        /// Il Box, il colore rialzista
        /// </summary>
        [Parameter("Long Color", Group = "Styles", DefaultValue = "DodgerBlue")]
        public string ColorLong { get; set; }

        /// <summary>
        /// Il Box, il colore neutrale
        /// </summary>
        [Parameter("Short Color", Group = "Styles", DefaultValue = "Gray")]
        public string ColorNeutral { get; set; }

        /// <summary>
        /// Il Box, il colore ribassista
        /// </summary>
        [Parameter("Short Color", Group = "Styles", DefaultValue = "Red")]
        public string ColorShort { get; set; }

        /// <summary>
        /// Il Box, l'opacità
        /// </summary>
        [Parameter("Opacity", Group = "Styles", DefaultValue = 30, MinValue = 1, MaxValue = 100, Step = 1)]
        public int Opacity { get; set; }

        /// <summary>
        /// Il Box, il riempimento
        /// </summary>
        [Parameter("Fill Range ?", Group = "Styles", DefaultValue = true)]
        public bool FillBox { get; set; }

        #endregion

        #region Property

        List<DailyBar> DailyBars = new List<DailyBar>();
        Bar FirstCandleOfTheDay;
        Bar LastCandleOfTheDay;

        double DayHighestPrice = 0;
        double DayLowestPrice = 0;

        #endregion

        #region Indicator Events

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Se il timeframe è superiore o uguale al giornaliero devo uscire
            if (TimeFrame >= TimeFrame.Daily)
                Print("{0} : USE THIS INDICATOR ON TIMEFRAME LOWER 1DAY", NAME);

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            // --> L'utente potrebbe aver inserito un colore errato
            if (Color.FromName(ColorLong).ToArgb() == 0)
                ColorLong = "DodgerBlue";
            if (Color.FromName(ColorShort).ToArgb() == 0)
                ColorShort = "Red";

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {


            // --> Non esiste ancora un metodo per rimuovere l'indicatore dal grafico, quindi ci limitiamo a uscire
            // --> Risparmio risorse controllando solo quando mi trovo sull'ultima candela, quella corrente
            // --> Devo avere in memoria abbastanza candele daily
            if (TimeFrame >= TimeFrame.Daily)
                return;

            // --> Deve essere inizializzata
            if (FirstCandleOfTheDay == null)
            {

                FirstCandleOfTheDay = Bars[index];
                LastCandleOfTheDay = Bars[index];

            }

            // --> Poichè l'indice non corrisponde a quello giornaliero, devo ricreare le aperture e le chiusure
            DateTime now = Bars.OpenTimes[index];

            if (Bars[index].High > DayHighestPrice || DayHighestPrice == 0)
                DayHighestPrice = Bars[index].High;
            if (Bars[index].Low < DayLowestPrice || DayLowestPrice == 0)
                DayLowestPrice = Bars[index].Low;

            // --> Ricreo il cambio candela daily
            // --> Ad ogni cambio candela corrente devo aggiornare i dati
            if (FirstCandleOfTheDay.OpenTime != now)
            {

                // --> Se è la prima candela del giorno sarà anche l'inizio della daily
                if (now.Hour == DayStartHour && now.Minute == DayStartMinute)
                {

                    // --> Siamo in un nuovo giorno, registro la candela appena chiusa
                    DailyBars.Add(new DailyBar(FirstCandleOfTheDay.Open, FirstCandleOfTheDay.OpenTime, LastCandleOfTheDay.Close, DayHighestPrice, DayLowestPrice));

                    FirstCandleOfTheDay = Bars[index];

                    // --> Resettiamo le memorie
                    DayHighestPrice = 0;
                    DayLowestPrice = 0;

                }
                else
                {

                    // --> Registro la candela precedente, sarà l'ultima del giorno
                    LastCandleOfTheDay = Bars[index];

                }

            }
            else
            {

                // --> Inutile proseguire, la traccia daily è già stata disegnata
                return;

            }

            // --> Se non ho abbastanza candele devo uscire
            if (DailyBars.Count < BoxPeriod)
                return;

            double HighestBody = 0;
            double HighestBodyShadow = 0;

            // --> Indice Giornaliero
            int DailyIndex = DailyBars.Count - 1;

            // --> Partendo dal periodo devo controllare la distanza minima e massima dei body delle candele
            for (int i = 0; i < BoxPeriod; i++)
            {

                int CurrentIndex = DailyIndex - i;

                double CurrentBody = DailyBars[CurrentIndex].Body();
                double CurrentBodyShadow = DailyBars[CurrentIndex].Shadow();

                if (CurrentBody > HighestBody)
                    HighestBody = CurrentBody;

                if (CurrentBodyShadow > HighestBodyShadow)
                    HighestBodyShadow = CurrentBodyShadow;

            }

            // --> Ricavo l'inizio e la fine temporale del box, verrà preso in considerazione solo per timeframe inferiori
            DateTime today = FirstCandleOfTheDay.OpenTime;

            // --> Facendo attenzione al Venerdì
            DateTime tomorrow = (today.DayOfWeek != DayOfWeek.Friday) ? today.AddDays(1) : today.AddDays(3);

            string rangeFlag = (OnlyLast) ? "" : today.ToString();

            // --> Disegnamo il riferimento a seconda la scelta fatta
            switch (BoxDrawMode)
            {

                case _DrawMode.Rectangle:

                    ChartRectangle LongRectangle = Chart.DrawRectangle("Long" + rangeFlag, today, FirstCandleOfTheDay.Open, tomorrow, FirstCandleOfTheDay.Open + HighestBody, Color.FromArgb(Opacity, Color.FromName(ColorLong)), TicknessBox, LineStyleBox);
                    ChartRectangle ShortRectangle = Chart.DrawRectangle("Short" + rangeFlag, today, FirstCandleOfTheDay.Open, tomorrow, FirstCandleOfTheDay.Open - HighestBody, Color.FromArgb(Opacity, Color.FromName(ColorShort)), TicknessBox, LineStyleBox);

                    LongRectangle.IsFilled = FillBox;
                    ShortRectangle.IsFilled = FillBox;

                    break;

                case _DrawMode.Line:

                    Chart.DrawTrendLine("Long" + rangeFlag, today, FirstCandleOfTheDay.Open + HighestBody, tomorrow, FirstCandleOfTheDay.Open + HighestBody, Color.FromName(ColorLong), TicknessBox, LineStyleBox);
                    Chart.DrawTrendLine("Neutral" + rangeFlag, today, FirstCandleOfTheDay.Open, tomorrow, FirstCandleOfTheDay.Open, Color.FromName(ColorNeutral), TicknessBox, LineStyleBox);
                    Chart.DrawTrendLine("Short" + rangeFlag, today, FirstCandleOfTheDay.Open - HighestBody, tomorrow, FirstCandleOfTheDay.Open - HighestBody, Color.FromName(ColorShort), TicknessBox, LineStyleBox);

                    break;

            }

            ChartFibonacciRetracement FiboUp;
            ChartFibonacciRetracement FiboDown;

            // --> Fibonacci come proporzione
            switch (FiboMode)
            {

                case _FiboMode.Linear:

                    FiboUp = Chart.DrawFibonacciRetracement("FiboUp", tomorrow, FirstCandleOfTheDay.Open + HighestBody, tomorrow, FirstCandleOfTheDay.Open, Color.FromName(ColorLong), 1, LineStyle.DotsRare);
                    FiboUp.IsInteractive = FiboEditable;
                    FiboUp.DisplayPrices = FiboPrice;

                    FiboDown = Chart.DrawFibonacciRetracement("FiboDown", tomorrow, FirstCandleOfTheDay.Open - HighestBody, tomorrow, FirstCandleOfTheDay.Open, Color.FromName(ColorShort), 1, LineStyle.DotsRare);
                    FiboDown.IsInteractive = FiboEditable;
                    FiboDown.DisplayPrices = FiboPrice;

                    if (!FiboEditable)
                    {

                        _standardFibo(ref FiboUp);
                        _standardFibo(ref FiboDown);

                    }

                    break;

                case _FiboMode.Reverse:

                    FiboUp = Chart.DrawFibonacciRetracement("FiboUp", tomorrow, FirstCandleOfTheDay.Open, tomorrow, FirstCandleOfTheDay.Open + HighestBody, Color.FromName(ColorLong), 1, LineStyle.DotsRare);
                    FiboUp.IsInteractive = FiboEditable;
                    FiboUp.DisplayPrices = FiboPrice;

                    FiboDown = Chart.DrawFibonacciRetracement("FiboDown", tomorrow, FirstCandleOfTheDay.Open, tomorrow, FirstCandleOfTheDay.Open - HighestBody, Color.FromName(ColorShort), 1, LineStyle.DotsRare);
                    FiboDown.IsInteractive = FiboEditable;
                    FiboDown.DisplayPrices = FiboPrice;

                    if (!FiboEditable)
                    {

                        _standardFibo(ref FiboUp);
                        _standardFibo(ref FiboDown);

                    }

                    break;

                case _FiboMode.Disabled:


                    Chart.RemoveObject("FiboUp");
                    Chart.RemoveObject("FiboDown");

                    break;

            }

            // --> Disegnamo il riferimento delle shadow
            Chart.DrawTrendLine("High" + rangeFlag, today, FirstCandleOfTheDay.Open + HighestBodyShadow, tomorrow, FirstCandleOfTheDay.Open + HighestBodyShadow, Color.FromName(ColorLong), TicknessBox, LineStyleBox);
            Chart.DrawTrendLine("Low" + rangeFlag, today, FirstCandleOfTheDay.Open - HighestBodyShadow, tomorrow, FirstCandleOfTheDay.Open - HighestBodyShadow, Color.FromName(ColorShort), TicknessBox, LineStyleBox);


        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Standardizza i livelli di Fibonacci
        /// </summary>
        /// <param name="MyFibo">L'oggetto da modificare</param>
        private void _standardFibo(ref ChartFibonacciRetracement MyFibo)
        {

            decimal[] DefaultFiboLevels = new[] 
            {
                0.0m,
                23.6m,
                38.2m,
                50.0m,
                61.8m,
                76.4m,
                100.0m
            };

            for (int i = 0; i < MyFibo.FibonacciLevels.Count; i++)
            {

                MyFibo.FibonacciLevels[i].IsVisible = Array.IndexOf(DefaultFiboLevels, (decimal)MyFibo.FibonacciLevels[i].PercentLevel) > -1;

            }

        }

        /// <summary>
        /// In caso di necessità viene utilizzata per stampare dati sul grafico
        /// </summary>
        /// <param name="mex">Il messaggio da visualizzare</param>
        /// <param name="doPrint">Flag se si vuole stampare nei log</param>
        private void _debug(string mex = "...", bool doPrint = true)
        {

            Chart.DrawStaticText(NAME + "Debug", string.Format("{0} : {1}", NAME, mex), VerticalAlignment.Bottom, HorizontalAlignment.Right, Color.Red);
            if (doPrint)
                Print(mex);

        }

        #endregion

    }

}

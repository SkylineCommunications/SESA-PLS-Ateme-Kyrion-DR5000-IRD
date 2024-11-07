namespace SESSA_SRM_PLS_Ateme_Kyrion_DR5000.IRD_1.SES
{
    /// <summary>
    /// Class cocntaining parameter IDs from Ateme Kyrion DR5000 protocol.
    /// </summary>
    internal static class ParameterDR5000
    {
        // TODO: missing Roll-off, FEC
        #region Input Type

        public const int InputTypeRead = 3001;

        public const int InputTypeWrite = 3101;

        #endregion

        #region Service ID

        public const int CompositionPrimaryServiceIDRead = 8002;

        public const int CompositionPrimaryServiceIDWrite = 8102;

        #endregion

        #region Service Name

        public const int CompositionPrimaryServiceRead = 8004;

        public const int CompositionPrimaryServiceWrite = 8104;

        #endregion

        #region DVB-S/S2 Mode Config

        public const int DvbModeConfigRead = 4002;

        public const int DvbModeConfigWrite = 4102;

        #endregion

        #region Satellite Frequency

        public const int RfFrequencyConfigRead = 4003;

        public const int RfFrequencyConfigWrite = 4103;

        #endregion

        #region LNB LO Frequency

        public const int LnbLoFrequencyRead = 4004;

        public const int LnbLoFrequencyWrite = 4104;

        #endregion

        #region Symbol Rate

        public const int SymbolRateRead = 4005;

        public const int SymbolRateWrite = 4105;

        #endregion

        #region BISS Mode

        public const int BissModeRead = 13004;

        public const int BissModeWrite = 1324;

        #endregion

        #region BISS 1 Key

        public const int Biss1KeyRead = 13045;

        public const int Biss1KeyWrite = 13065;

        #endregion

        #region BISS E Key

        public const int BissEKeyRead = 13046;

        public const int BissEKeyWrite = 13066;

        #endregion
    }
}

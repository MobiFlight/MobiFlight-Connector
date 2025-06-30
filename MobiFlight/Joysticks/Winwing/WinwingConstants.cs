namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingConstants
    {
        internal const int PRODUCT_ID_FCU_ONLY = 0xBB10;
        internal const int PRODUCT_ID_FCU_EFISL = 0xBC1D;
        internal const int PRODUCT_ID_FCU_EFISR = 0xBC1E;
        internal const int PRODUCT_ID_FCU_EFISL_EFISR = 0xBA01;

        internal const int PRODUCT_ID_PAP3_ONLY = 0xBF0F;

        internal const int PRODUCT_ID_MCDU_CPT = 0xBB36;
        internal const int PRODUCT_ID_MCDU_OBS = 0xBB3A;
        internal const int PRODUCT_ID_MCDU_FO = 0xBB3E;

        internal const int PRODUCT_ID_PFP3N_CPT = 0xBB35;
        internal const int PRODUCT_ID_PFP3N_OBS = 0xBB39;
        internal const int PRODUCT_ID_PFP3N_FO = 0xBB3D;

        internal const int PRODUCT_ID_PFP7_CPT = 0xBB37;
        internal const int PRODUCT_ID_PFP7_OBS = 0xBB3B;
        internal const int PRODUCT_ID_PFP7_FO  = 0xBB3F;

        internal static readonly int[] FCU_PRODUCTIDS = { PRODUCT_ID_FCU_ONLY, PRODUCT_ID_FCU_EFISL, PRODUCT_ID_FCU_EFISR, PRODUCT_ID_FCU_EFISL_EFISR };
        internal static readonly int[] PAP3_PRODUCTIDS = { PRODUCT_ID_PAP3_ONLY };
        internal static readonly int[] CDU_PRODUCTIDS = { PRODUCT_ID_MCDU_CPT, PRODUCT_ID_MCDU_OBS, PRODUCT_ID_MCDU_FO,
                                                          PRODUCT_ID_PFP3N_CPT, PRODUCT_ID_PFP3N_OBS, PRODUCT_ID_PFP3N_FO,
                                                          PRODUCT_ID_PFP7_CPT, PRODUCT_ID_PFP7_OBS, PRODUCT_ID_PFP7_FO };

        internal const string CDU_DATA = "Cdu Data";
        internal const string FONT_DATA = "Font Data";
    }
}

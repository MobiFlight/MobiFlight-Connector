namespace MobiFlight.xplane.WebApi
{
    internal static class XplaneWebApiConstants
    {
        public const string DatarefsPath = "/api/v3/datarefs";
        public const string CommandsPath = "/api/v3/commands";
        public const string WebSocketPath = "/api/v3";

        public const string MessageTypeDatarefSubscribeValues = "dataref_subscribe_values";
        public const string MessageTypeDatarefUnsubscribeValues = "dataref_unsubscribe_values";
        public const string MessageTypeDatarefUpdateValues = "dataref_update_values";
        public const string MessageTypeDatarefSetValues = "dataref_set_values";
        public const string MessageTypeCommandSetIsActive = "command_set_is_active";
        public const string MessageTypeResult = "result";

        public const string HeartbeatTimeDataRef = "sim/time/total_running_time_sec";
        public const string HeartbeatAircraftNameDataRef = "sim/aircraft/view/acf_ui_name";
    }
}

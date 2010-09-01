
/// <summary>
/// List of Pobicos instructions
/// </summary>
public enum InstructionsList
{
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    EnableEvent,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    CreateGenericAgent,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    CreateNonGenericAgents,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    Release,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetChildInfo,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SendCommand,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetCommand,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SendReport,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetReport,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SetTimer,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetTimerId,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    CreateReportList,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    DestroyReportList,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AddReport,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetNextReport,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    DisableEvent,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetMyID,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetConfigSetting,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    DbgString,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    DbgUInt32,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
	GetTemp = 290455552,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    GetBrightness,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    Alert,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertAurally,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertByVoice,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertByVoiceInPolish,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertBySound,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertBySirenSound,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertVisually,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    AlertByDisplayingText,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ConveyMessage,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
	ConveyMessageByText,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ConveyMessageByVoice,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SetTempRange,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SetTemp,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SetBrightness,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    Close,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SwitchOn,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SwitchOff,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TurnDownBrightness,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TurnDownTemp,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TurnUpTemp,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TurnUpBrightness
}
/// <summary>
/// List of Pobicos events
/// </summary>
public enum EventsList
{
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ponge_originated_event_switch_originated_event,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
	SmokeEvent,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    Init,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    Finalize,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ChildCreated,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ChildCreationTimeout,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ChildUnreachable,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    CommandArrived,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ReportArrived,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    Timeout,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ConfigSettingsChanged,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ThresholdExceeded,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TemperatureRangeExceeded,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TemperatureTooHigh,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    TemperatureTooLow,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    BightnessRangeExceeded,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    BightnessTooHigh,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    BightnessTooLow,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    HumidityRangeExceeded,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    HumidityTooHigh,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    HumidityTooLow,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ObjectClosed,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    DangerousElementDetected,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    SmokeDetected,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    CabonOxideDetected,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    FireDetected,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    FireAlarmActivated,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ObjectSwitched,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ObjectSwitchedOn,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ObjectSwitchedOff,
    /// <summary>
    /// POBICOS taxonomy element
    /// </summary>
    ObjectOpened,
    

}
/// <summary>
/// Client types
/// </summary>
public enum clientType
{
    /// <summary>
    /// POBICOS object simulator enum
    /// </summary>
    OBJECT = 1,
    /// <summary>
    /// POBICOS node simulator enum
    /// </summary>
    NODE = 2
}
/// <summary>
/// Link status options
/// </summary>
public enum LinkStatus
{
    /// <summary>
    /// Enum - ON
    /// </summary>
    ON,
    /// <summary>
    /// Enum - off
    /// </summary>
    OFF
}
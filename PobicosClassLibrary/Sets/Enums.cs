
/// <summary>
/// List of Pobicos instructions
/// </summary>
public enum InstructionsList
{
    /// <summary>
    /// 
    /// </summary>
    EnableEvent,
    /// <summary>
    /// 
    /// </summary>
    CreateGenericAgent,
    /// <summary>
    /// 
    /// </summary>
    CreateNonGenericAgents,
    Release,
    /// <summary>
    /// 
    /// </summary>
    GetChildInfo,
    /// <summary>
    /// 
    /// </summary>
    SendCommand,
    GetCommand,
    SendReport,
    GetReport,
    SetTimer,
    GetTimerId,
    CreateReportList,
    DestroyReportList,
    AddReport,
    GetNextReport,
    DisableEvent,
    GetMyID,
    GetConfigSetting,
    DbgString,
    DbgUInt32,
	GetTemp = 290455552,
    GetBrightness,
    Alert,
    AlertAurally,
    AlertByVoice,
    AlertByVoiceInPolish,
    AlertBySound,
    AlertBySirenSound,
    AlertVisually,
    AlertByDisplayingText,
    ConveyMessage,
	ConveyMessageByText = 889192448,
    ConveyMessageByVoice,
    SetTempRange,
    SetTemp,
    SetBrightness,
    Close,
    SwitchOn,
    SwitchOff,
    TurnDownBrightness,
    TurnDownTemp,
    TurnUpTemp,
    TurnUpBrightness
}
/// <summary>
/// List of Pobicos events
/// </summary>
public enum EventsList
{
    ponge_originated_event_switch_originated_event = 0x11130000,
	SmokeEvent,	//temp - do czasu zestandaryzwooania sposobu czytania eventów  xmla
    Init,
    Finalize,
    ChildCreated,
    ChildCreationTimeout,
    ChildUnreachable,
    CommandArrived,
    ReportArrived,
    Timeout,
    ConfigSettingsChanged,
    ThresholdExceeded,
    TemperatureRangeExceeded,
    TemperatureTooHigh,
    TemperatureTooLow,
    BightnessRangeExceeded,
    BightnessTooHigh,
    BightnessTooLow,
    HumidityRangeExceeded,
    HumidityTooHigh,
    HumidityTooLow,
    ObjectClosed,
    DangerousElementDetected,
    SmokeDetected,
    CabonOxideDetected,
    FireDetected,
    FireAlarmActivated,
    ObjectSwitched,
    ObjectSwitchedOn,
    ObjectSwitchedOff,
    ObjectOpened,
    

}
/// <summary>
/// client types
/// </summary>
public enum clientType
{
    OBJECT = 1,
    NODE = 2
}
/// <summary>
/// link status options
/// </summary>
public enum LinkStatus
{
    ON,
    OFF
}
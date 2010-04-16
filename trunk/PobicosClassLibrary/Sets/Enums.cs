

public enum InstructionsList
{
    EnableEvent,
    CreateGenericAgent,
    CreateNonGenericAgents,
    Release,
    GetChildInfo,
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
    GetTemp,
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
    ConveyMessageByText,
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

public enum clientType
{
    OBJECT = 1,
    NODE = 2
}

public enum LinkStatus
{
    ON,
    OFF
}


public enum InstructionsList
{
    pongiSwitchOn = 0x22450000,
    pongiSwitchOff = 0x42450000,
    pongiAlert = 0x25000000,
    pongiGetTemp = 0x11500000,
    pongiGetBrightness = 0x21500000
}

public enum EventsList
{
    PONGE_ORIGINATED_EVENT_SWITCH_ORIGINATED_EVENT = 0x11130000,
	smokeEvent	//temp - do czasu zestandaryzwooania sposobu czytania eventów  xmla
}

public enum clientType
{
    OBJECT = 1,
    NODE = 2
}
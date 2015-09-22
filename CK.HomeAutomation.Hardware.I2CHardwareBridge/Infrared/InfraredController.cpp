#include "Infrared\Infrared.h"
#include <Wire.h>

#define INFRARED_RECEIVER_FREQUENCY FREQUENCY_38Khz
#define INFRARED_SENDER_FREQUENCY FREQUENCY_38Khz

byte _cachedSignal[128];
byte _cachedSignalIndex = 0;
InfraredSender _irs = InfraredSender(PD4, FREQUENCY_38Khz);
//InfraredReceiver _irr = InfraredReceiver();

void InfraredController_handleI2CWrite(int dataLength)
{
	// data[0]=action
	// data[1]=irAction
	//	> 1=FILL_SIGNAL_CACHE > [2]=offset, [3]=length
	//	> 2=SEND_CACHED_SIGNAL
	if (dataLength < 2)
	{
		return;
	}

	byte irAction = Wire.read();
	switch (irAction)
	{
		case 1:
		{
			_cachedSignalIndex = Wire.read();
			byte length = Wire.read();

			for (byte i = 0; i < length; i++)
			{
				_cachedSignal[_cachedSignalIndex] = Wire.read();
				_cachedSignalIndex++;
			}
			
			break;
		}

		case 2:
		{
			_irs.sendSignalRawArray(_cachedSignal, _cachedSignalIndex);
			break;
		}
	}
}

void InfraredController_sendTestSignal()
{
	byte signal[67] = { 17,8,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,1,1,1,1,1,1,3,1,1,1,1,1,1,1,1,1,3,1,3,1,3,1,1,1,3,1,3,1,3,1,3,1 };
	_irs.sendSignalRawArray(signal, 67);
}
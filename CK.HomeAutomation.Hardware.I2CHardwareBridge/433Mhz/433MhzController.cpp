#include <Arduino.h>
#include <Wire.h>

#include "433Mhz\RCSwitch.h"

#define RCSWITCH_PIN PD7+3
#define RCSWITCH_PIN PD5

RCSwitch _rcs = RCSwitch();

void _433MhzController_setup()
{
	_rcs.enableTransmit(RCSWITCH_PIN);
	_rcs.enableReceive(INT1);

}

void _433MhzController_checkForReceivedSignal()
{
	if (!_rcs.available())
	{
		return;
	}

	Serial.println("Received value: " + String(_rcs.getReceivedValue()));
	Serial.println("Received length: " + String(_rcs.getReceivedBitlength()));

	_rcs.resetAvailable();

}

void _433MhzController_handleI2CWrite(int dataLength)
{
	// Byte 0 = Action; Byte 1-4 = code; Byte 5 = length
	if (dataLength != 6)
	{
		return;
	}

	unsigned long buffer[4];
	buffer[0] = Wire.read();
	buffer[1] = Wire.read();
	buffer[2] = Wire.read();
	buffer[3] = Wire.read();

	byte length = Wire.read();
	unsigned long code = buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0];

#if (DEBUG)
	Serial.println("Sending 433Mhz signal with code " + String(code) + " and length " + String(length) + ".");
#endif

	_rcs.send(code, length);
}

void _433MhzController_sendTestSignal()
{
	_rcs.send(21UL, 24U);
}
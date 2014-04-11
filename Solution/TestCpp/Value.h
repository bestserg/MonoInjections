#pragma once

using namespace SecureField;

ref class Value
{
	int _number;
public:
	Value(void);

	[SecureField]
	property int Number
	{
		int get()
		{
			return _number;
		}
		void set(int value)
		{
			_number = value;
		}
	}
};


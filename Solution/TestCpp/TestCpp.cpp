// TestCpp.cpp : main project file.

#include "stdafx.h"
#include "Value.h"

using namespace System;

int main(array<String ^> ^args)
{
    Value ^val = gcnew Value();
    
    while (true) 
    {
        Console::Write("Enter: ");
        String ^str = Console::ReadLine();
        if (str == "p")
        {
            Console::WriteLine("value: {0}", val->Number);
            Console::ReadLine();
        }
        else
        {
            int n = Int32::Parse(str);
            val->Number = n;
        }
                
        Console::Clear();
    }
    return 0;
}

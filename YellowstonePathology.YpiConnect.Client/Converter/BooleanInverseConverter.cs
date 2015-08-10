﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace YellowstonePathology.YpiConnect.Client.Converter
{
	[ValueConversion(typeof(bool), typeof(bool))]
	class BooleanInverseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{            
            bool result = false;
            if ((bool)value == false) result = true;
            return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{            
            return DependencyProperty.UnsetValue;
		}
	}	
}

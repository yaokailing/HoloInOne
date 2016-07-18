﻿/*
    Copyright (c) Microsoft Corporation All rights reserved.  
 
    MIT License: 
 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    documentation files (the  "Software"), to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
    and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
 
    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Accelerometer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    partial class MainPage
    {
        private App viewModel;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.StatusMessage = "Running ...";

            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    this.viewModel.StatusMessage = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }

                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    int samplesReceived = 0; // the number of Accelerometer samples received

                    // Subscribe to Accelerometer data.
                    bandClient.SensorManager.Accelerometer.ReadingChanged += (s, args) => 
                    {
                        samplesReceived++;
                        Accelerometer_ReadingChanged(s, args);
                    };
                    await bandClient.SensorManager.Accelerometer.StartReadingsAsync();
                    
                    // Receive Accelerometer data for a while, then stop the subscription.
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await bandClient.SensorManager.Accelerometer.StopReadingsAsync();
                    this.viewModel.StatusMessage = string.Format("{0} Accelerometer samples were collected in 1 second.", samplesReceived);
                }
            }
            catch (Exception ex)
            {
                this.viewModel.StatusMessage = ex.ToString();
            }
        }

        private void Accelerometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            IBandAccelerometerReading accel = e.SensorReading;
            string text = string.Format("X = {0:G4}\nY = {1:G4}\nZ = {2:G4}", accel.AccelerationX, accel.AccelerationY, accel.AccelerationZ);
            System.Diagnostics.Debug.WriteLine(text);
        }


    }
}

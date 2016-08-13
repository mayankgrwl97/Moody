using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bing.Speech;
using Bing.Speech.Xaml;
using System.Net.Http;
using System.Threading;
using System.Globalization;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App6
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
     
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        SpeechRecognizer SR;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SpeechControl.SpeechRecognizer = SR;
            var credentials = new SpeechAuthorizationParameters();
            credentials.ClientId = "7073836075";
            credentials.ClientSecret = "rzrClqMpGP88fG1hz1dJOYLuRbMrFf0bVgV/drOu0ss=";
            try
            {
                SR = new SpeechRecognizer("en-US", credentials);
            }
            catch (Exception ex)
            {
                ResultText.Text = "" + ex;
            }
        }


        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            String s;
            try
            {
                var result = await SR.RecognizeSpeechToTextAsync();
                s = result.Text;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == ' ')
                    {
                        s = s.Replace(' ', '_');
                    }
                    else if (s[i] == '_')
                        continue;
                    else if (s[i] < 65 || (s[i] > 90 && s[i] < 97))
                    {
                        s = s.Remove(i, 1);
                    }
                    else if (s[i] > 122)
                    {
                        s = s.Remove(i, 1);
                    }

                }
                ResultText.Text = result.Text;

                HttpClient httpClient = new HttpClient();
                string url = "http://127.0.0.1:8000/ml/" + s;
 
                var response = await httpClient.GetAsync(url);
                // var response = await httpClient.GetAsync("http://192.168.1.121:8000/ml/i_am_happy");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                //ResultText.Text = content;
                string[] tokens = content.Split(':', ',');
                string mood;
                //string comp = tokens[1];
                tokens[1] = tokens[1].Trim().Substring(1, tokens[1].Length - 3);
                //BoxText.Text = tokens[1];
               // LayoutRoot.Background = background;

                if (Double.Parse(tokens[1], CultureInfo.CurrentCulture) == 0)
              {
                  mood = "Undetermined";
                   
                }

              else if (Double.Parse(tokens[1], CultureInfo.CurrentCulture) >=0.5) 
              {
                  mood = "Happy";
                    
                }
              else if (Double.Parse(tokens[1], CultureInfo.CurrentCulture) < 0.5 && Double.Parse(tokens[1], CultureInfo.CurrentCulture) > 0)
              {
                  mood = "Cheerful";
                   
                }
              else if (Double.Parse(tokens[1], CultureInfo.CurrentCulture) < 0 && Double.Parse(tokens[1], CultureInfo.CurrentCulture) >= -0.5)
              {
                  mood = "Gloomy";
                   
                } 
              else
              {
                  mood = "Sad";
                    
                }

              BoxText.Text = mood; 
          } 
            catch (Exception ex)
            {
                s = "";
                ResultText.Text = "Please Try Again";
            }
            
        }
        

        private void video_Click(object sender, RoutedEventArgs e)
        {
                       
        }

        private async void music_Click(object sender, RoutedEventArgs e)
        {
            if (BoxText.Text.Equals("Happy"))
            {
                myControl1.Stop();
                myControl2.Stop();
                myControl3.Stop();
                myControl.Play();
            }
                
            else if (BoxText.Text.Equals("Sad"))
            {
                myControl.Stop();
                myControl2.Stop();
                myControl3.Stop();
                myControl1.Play();
            }
            
            else if (BoxText.Text.Equals("Cheerful"))
            {
                myControl1.Stop();
                myControl.Stop();
                myControl3.Stop();
                myControl2.Play();
            }
            else  if (BoxText.Text.Equals("Gloomy"))
            {
                myControl1.Stop();
                myControl2.Stop();
                myControl.Stop();
                myControl3.Play();
            }

        }
    }
}

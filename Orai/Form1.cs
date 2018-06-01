using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;
using SimpleWifi;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace Orai
{


    public partial class Form1 : Form
    {
         static string Google_API_Key;

        public Form1()
        {
            InitializeComponent();

            StreamReader reader = new StreamReader("gkey.txt");

            Google_API_Key = reader.ReadLine();
        }
       


        private async void button1_Click(object sender, EventArgs e)
        {
            var response = await GetGeoData();

            Geo coordinates = JsonConvert.DeserializeObject<Geo>(response);

            var coordinates_string = coordinates.Location.Lat.ToString().Replace(',','.') + ", " + coordinates.Location.Lng.ToString().Replace(',', '.');


            response = await GetCity(coordinates);


            Adresas adresas = JsonConvert.DeserializeObject<Adresas>(response);

            textBox1.Text = adresas.Results[1].FormattedAddress.ToString() + " (" + coordinates_string + ")";

        }

        private static async Task<string> GetGeoData()
        {
            HttpClient httpClient = new HttpClient();

            string myJson = "";

            var response = await httpClient.PostAsync("https://www.googleapis.com/geolocation/v1/geolocate?key=" + Google_API_Key,
                new StringContent(myJson, Encoding.UTF8, "application/json"));
            string content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private static async Task<string> GetCity(Geo data)
        {
            HttpClient httpClient = new HttpClient();

            string myJson = 
                
                "latlng=" + data.Location.Lat.ToString().Replace(',', '.') + "," +data.Location.Lng.ToString().Replace(',', '.') + "&" + "key=" + Google_API_Key;

            var send = new StringContent(myJson, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://maps.googleapis.com/maps/api/geocode/json?" + myJson, send);
                
            string content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}

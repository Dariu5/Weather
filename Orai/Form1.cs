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
using System.Xml.Serialization;

namespace Orai
{

    public class SaveLoad

    {
        public string Address { get; set; }
        public Geo Coordinates { get; set; }


    
        public void Restore()
        {
            XmlSerializer x = new XmlSerializer(typeof(SaveLoad));
            FileStream reader = new FileStream("settings.xml", FileMode.Open);
            SaveLoad temp = (SaveLoad)x.Deserialize(reader);

            this.Address = temp.Address;
            this.Coordinates = temp.Coordinates;


        }

        

        public void Save(string address, Geo geo)

        {
            this.Address = address;
            this.Coordinates = geo;
            XmlSerializer x = new XmlSerializer(typeof(SaveLoad));
            TextWriter writer = new StreamWriter("settings.xml");
            x.Serialize(writer, this);



        }



    }


    public partial class Form1 : Form
    {
         static string Google_API_Key;

        public Form1()
        {
            InitializeComponent();

            StreamReader reader = new StreamReader("gkey.txt");

            Google_API_Key = reader.ReadLine();

            SaveLoad restore = new SaveLoad();

            restore.Restore();

            UpdateLocationTextbox(restore.Coordinates, restore.Address);

        }
       


        private async void button1_Click(object sender, EventArgs e)
        {
            var response = await GetGeoData();

            Geo coordinates = JsonConvert.DeserializeObject<Geo>(response);

            response = await GetCity(coordinates);

            Adresas adresai = JsonConvert.DeserializeObject<Adresas>(response);

            string adresas = adresai.Results[1].FormattedAddress.ToString();
            UpdateLocationTextbox(coordinates, adresas);

            SaveLoad save = new SaveLoad();

            save.Save(adresas, coordinates);


        }

        private void UpdateLocationTextbox(Geo coordinates, string adresas)
        {
            var coordinates_string = coordinates.Location.Lat.ToString().Replace(',', '.') + ", " + coordinates.Location.Lng.ToString().Replace(',', '.');
            textBox1.Text = adresas + " (" + coordinates_string + ")";
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://darksky.net/poweredby/");
        }
    }
}

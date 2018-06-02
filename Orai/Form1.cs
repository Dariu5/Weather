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
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;

namespace Orai
{

  
    public partial class Form1 : Form
    {
         static string Google_API_Key;
         static string DarkSky_API_Key;
        SaveLoad save = new SaveLoad();

        public Form1()
        {
            InitializeComponent();

            StreamReader reader = new StreamReader("gkey.txt");

            Google_API_Key = reader.ReadLine();
            DarkSky_API_Key = reader.ReadLine();

            

            save.Restore();

            UpdateLocationTextbox(save.Coordinates, save.Address);

        }

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

        private async void button1_Click(object sender, EventArgs e)
        {

            textBox1.Text = "...";


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
                       

            var response = await httpClient.PostAsync("https://www.googleapis.com/geolocation/v1/geolocate?key=" + Google_API_Key, null);
            string content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private static async Task<string> GetCity(Geo data)
        {
            HttpClient httpClient = new HttpClient();

            string myJson = 
                
                "latlng=" + data.Location.Lat.ToString().Replace(',', '.') + "," +data.Location.Lng.ToString().Replace(',', '.') + "&" + "key=" + Google_API_Key;

            var response = await httpClient.PostAsync("https://maps.googleapis.com/maps/api/geocode/json?" + myJson, null);
                
            string content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private static async Task<string> GetWheather(Geo data)
        {
            HttpClient httpClient = new HttpClient();

            string key = "[" + DarkSky_API_Key + "]/";
            string coord = data.Location.Lat.ToString().Replace(',', '.') + "," + data.Location.Lng.ToString().Replace(',', '.');
                

           var response = await httpClient.GetAsync("https://api.darksky.net/forecast/"+ DarkSky_API_Key + "/" + coord + "?units=si");

            string content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://darksky.net/poweredby/");
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            var response = await GetWheather(save.Coordinates);
            var orai = DarkSky.FromJson(response);

            label_temp.Text = orai.Currently.Temperature.ToString() + "°C";
           
            pictureBox1.Image = UpdateIcon(orai.Currently.Icon);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            //dataGridView1.DataSource = orai.Hourly.Data;

            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            foreach (var item in orai.Hourly.Data)
            {



                dataGridView1.Rows.Add(dateTime.AddSeconds(item.Time).ToString(),item.PrecipIntensity.ToString(),
                    item.Temperature.ToString(), item.CloudCover.ToString(),item.WindSpeed.ToString(),
                    UpdateIcon(item.Icon));    


            }

        }

        private Image UpdateIcon(string ikon)
        {
            /*http://adamwhitcroft.com/climacons/*/
            /*https://www.flaticon.com/packs/weather-icons*/

            switch (ikon)

            {
                case "clear-day":

                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");

                case "clear-night":
                    return  Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");
                  

                case "rain":
                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//rain.png");
                 
                case "snow":

                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");

                case "sleet":

                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");

                case "wind":

                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");

                case "fog":

                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");

                case "cloudy":
                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//cloud.png");
                   

                case "partly-cloudy-day":
                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//cloudy.png");
                   

                case "party-cloudy-night":
                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//cloudy.png");                  

                default:
                    return Image.FromFile(Environment.CurrentDirectory + "//Icons//sun.png");
                    

            }
        }
    }
}

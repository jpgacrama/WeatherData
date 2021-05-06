using System;
using System.IO;
using System.Text.Json;

namespace DebugTest
{
    internal class Program
    {
        private string location;
        private string id;
        private string latLong;

        public Program()
        {
            location = "";
            id = "";
            latLong = "";
        }

        private string getDataFromLocation(string location)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            var response = client.GetAsync("https://www.metaweather.com/api/location/search/?query=" + location).Result;
            var streamReader = new StreamReader(response.Content.ReadAsStreamAsync().Result);
            var content = streamReader.ReadToEnd();
            id = content.Split("woeid\":")[1].Split(",")[0];
            return id;
        }

        private string getDataFromWebURI()
        {
            Console.WriteLine("Do you want to enter location (y/n):");

            var choice = location = Console.ReadLine();
            if (choice == "y" || choice == "Y")
            {
                Console.WriteLine("Enter Desired Location:");
                location = Console.ReadLine();
            }
            else {
                Console.WriteLine("Enter Desired <Latitude,Longitude>:");
                latLong = Console.ReadLine();
            }

            id = getDataFromLocation(location);

            return id;
        }

        private void getMetaWeatherData()
        {
            System.Net.Http.HttpClient client2 = new System.Net.Http.HttpClient();
            var response2 = client2.GetAsync("https://www.metaweather.com/api/location/" + id).Result;
            var streamReader2 = new StreamReader(response2.Content.ReadAsStreamAsync().Result);
            var data = streamReader2.ReadToEnd();
            WeatherInfo weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(data);
            Console.WriteLine(location + " max temp: " + weatherInfo.consolidated_weather[0].max_temp + " min temp: " + weatherInfo.consolidated_weather[0].min_temp);

            writeWeatherDataToFile(weatherInfo);
        }

        private void writeWeatherDataToFile(WeatherInfo weatherInfo)
        {
            try
            {
                string fileName = "weather" + location + DateTimeOffset.Now.ToUnixTimeSeconds() + ".json";
                FileStream stream = new FileStream(fileName, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(stream);
                sw.Write(JsonSerializer.Serialize(weatherInfo));
                Console.WriteLine("Writing to {0}", fileName);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private static void Main(string[] args)
        {
            Program program = new Program();
            program.id = program.getDataFromWebURI();
            program.getMetaWeatherData();
        }
    }

    public class WeatherInfo
    {
        public Consolidated_Weather[] consolidated_weather { get; set; }
        public DateTime time { get; set; }
        public DateTime sun_rise { get; set; }
        public DateTime sun_set { get; set; }
        public string timezone_name { get; set; }
        public Parent parent { get; set; }
        public Source[] sources { get; set; }
        public string title { get; set; }
        public string location_type { get; set; }
        public int woeid { get; set; }
        public string latt_long { get; set; }
        public string timezone { get; set; }
    }

    public class Parent
    {
        public string title { get; set; }
        public string location_type { get; set; }
        public int woeid { get; set; }
        public string latt_long { get; set; }
    }

    public class Consolidated_Weather
    {
        public long id { get; set; }
        public string weather_state_name { get; set; }
        public string weather_state_abbr { get; set; }
        public string wind_direction_compass { get; set; }
        public DateTime created { get; set; }
        public string applicable_date { get; set; }
        public float min_temp { get; set; }
        public float max_temp { get; set; }
        public float the_temp { get; set; }
        public float wind_speed { get; set; }
        public float wind_direction { get; set; }
        public float air_pressure { get; set; }
        public int humidity { get; set; }
        public float visibility { get; set; }
        public int predictability { get; set; }
    }

    public class Source
    {
        public string title { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        public int crawl_rate { get; set; }
    }
}
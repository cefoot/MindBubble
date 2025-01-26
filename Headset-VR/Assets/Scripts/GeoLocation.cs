using Newtonsoft.Json;

[System.Serializable]
public class GeoLocation
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("countryCode")]
    public string CountryCode { get; set; }

    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("regionName")]
    public string RegionName { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("zip")]
    public string Zip { get; set; }

    [JsonProperty("lat")]
    public float Latitude { get; set; }

    [JsonProperty("lon")]
    public float Longitude { get; set; }

    [JsonProperty("timezone")]
    public string Timezone { get; set; }

    [JsonProperty("isp")]
    public string ISP { get; set; }

    [JsonProperty("org")]
    public string Organization { get; set; }

    [JsonProperty("as")]
    public string AS { get; set; }

    [JsonProperty("query")]
    public string Query { get; set; }
}

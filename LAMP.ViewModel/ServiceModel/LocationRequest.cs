namespace LAMP.ViewModel
{
    /// <summary>
    /// Class LocationRequest
    /// </summary>
    public class LocationRequest
    {
        public long UserID { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public byte Type { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}

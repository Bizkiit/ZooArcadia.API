namespace ZooArcadia.API.Services
{
    public class ImageService
    {
        public string ConvertToBase64(byte[] imageData)
        {
            return Convert.ToBase64String(imageData);
        }

        public byte[] ConvertFromBase64(string base64Image)
        {
            return Convert.FromBase64String(base64Image);
        }
    }
}

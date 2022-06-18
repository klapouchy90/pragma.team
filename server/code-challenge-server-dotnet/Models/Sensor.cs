namespace server.Models
{
    public class Sensor
    {
        public string Id { get; set; }

        public int Temperature { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Sensor)
            {
                var sensor = obj as Sensor;
                return sensor.Id == Id && sensor.Temperature == Temperature;
            }

            return false;
        }
    }
}
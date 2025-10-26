using SGPdotNET.TLE;
using SGPdotNET.Propagation;

class Program
{
    static void Main()
    {
        // TLE для МКС
        var tle = new Tle(
            "ISS (ZARYA)",
            "1 25544U 98067A   19034.73310439  .00001974  00000-0  38215-4 0  9991",
            "2 25544  51.6436 304.9146 0005074 348.4622  36.8575 15.53228055154526"
        );

        // Создаём экземпляр SGP4 по TLE
        var sgp4 = new Sgp4(tle);

        // Время, на которое считаем позицию
        DateTime now = DateTime.UtcNow;

        // Позиция спутника в ECI
        var eci = sgp4.FindPosition(now);

        // Переводим ECI → геодезические координаты
        var geo = eci.ToGeodetic();

        Console.WriteLine($"Время UTC: {now}");
        Console.WriteLine($"Широта: {geo.Latitude.Degrees:F6}°");
        Console.WriteLine($"Долгота: {geo.Longitude.Degrees:F6}°");
        Console.WriteLine($"Высота: {geo.Altitude:F3} км");
    }
}
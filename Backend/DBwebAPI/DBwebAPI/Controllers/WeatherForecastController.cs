using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;

namespace DBwebAPI.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    //public class DBTest : ControllerBase
    //{
    //    [HttpGet]
    //    public string Login(int user_id,string password)
    //    {
            //return null;
            //DBconn dbconn = new DBconn();
            //SqlSugarClient sqlORM = null;
            //dbconn.Conn();
            //sqlORM = dbconn.sqlORM;
            //return sqlORM.Queryable<Usr>().Select(it => it.userName).ToList()[0].ToString();





            //try
            //{
            //    if (dbconn.Conn() == true)
            //        sqlORM = dbconn.sqlORM;
            //    //return sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id && it.userPassword == password).ToList().ToString();
            //    Usr usr= new Usr();
            //    return sqlORM.Queryable<Usr>().Select(it => it.userName).ToList().ToString();
            //}
            //catch (Exception ex)
            //{
            //    return "≤È’“ ß∞‹";
            //}

            //try
            //{
                

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        //}
    //    private static readonly string[] Summaries = new[]
    //    {
    //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //};

    //    private readonly ILogger<WeatherForecastController> _logger;

    //    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    //    {
    //        _logger = logger;
    //    }

    //    [HttpGet(Name = "GetWeatherForecast")]
    //    public IEnumerable<WeatherForecast> Get()
    //    {
    //        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //        {
    //            Date = DateTime.Now.AddDays(index),
    //            TemperatureC = Random.Shared.Next(-20, 55),
    //            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //        })
    //        .ToArray();
    //    }
    //}
}
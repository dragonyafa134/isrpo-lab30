using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using HeroesApi.Models;
using HeroesApi.Data;

namespace HeroesApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase {
    [HttpGet]
    public ActionResult<List<Hero>> GetAll() {
        return Ok(HeroesStore.Heroes);
    }
    
    [HttpGet("{id}")]
    public ActionResult<Hero> GetById(int id) {
        var hero = HeroesStore.Heroes.FirstOrDefault(h => h.Id == id);
        if (hero is null) {
            return NotFound(new{message = $"Герой с id= {id} не найден"});
        }
        return Ok(hero);
    }


    [HttpGet("demo")]
    public ActionResult GetDemo() {
        var hero = HeroesStore.Heroes.First();
        var defaultOptions = new JsonSerializerOptions {
            WriteIndented = true
        };
        var ourOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        return Ok(new {
            withDefaultSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, defaultOptions), defaultOptions),
            withOurSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, ourOptions), ourOptions),
            note = "Сравните имена полей и значение universe в обоих случаях."
        });
    }

    [HttpGet("serialize")]
    public ActionResult GetSerialize() {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = {new JsonStringEnumConverter()}
        };
        var hero = new Hero {
            Id = 99,
            Name = "Тестовый герой",
            RealName = "Студент",
            Universe = Universe.Marvel,
            PowerLevel = 50,
            Powers = new() {"Программирование", "Дебаггинг"},
            Weapon = new() {Name = "Клавиатура", IsRanged = false},
            InternalNotes = "Это поле не попадет в Json"
        };
        string serialized = JsonSerializer.Serialize(hero, options);
        var deserialized = JsonSerializer.Deserialize<Hero>(serialized, options);
        return Ok(new {
            serializedJson = serialized,
            deserializedObject = deserialized,
            internalNotesAfterDeserialize = deserialized?.InternalNotes ?? "null - поле не было сериализовано, как и ожидалось"
        });
    }

    [HttpGet("search")]
    public ActionResult<List<Hero>> Search([FromQuery] string? name = null){
        if (string.IsNullOrWhiteSpace(name)){
            return BadRequest(new { Message = "Параметр name не может быть"});
        }
        var results = HeroesStore.Heroes
            .Where(h => h.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Ok(results);
    }

}
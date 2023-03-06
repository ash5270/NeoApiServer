using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controlers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{

    public PizzaController()
    {
        Console.WriteLine("test");
    }

    [HttpGet]
    public async Task<ActionResult<List<Pizza>>> GetAll()
    {
        Console.WriteLine("Get ");
        Console.WriteLine("Requset");
        return PizzaService.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<Pizza> Get(int id)
    {
        Console.WriteLine("Get id ");
        var pizza = PizzaService.Get(id);
        if (pizza == null)
            return NotFound();
        return pizza;
    }

    [HttpPost]
    public IActionResult Create(Pizza pizza)
    {
        Console.WriteLine($"post : {pizza.Name},{pizza.IsGlutenFree}");
        PizzaService.Add(pizza);
        return CreatedAtAction(nameof(Create), new { id = pizza.Id }, pizza);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Pizza pizza)
    {
        Console.WriteLine("Put ");
        if (id != pizza.Id)
        {
            Console.WriteLine($"id: {id},pizza.id: {pizza.Id}");
            return BadRequest();
        }
        var exitsingPizza = PizzaService.Get(id);
        if (exitsingPizza is null)
            return NotFound();
        PizzaService.Update(pizza);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Console.WriteLine("Delete ");
        var pizza = PizzaService.Get(id);
        if (pizza == null)
            return NotFound();

        PizzaService.Delete(id);
        return NoContent();
    }
}
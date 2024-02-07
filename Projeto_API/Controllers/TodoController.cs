using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_API.Data;
using Projeto_API.Models;
using Projeto_API.ModelViews;

namespace Projeto_API.Controllers {
    [ApiController]
    [Route (template: "v1")]
    public class TodoController : ControllerBase {
        [HttpGet]
        [Route (template: "todo")]
        public async Task<IActionResult> GetAsync ([FromServices] AppDbContext context) {
            var lista = await context
                .Todos
                .AsNoTracking ()
                .ToListAsync ();
            return Ok (lista);
        }

        [HttpGet]
        [Route (template: "todo/{id}")]
        public async Task<IActionResult> GetByIdAsync ([FromServices] AppDbContext context, [FromRoute] int id) {
            var lista = await context
                .Todos.AsNoTracking ()
                .FirstOrDefaultAsync (x => x.Id == id);

            return lista == null ?
                NotFound () :
                Ok (lista);
        }

        [HttpPost (template: "todo")]
        public async Task<IActionResult> PostAsync ([FromServices] AppDbContext context, [FromBody] CreateTodoViewModel model) {
            if (!ModelState.IsValid)
                return BadRequest ();

            var tarefa = new Todo {
                Title = model.Title,
                Data = DateTime.Now,
                Done = false
            };

            try {
                await context.AddAsync (tarefa);
                await context.SaveChangesAsync ();
                return Created ($"v1/todo/{tarefa.Id}", tarefa);
            } catch (Exception e) {
                return BadRequest ();
            }
        }

        [HttpPut (template: "todo/{id}")]
        public async Task<IActionResult> UpdateAsync ([FromServices] AppDbContext context, [FromBody] CreateTodoViewModel model, [FromRoute] int id) {
            if (!ModelState.IsValid)
                return BadRequest ();

            var todo = await context.Todos.FirstOrDefaultAsync (x => x.Id == id);

            if (todo == null)
                return NotFound ();

            try {
                todo.Title = model.Title;
                context.Todos.Update (todo);
                await context.SaveChangesAsync ();
                return Ok ();
            } catch (System.Exception) {
                return BadRequest ();
            }

        }

        [HttpDelete (template: "todo/{id}")]
        public async Task<IActionResult> DeleteAsync ([FromServices] AppDbContext context, int id) {
            var todo = await context.Todos.FirstOrDefaultAsync (x => x.Id == id);
            if (todo == null)
                return NotFound ();

            context.Todos.Remove (todo);
            await context.SaveChangesAsync ();
            return Ok ();
        }

    }
}
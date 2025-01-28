using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserScoresController : ControllerBase
    {
        private readonly GameDatabaseContext _context;
        private readonly ILogger<UserScoresController> _logger;

        public UserScoresController(GameDatabaseContext context, ILogger<UserScoresController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: api/UserScores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserScore>>> GetUserScores()
        {
            return await _context.UserScores.ToListAsync();
        }

        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult<IEnumerable<UserScore>>> GetTop10Score()
        {
            List<UserScore> scores = await _context.UserScores.ToListAsync();
            return scores.OrderByDescending(s => s.Score).Take(10).ToList();
        }

        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult<UserScore>> getTopScore()
        {
            List<UserScore> scores = await _context.UserScores.ToListAsync();
            if (scores.Count == 0)
                return new UserScore { Id = 0, Name = "No user", Score = 0 };
            else
                return scores.OrderByDescending(s => s.Score).FirstOrDefault();
        }

        // POST: api/UserScores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserScore>> PostUserScore(UserScore userScore)
        {
            if (userScore == null || string.IsNullOrWhiteSpace(userScore.Name) || userScore.Score < 0)
            {
                return BadRequest("Invalid score data.");
            }

            try
            {
                _context.UserScores.Add(userScore);
                await _context.SaveChangesAsync();

                return CreatedAtAction("PostUserScore", new { id = userScore.Id }, userScore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the user score.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/UserScores/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserScore(int id)
        {
            var userScore = await _context.UserScores.FindAsync(id);
            if (userScore == null)
            {
                return NotFound();
            }

            _context.UserScores.Remove(userScore);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/UserScores
        [HttpDelete]
        public async Task<IActionResult> DeleteAllUserScores()
        {
            _context.UserScores.RemoveRange(_context.UserScores);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

using APS_Project.Data;
using APS_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APS_Project.Pages
{
    [Authorize]
    public class MyRecipesModel : PageModel
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppUser AppUser { get; set; }
        public List<Recipe> Recipes { get; set; }

        public MyRecipesModel(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                AppUser = _dbContext.AppUsers.Find(userId);
            }
        }
        public async Task OnGetAsync()
        {
            _ = await _dbContext.UserLikeRecipes.ToListAsync();
            _ = await _dbContext.UserDislikeRecipes.ToListAsync();
            _ = await _dbContext.UserFollowRecipes.ToListAsync();
            _ = await _dbContext.Recipes.ToListAsync();
            Recipes = AppUser.UserRecipes;
        }
        public IActionResult OnPostSearch(string category, DateTime startTime, DateTime endTime)
        {
            _ = _dbContext.Recipes.ToList();
            Recipes = AppUser.UserRecipes;
            _ = _dbContext.Categories.ToList();
            if (startTime != DateTime.MinValue)
            {
                Recipes = Recipes
                    .Where(p => p.PublicationDate >= startTime)
                    .ToList();
            }
            if (endTime != DateTime.MinValue)
            {
                Recipes = Recipes
                    .Where(p => p.PublicationDate <= endTime)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(category))
            {
                Recipes = Recipes
                    .Where(p => p.Categories.Any(p => p.Name == category))
                    .ToList();

            }
            if (Recipes is null)
                Recipes = new List<Recipe>();
            return Page();
        }
    }
}

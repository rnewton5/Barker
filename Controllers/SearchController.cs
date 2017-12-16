using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Barker.Models;
using Barker.Data;
using Barker.Models.SearchViewModels;

namespace Barker.Controllers
{
    public class SearchController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<User> _userManager;

        public SearchController(BarkerDbContext context, UserManager<User> userManager){
            _context = context;
            _userManager = userManager;
        }

        [Route("")]
        public IActionResult Index(string searchTerm)
        {
            if (searchTerm == null)
                return View(new SearchViewModel() { Users = new List<UserSearchItem>() });
            var loggedInUserId = _userManager.GetUserId(User);
            var matchingUsers = _context.Users.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower())).ToList();
            var SearchVm = new SearchViewModel() { Users = new List<UserSearchItem>() };
            foreach(var user in matchingUsers) {
                SearchVm.Users.Add(new UserSearchItem() {
                    UserName = user.UserName,
                    BarksCount = _context.Posts.Count(p => p.User == user),
                    FollowersCount = _context.Follows.Count(f => f.FolloweeId == user.Id),
                    isFollowing = _context.Follows.Any(f => f.FolloweeId == user.Id && f.FollowerId == loggedInUserId)
                });
            }
            return View(SearchVm);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EXAMcsharp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;



namespace EXAMcsharp.Controllers
{

    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }



        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Dashboard"); 
            }
            else
            {
                return View("Index");
            }
        }



        [HttpPost("login")]
        public IActionResult Login(LoginUser logUser)
        {
            if (ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault( u => u.Email == logUser.LoginEmail);
                if (userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "That email/password is not signed up yet");
                    return View ("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Not the right password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }



        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
           if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.currentUser = currentUser;

            List<Actvty> eachActvty = dbContext.Actvtys.Include(w => w.Memberships).ThenInclude(a => a.User).OrderBy(e => e.Date).ToList();

            foreach(Actvty a in eachActvty.ToList())
            {
                if(a.Date < DateTime.Now)
                {
                    eachActvty.Remove(a);
                }
            }
            ViewBag.AllActivities = eachActvty;

            List<User> Creators = dbContext.Users.ToList();
            ViewBag.Creators = Creators;
            return View();


                //User dbUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
                
            
        }





        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }





















            
        [HttpGet("addactvty")]
        public IActionResult AddActvty()
        {
            return View("AddActvty"); 
        }



        [HttpPost("createactvty")]
        public IActionResult CreateActvty(Actvty newActvty)
        {   
            if(ModelState.IsValid)
            {
                newActvty.UserId = (int)HttpContext.Session.GetInt32("UserId");
                dbContext.Add(newActvty); 
                dbContext.SaveChanges();
                Actvty thisActvty = dbContext.Actvtys.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
                return Redirect($"/actvty/"+thisActvty.ActvtyId); 
            }
            return View("AddActvty", newActvty);
         

        }

        [HttpGet("actvty/{ActvtyId}")]
        public IActionResult Actvty(int ActvtyId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.currentUser = currentUser;

            Actvty myActvty = dbContext.Actvtys.FirstOrDefault(w => w.ActvtyId == ActvtyId);
            ViewBag.myActivity = myActvty;

            User aCoord = dbContext.Users.FirstOrDefault(ec => ec.UserId == myActvty.UserId);
            ViewBag.ACoordinator = aCoord;

            var aMembers = dbContext.Actvtys.Include(w => w.Memberships).ThenInclude(u => u.User).FirstOrDefault(w => w.ActvtyId == ActvtyId);
            
            ViewBag.AllMembers = aMembers.Memberships;
             
            Console.WriteLine("//////////////////////////////////////////////////");
            return View ("Actvty"); 
            
           
        }

        [HttpGet("delete/{ActvtyId}")]
        public IActionResult DeleteActvty(int ActvtyId)
        {
            Actvty deleteActvty = dbContext.Actvtys.SingleOrDefault(w => w.ActvtyId == ActvtyId);
            dbContext.Actvtys.Remove(deleteActvty);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

    

        [HttpGet("attend/{ActvtyId}")]
        public IActionResult Attend(int ActvtyId)
        {
            Actvty myActvty = dbContext.Actvtys.FirstOrDefault(a => a.ActvtyId == ActvtyId);
            User uActvtys = dbContext.Users
                .Include(b => b.Memberships)
                .ThenInclude( m=> m.Actvty)
                .ToList().FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            
            bool willAttend = true;
            foreach (var u in uActvtys.Memberships)
            {
                if (u.Actvty.Date == myActvty.Date)
                {
                    willAttend = false;
                    Console.WriteLine("Can't attend "+myActvty.Title);
                }
            }

            if (willAttend)
            {
                Membership membership = new Membership();
                membership.UserId = (int)HttpContext.Session.GetInt32("UserId");
                membership.ActvtyId = ActvtyId;
                dbContext.Memberships.Add(membership);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }


        [HttpGet("unrsvp/{memId}")]
        public IActionResult UnRSVP(int memId)
        {
            Membership maker = dbContext.Memberships.FirstOrDefault(r => r.MembershipId == memId); 
            // && r.UserId == HttpContext.Session.GetInt32("UserId"));
            dbContext.Memberships.Remove(maker);  
            dbContext.SaveChanges();
            return RedirectToAction("dashboard", memId);
            
        }


        // [HttpGet("unrsvp/{memId}")]
        // public IActionResult Unrsvp(int memId)
        // {
        //     Membership membership = dbContext.Memberships.FirstOrDefault(a => a.MembershipId == memId);
        //     dbContext.Memberships.Remove(membership);
        //     dbContext.SaveChanges();
        //     return RedirectToAction("Dashboard");
        // }



















    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;
using SqlItems;
using PatternDemo.Models;

namespace PatternDemo.Controllers
{
    public class ItemsController : Controller
    {
        private readonly Repository.IRepository<Item> _context;

        public ItemsController(Repository.IRepository<Item> context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var all = _context.GetAll();
            var vm = runnerVM.buildVM(all.ToList());
			return View(vm);
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = _context.GetById((int)id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
    }
}

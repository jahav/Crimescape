using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Crimescape.Models;
using System.IO;
using Crimescape.Entities;
using AutoMapper;

namespace Crimescape.Controllers
{
    public class DataController : Controller
    {
        private CrimeContext db = new CrimeContext();

        //
        // GET: /Data/

        public ActionResult Index()
        {
            return View(db.DataSources.ToList());
        }

        //
        // GET: /Data/Details/5

        public ActionResult Details(int id = 0)
        {
            ExcelDataSource exceldatasource = db.DataSources.Find(id);
            if (exceldatasource == null)
            {
                return HttpNotFound();
            }
            return View(exceldatasource);
        }

        //
        // GET: /Data/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Data/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExcelDataViewModel exceldatasource)
        {
            if (ModelState.IsValid)
            {
				// TODO: Solve multiple files with same names
				var newName = exceldatasource.File.FileName;
				exceldatasource.File.SaveAs(Path.Combine(Server.MapPath("~/Uploads"), newName));
				var newSource = new ExcelDataSource
				{
					OriginalFilename = exceldatasource.File.FileName,
					Description = exceldatasource.Description,
					Filename = newName
				};
				db.DataSources.Add(newSource);
				db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(exceldatasource);
        }

        //
        // GET: /Data/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ExcelDataSource exceldatasource = db.DataSources.Find(id);
            if (exceldatasource == null)
            {
                return HttpNotFound();
            }
            return View(exceldatasource);
        }

        //
        // POST: /Data/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExcelDataSource exceldatasource)
        {
            if (ModelState.IsValid)
            {
				var entity = db.DataSources.Find(exceldatasource.Id);
				entity.Description = exceldatasource.Description;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exceldatasource);
        }

        //
        // GET: /Data/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ExcelDataSource exceldatasource = db.DataSources.Find(id);
            if (exceldatasource == null)
            {
                return HttpNotFound();
            }
            return View(exceldatasource);
        }

        //
        // POST: /Data/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExcelDataSource exceldatasource = db.DataSources.Find(id);
            db.DataSources.Remove(exceldatasource);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		public ActionResult Import(int id = 0)
		{
			var dataSource = db.DataSources.Find(id);
			if (dataSource == null)
			{
				return HttpNotFound();
			}
			return View(Mapper.Map<ExcelDataViewModel>(dataSource));
		}

		[HttpPost, ActionName("Import")]
		[ValidateAntiForgeryToken]
		public ActionResult ImportConfirmed(int id = 0)
		{
			return Content("TODO:");
		}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
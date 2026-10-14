using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Core.Enums;
using Models;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class InventoryDetailsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: InventoryDetails
        public ActionResult Index(long inventoryId)
        {
            var inventoryDetails = db.InventoryDetails.Include(i => i.Inventory).Where(i=>
                i.InventoryId==inventoryId&&
                i.IsDeleted==false).OrderByDescending(i=>i.CreationDate);
            return View(inventoryDetails.ToList());
        }
         
        public ActionResult Create(long inventoryId)
        {
            ViewBag.InventoryId = inventoryId;
            return View();
        }

        // POST: InventoryDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryDetail inventoryDetail,long inventoryId)
        {
            if (ModelState.IsValid)
            {
				inventoryDetail.IsDeleted=false;
				inventoryDetail.CreationDate= DateTime.Now;
                inventoryDetail.InventoryId = inventoryId;
                inventoryDetail.InventoryDetailType = InventoryDetailType.Buy;
                inventoryDetail.IsActive = true;

                db.InventoryDetails.Add(inventoryDetail);


                var inventory = db.Inventories.FirstOrDefault(c => c.Id == inventoryId);
                if (inventory != null)
                {
                    inventory.Stock += inventoryDetail.Stock;
                }
                db.SaveChanges();
                return RedirectToAction("Index",new {inventoryId=inventoryId});
            }

            //ViewBag.InventoryId = new SelectList(db.Inventories, "Id", "Description", inventoryDetail.InventoryId);
            return View(inventoryDetail);
        }

   //     // GET: InventoryDetails/Edit/5
   //     public ActionResult Edit(long? id)
   //     {
   //         if (id == null)
   //         {
   //             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
   //         }
   //         InventoryDetail inventoryDetail = db.InventoryDetails.Find(id);
   //         if (inventoryDetail == null)
   //         {
   //             return HttpNotFound();
   //         }
   //         ViewBag.InventoryId = new SelectList(db.Inventories, "Id", "Description", inventoryDetail.InventoryId);
   //         return View(inventoryDetail);
   //     }

   //     // POST: InventoryDetails/Edit/5
   //     // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
   //     // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
   //     [HttpPost]
   //     [ValidateAntiForgeryToken]
   //     public ActionResult Edit([Bind(Include = "Id,InventoryId,RefrenceId,Stock,InventoryDetailType,TransactionDate,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] InventoryDetail inventoryDetail)
   //     {
   //         if (ModelState.IsValid)
   //         {
			//	inventoryDetail.IsDeleted=false;
			//		inventoryDetail.LastModifiedDate=DateTime.Now;
   //             db.Entry(inventoryDetail).State = EntityState.Modified;
   //             db.SaveChanges();
   //             return RedirectToAction("Index");
   //         }
   //         ViewBag.InventoryId = new SelectList(db.Inventories, "Id", "Description", inventoryDetail.InventoryId);
   //         return View(inventoryDetail);
   //     }

   //     // GET: InventoryDetails/Delete/5
   //     public ActionResult Delete(long? id)
   //     {
   //         if (id == null)
   //         {
   //             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
   //         }
   //         InventoryDetail inventoryDetail = db.InventoryDetails.Find(id);
   //         if (inventoryDetail == null)
   //         {
   //             return HttpNotFound();
   //         }
   //         return View(inventoryDetail);
   //     }

   //     // POST: InventoryDetails/Delete/5
   //     [HttpPost, ActionName("Delete")]
   //     [ValidateAntiForgeryToken]
   //     public ActionResult DeleteConfirmed(long id)
   //     {
   //         InventoryDetail inventoryDetail = db.InventoryDetails.Find(id);
			//inventoryDetail.IsDeleted=true;
			//inventoryDetail.DeletionDate=DateTime.Now;
 
   //         db.SaveChanges();
   //         return RedirectToAction("Index");
   //     }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

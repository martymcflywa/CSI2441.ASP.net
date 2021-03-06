﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using A2.University.Web.Models;
using A2.University.Web.Models.Business.Services;
using A2.University.Web.Models.Entities;
using A2.University.Web.Models.StaffPortal;

namespace A2.University.Web.Controllers.StaffPortal
{

    /// <summary>
    /// Controller for Staff
    /// </summary>
    public class StaffController : Controller
    {
        private readonly UniversityEntities _db = new UniversityEntities();
        private int _emailMatchTally;
        private string _email;
        private string _tempEmail;

        /// <summary>
        /// GET: Staff
        /// Displays Staff/Index CRUD grid of all staff members in database.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            StaffIndexViewModel staffViewModel = new StaffIndexViewModel();
            var staffEntity = _db.Staff.OrderByDescending(s => s.staff_id).ToList();

            // transfer entity list to viewmodel list
            foreach (Staff staff in staffEntity)
            {
                staffViewModel.Staff.Add(new StaffIndexViewModel
                {
                    StaffId = staff.staff_id,
                    FirstName = staff.firstname,
                    LastName = staff.surname,
                    Email = staff.email,
                    FullName = staff.firstname + " " + staff.surname
                });
            }

            return View(staffViewModel.Staff);
        }

        /// <summary>
        /// GET: Staff/Details/5
        /// Shows details of Staff when "View" link clicked.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // create entitymodel, match id
            Staff staffEntityModel = _db.Staff.Find(id);
            // create viewmodel, pass values from entitymodel
            StaffDetailsViewModel staffViewModel = new StaffDetailsViewModel();
            PopulateViewModel(staffViewModel, staffEntityModel);

            if (staffEntityModel == null)
            {
                return HttpNotFound();
            }
            
            // render view using viewmodel
            return View(staffViewModel);
        }

        /// <summary>
        /// GET: Staff/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            // render view using viewmodel
            return View(new StaffCreateViewModel());
        }

        /// <summary>
        /// POST: Staff/Create
        /// Stores new Staff in database if passes validation, defined by StaffBaseViewModelValidator.
        /// Shows feedback to user when successfully creates a new staff member.
        /// </summary>
        /// <param name="staffViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StaffId,FirstName,LastName,Email")] StaffCreateViewModel staffViewModel)
        {
            // if input passes validation
            if (ModelState.IsValid)
            {
                // create entitymodel, pass values from viewmodel
                Staff staffEntityModel = new Staff();
                PopulateEntityModel(staffViewModel, staffEntityModel);

                // update db using entitymodel
                _db.Staff.Add(staffEntityModel);
                _db.SaveChanges();

                // provide feedback to user
                TempData["notice"] = $"Staff {staffEntityModel.staff_id} {staffEntityModel.firstname} {staffEntityModel.surname} was successfully created";

                return RedirectToAction("Index");
            }

            return View(staffViewModel);
        }

        /// <summary>
        /// GET: Staff/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // create entitymodel, match id
            Staff staffEntityModel = _db.Staff.Find(id);
            // create viewmodel, pass values from entity model
            StaffEditViewModel staffViewModel = new StaffEditViewModel();
            PopulateViewModel(staffViewModel, staffEntityModel);

            if (staffEntityModel == null)
            {
                return HttpNotFound();
            }

            // render view using viewmodel
            return View(staffViewModel);
        }

        /// <summary>
        /// POST: Staff/Edit/5
        /// Stores edited data if viewmodel passes validation.
        /// Generates new email on edit.
        /// Shows feedback to user when successfully edits data.
        /// </summary>
        /// <param name="staffEntityModel"></param>
        /// <param name="staffViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StaffId,FirstName,LastName,Email")] Staff staffEntityModel, StaffEditViewModel staffViewModel)
        {
            if (ModelState.IsValid)
            {
                // populate the entitymodel
                PopulateEntityModel(staffViewModel, staffEntityModel);

                // update db using entitymodel
                _db.Entry(staffEntityModel).State = EntityState.Modified;
                _db.SaveChanges();

                // provide feedback to user
                TempData["notice"] = $"Staff {staffEntityModel.staff_id} {staffEntityModel.firstname} {staffEntityModel.surname} was successfully edited";

                return RedirectToAction("Index");
            }
            return View(staffViewModel);
        }

        /// <summary>
        /// GET: Student/Delete/5
        /// Displays "Are you sure you want to delete" view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // create entity model, match id
            Staff staffEntityModel = _db.Staff.Find(id);
            // create viewmodel, pass values from entitymodel
            StaffDeleteViewModel staffViewModel = new StaffDeleteViewModel();
            PopulateViewModel(staffViewModel, staffEntityModel);

            // get number of affected rows
            int rows = GetNumberOfAffectedRows((long) id);
            if (rows > 0)
            {
                // tell user how many rows this deletion will affect
                TempData["delete-notice"] =
                    $"WARNING: Deleting this record will also delete {rows} other record/s in the database!";
            }

            if (staffEntityModel == null)
            {
                return HttpNotFound();
            }

            // render view using viewmodel
            return View(staffViewModel);
        }

        /// <summary>
        /// POST: Staff/Delete/5
        /// Deletes row from database.
        /// Shows feedback to user when successfully deletes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Staff staff = _db.Staff.Find(id);

            // do own cascade on delete
            CascadeOnDelete(id);

            _db.Staff.Remove(staff);
            _db.SaveChanges();

            // provide feedback to user
            TempData["notice"] = $"Staff {staff.staff_id} {staff.firstname} {staff.surname} was successfully deleted";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Passes data from the view model to the entity model.
        /// </summary>
        /// <param name="viewModel">StaffBaseViewModel</param>
        /// <param name="entityModel">Staff</param>
        private void PopulateEntityModel(StaffBaseViewModel viewModel, Staff entityModel)
        {
            entityModel.staff_id = viewModel.StaffId;
            entityModel.firstname = viewModel.FirstName;
            entityModel.surname = viewModel.LastName;
            
            // generate email
            StartEmailRecursiveSearch(viewModel);
            // could pass directly to entity, but may need in view
            viewModel.Email = _email;
            entityModel.email = viewModel.Email;
        }


        /// <summary>
        /// Passes data from the entity model to the view model.
        /// </summary>
        /// <param name="viewModel">StaffBaseViewModel</param>
        /// <param name="entityModel">Staff</param>
        private void PopulateViewModel(StaffBaseViewModel viewModel, Staff entityModel)
        {
            viewModel.StaffId = entityModel.staff_id;
            viewModel.FirstName = entityModel.firstname;
            viewModel.LastName = entityModel.surname;
            viewModel.Email = entityModel.email;
        }

        /// <summary>
        /// Returns number of affected rows.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int GetNumberOfAffectedRows(long id)
        {
            return
                _db.Courses.Count(su => su.coordinator_id == id) +
                _db.Units.Count(ue => ue.coordinator_id == id) +
                _db.CourseEnrolments.Count(ce => ce.Course.coordinator_id == id) +
                _db.UnitEnrolments.Count(ue => ue.Unit.coordinator_id == id);
            
        }

        /// <summary>
        /// Implemented own cascade on delete,
        /// database not performing it on its own.
        /// </summary>
        /// <param name="id"></param>
        private void CascadeOnDelete(long id)
        {
            var courses = _db.Courses
                .Where(c => c.coordinator_id == id);
            var units = _db.Units
                .Where(u => u.coordinator_id == id);
            var courseEnrolments = _db.CourseEnrolments
                .Where(ce => ce.Course.coordinator_id == id);
            var unitEnrolments = _db.UnitEnrolments
                .Where(ue => ue.Unit.coordinator_id == id);

            foreach (Course x in courses)
            {
                _db.Courses.Remove(x);
            }
            foreach (Unit x in units)
            {
                _db.Units.Remove(x);
            }
            foreach (CourseEnrolment x in courseEnrolments)
            {
                _db.CourseEnrolments.Remove(x);
            }
            foreach (UnitEnrolment x in unitEnrolments)
            {
                _db.UnitEnrolments.Remove(x);
            }
        }

        /// <summary>
        /// Function initiates recursive search of generated emails to ensure no duplicates exist.
        /// Begins by reseting tallies and current Email fields, generates first version of Email, then passes to EmailRecursiveSearch.
        /// TODO: Try to implement this in GenerateEmail class, code duplication between Student/UnitControllers
        /// </summary>
        /// <param name="staff">Staff</param>
        private void StartEmailRecursiveSearch([Bind(Include = "StaffId,FirstName,LastName,Email")] StaffBaseViewModel staff)
        {
            // reset fields for each new student instance
            _emailMatchTally = 0;
            _email = "";

            // generate initial standard Email
            string target = staff.FirstName[0] + "." + staff.LastName + EmailGenerator.StaffEmailSuffix;
            // use email as target for search
            EmailRecursiveSearch(staff, target.ToLower());
        }
        
        /// <summary>
        /// Recursive function to search each version of generated Email against existing emails to ensure no duplicates exist.
        /// </summary>
        /// <param name="staff">Staff</param>
        /// <param name="target">string</param>
        private void EmailRecursiveSearch([Bind(Include = "StaffId,FirstName,LastName,Email")] StaffBaseViewModel staff,
            string target)
        {
            if (SearchEmail(target))
            {
                // increment tally
                _emailMatchTally++;
                // make new temp Email based on tally
                _tempEmail = EmailGenerator.GenerateEmail(
                    "staff",
                    _emailMatchTally,
                    staff.FirstName.ToLower(),
                    staff.LastName.ToLower());

                // recursive call to search new version of generated Email
                EmailRecursiveSearch(staff, _tempEmail);
            }
            else
            {
                _email = target;
            }
        }

        /// <summary>
        /// Function searches generated Email against existing emails in database.
        /// </summary>
        /// <param name="target">string</param>
        /// <returns>bool</returns>
        private bool SearchEmail(string target)
        {
            return _db.Staff.Any(e => e.email == target);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

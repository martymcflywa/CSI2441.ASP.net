﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using A2.University.Web.Models;
using A2.University.Web.Models.Business;
using A2.University.Web.Models.Entities;
using A2.University.Web.Models.StaffPortal;

namespace A2.University.Web.Controllers.StaffPortal
{

    /// <summary>
    /// Controller for UnitEnrolment
    /// </summary>
    public class UnitEnrolmentController : Controller
    {
        private readonly UniversityEntities _db = new UniversityEntities();

        /// <summary>
        /// GET: UnitEnrolments
        /// Displays UnitEnrolment/Index CRUD grid of all CourseEnrolments in database.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            UnitEnrolmentIndexViewModel unitEnrolmentViewModel = new UnitEnrolmentIndexViewModel();

            // get list of all unit enrolments from db
            var unitEnrolmentsEntity = _db.UnitEnrolments
                // order by unit enrolment id descending
                .OrderByDescending(u => u.unit_enrolment_id)
                // then by student id descending
                .ThenByDescending(u => u.student_id)
                // then by year/sem descending
                .ThenByDescending(u => u.year_sem)
                // then by unit title
                .ThenBy(u => u.Unit.title)
                // joins
                .Include(u => u.Student)
                .Include(u => u.Unit)
                .Include(u => u.CourseEnrolment)
                .ToList();

            // transfer entity list to viewmodel list
            foreach (UnitEnrolment unitEnrolment in unitEnrolmentsEntity)
            {
                unitEnrolmentViewModel.UnitEnrolments.Add(new UnitEnrolmentIndexViewModel
                {
                    UnitEnrolmentId = unitEnrolment.unit_enrolment_id,
                    StudentId = unitEnrolment.student_id,
                    StudentFirstName = unitEnrolment.Student.firstname,
                    StudentLastName = unitEnrolment.Student.lastname,
                    UnitId = unitEnrolment.unit_id,
                    Title = unitEnrolment.Unit.title,
                    YearSem = unitEnrolment.year_sem.ToString(),
                    Mark = unitEnrolment.mark.ToString(),
                    Grade = GradeRules.GetGrade(unitEnrolment.mark),
                    CourseId = unitEnrolment.CourseEnrolment.course_id
                });
            }

            // render view using viewmodel list
            return View(unitEnrolmentViewModel.UnitEnrolments);
        }

        /// <summary>
        /// GET: UnitEnrolments/Details/5
        /// Shows details of UnitEnrolment when "View" link clicked.
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
            UnitEnrolment unitEnrolmentEntityModel = _db.UnitEnrolments.Find(id);
            // create viewmodel, pass values from entitymodel
            UnitEnrolmentDetailsViewModel unitEnrolmentViewModel = new UnitEnrolmentDetailsViewModel
            {
                // derived field
                CourseIdTitle = 
                    $"{unitEnrolmentEntityModel.CourseEnrolment.course_id} " +
                    $"{unitEnrolmentEntityModel.CourseEnrolment.Course.title}"

            };

            PopulateViewModel(unitEnrolmentViewModel, unitEnrolmentEntityModel);

            if (unitEnrolmentEntityModel == null)
            {
                return HttpNotFound();
            }
            return View(unitEnrolmentViewModel);
        }

        /// <summary>
        /// GET: UnitEnrolments/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            // create viewmodel
            UnitEnrolmentCreateViewModel unitEnrolmentViewModel = new UnitEnrolmentCreateViewModel();
            // populate dropdownlists
            PopulateDropDownLists(unitEnrolmentViewModel);

            return View(unitEnrolmentViewModel);
        }

        /// <summary>
        /// POST: UnitEnrolments/Create
        /// Stores new UnitEnrolment in database if passes validation, defined by UnitEnrolmentBaseViewModelValidator.
        /// Shows feedback to user when successfully creates new UnitEnrolment.
        /// </summary>
        /// <param name="unitEnrolmentViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitEnrolmentId,StudentId,UnitId,YearSem,Mark")] UnitEnrolmentCreateViewModel unitEnrolmentViewModel)
        {
            // if input passes validation
            if (ModelState.IsValid)
            {
                // create entitymodel, pass values from viewmodel
                UnitEnrolment unitEnrolmentEntityModel = new UnitEnrolment();
                PopulateEntityModel(unitEnrolmentViewModel, unitEnrolmentEntityModel);

                // update db using entitymodel
                _db.UnitEnrolments.Add(unitEnrolmentEntityModel);
                _db.SaveChanges();

                // populate course status after db update
                PopulateCourseStatus(unitEnrolmentEntityModel);

                // get unit
                var unit = _db.Units
                    .FirstOrDefault(u =>
                        u.unit_id == unitEnrolmentEntityModel.unit_id);
                // get student
                var student = _db.Students
                    .FirstOrDefault(s =>
                        s.student_id == unitEnrolmentEntityModel.student_id);

                // provide feedback to user
                TempData["notice"] = $"Unit Enrolment {unitEnrolmentEntityModel.unit_id} {unit?.title} for {student?.firstname} {student?.lastname} was successfully created";

                return RedirectToAction("Index");
            }

            // populate dropdownlists
            PopulateDropDownLists(unitEnrolmentViewModel);

            return View(unitEnrolmentViewModel);
        }

        /// <summary>
        /// GET: UnitEnrolments/Edit/5
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
            UnitEnrolment unitEnrolmentEntityModel = _db.UnitEnrolments.Find(id);
            // create viewmodel, pass values from entitymodel
            UnitEnrolmentEditViewModel unitEnrolmentViewModel = new UnitEnrolmentEditViewModel
            {
                // readonly field in view
                StudentIdFullName = 
                    $"{unitEnrolmentEntityModel.Student.student_id} " +
                    $"{unitEnrolmentEntityModel.Student.firstname} " +
                    $"{unitEnrolmentEntityModel.Student.lastname}"
            };

            PopulateViewModel(unitEnrolmentViewModel, unitEnrolmentEntityModel);

            // populate dropdownlists
            PopulateDropDownLists(unitEnrolmentViewModel);

            if (unitEnrolmentEntityModel == null)
            {
                return HttpNotFound();
            }

            // render view using viewmodel
            return View(unitEnrolmentViewModel);
        }

        /// <summary>
        /// POST: UnitEnrolments/Edit/5
        /// Stores edited data if viewmodel passes validation.
        /// Shows feedback to user when successfully edits data.
        /// </summary>
        /// <param name="unitEnrolmentEntityModel"></param>
        /// <param name="unitEnrolmentViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitEnrolmentId,StudentId,UnitId,YearSem,Mark")] UnitEnrolment unitEnrolmentEntityModel, UnitEnrolmentEditViewModel unitEnrolmentViewModel)
        {
            if (ModelState.IsValid)
            {
                // populate entitymodel
                PopulateEntityModel(unitEnrolmentViewModel, unitEnrolmentEntityModel);

                // update db using entitymodel
                // *** PROBLEM *** course_enrolment_id is set to 0 when unit enrolment is edited MUST FIX AFTER EXAMS!!
                _db.Entry(unitEnrolmentEntityModel).State = EntityState.Modified;
                _db.SaveChanges();

                // populate course status after db update
                PopulateCourseStatus(unitEnrolmentEntityModel);

                // get unit
                var unit = _db.Units
                    .FirstOrDefault(u =>
                        u.unit_id == unitEnrolmentEntityModel.unit_id);
                // get student
                var student = _db.Students
                    .FirstOrDefault(s =>
                        s.student_id == unitEnrolmentEntityModel.student_id);

                // provide feedback to user
                TempData["notice"] = $"Unit Enrolment {unitEnrolmentEntityModel.unit_id} {unit?.title} for {student?.firstname} {student?.lastname} was successfully edited";

                return RedirectToAction("Index");
            }

            // populate dropdownlists
            PopulateDropDownLists(unitEnrolmentViewModel);

            return View(unitEnrolmentViewModel);
        }

        /// <summary>
        /// GET: UnitEnrolments/Delete/5
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

            // create entitymodel, match id
            UnitEnrolment unitEnrolmentEntityModel = _db.UnitEnrolments.Find(id);
            // create viewmodel, pass values from entitymodel
            UnitEnrolmentDeleteViewModel unitEnrolmentViewModel = new UnitEnrolmentDeleteViewModel();
            PopulateViewModel(unitEnrolmentViewModel, unitEnrolmentEntityModel);

            if (unitEnrolmentEntityModel == null)
            {
                return HttpNotFound();
            }
            return View(unitEnrolmentViewModel);
        }

        /// <summary>
        /// POST: UnitEnrolments/Delete/5
        /// Deletes row from database.
        /// Shows feedback to user when successfully deletes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            UnitEnrolment unitEnrolment = _db.UnitEnrolments.Find(id);

            // provide feedback to user
            TempData["notice"] = $"Unit Enrolment {unitEnrolment.unit_id} {unitEnrolment.Unit.title} for {unitEnrolment.Student.firstname} {unitEnrolment.Student.lastname} was successfully deleted";

            _db.UnitEnrolments.Remove(unitEnrolment);
            _db.SaveChanges();

            // populate course status after db update
            PopulateCourseStatus(unitEnrolment);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Checks if course should be completed/excluded.
        /// Defaults to enrolled if neither.
        /// </summary>
        /// <param name="unitEnrolmentEntityModel"></param>
        private void PopulateCourseStatus(UnitEnrolment unitEnrolmentEntityModel)
        {
            ProgressRules progressRules = new ProgressRules(unitEnrolmentEntityModel.course_enrolment_id);
            CourseRules courseRules = new CourseRules();

            var courseEnrolment = _db.CourseEnrolments
                .FirstOrDefault(ce =>
                    ce.course_enrolment_id == unitEnrolmentEntityModel.course_enrolment_id);

            if (courseEnrolment != null)
            {
                // if completed
                if (progressRules.IsCourseComplete())
                {
                    courseEnrolment.course_status = courseRules.CourseStates["Completed"];
                }
                // if excluded
                else if (progressRules.IsCourseExcluded())
                {
                    courseEnrolment.course_status = courseRules.CourseStates["Excluded"];
                }
                // else roll back to ENROLLED for edit/deletes resulting in course no longer completed/excluded
                else
                {
                    courseEnrolment.course_status = courseRules.CourseStates["Enrolled"];
                }
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Populates dropdownlists for unit enrolment view.
        /// </summary>
        /// <param name="viewModel"></param>
        private void PopulateDropDownLists(UnitEnrolmentDropDownListViewModel viewModel)
        {
            string state = new CourseRules().CourseStates["Enrolled"];

            // get list of students ENROLLED in a course
            var courseEnrolmentsEntity = _db.CourseEnrolments
                .Where(ce =>
                    ce.course_status == state)
                .Include(s =>
                    s.Student)
                .ToList();

            // get list of units
            var unitsEntity = _db.Units.ToList();

            // transfer relevant elements to viewmodel list
            foreach (CourseEnrolment enrolment in courseEnrolmentsEntity)
            {
                viewModel.Students.Add(new UnitEnrolmentDropDownListViewModel
                {
                    StudentId = enrolment.student_id,
                    StudentIdFullName = 
                        $"{enrolment.student_id} " +
                        $"{enrolment.Student.firstname} " +
                        $"{enrolment.Student.lastname}"
                });
            }

            foreach (Unit unit in unitsEntity)
            {
                viewModel.Units.Add(new UnitEnrolmentDropDownListViewModel
                {
                    UnitId = unit.unit_id,
                    UnitIdTitle = $"{unit.unit_id} {unit.title}"
                });
            }

            // populate dropdownlist from viewmodel list
            viewModel.StudentDropDownList = new SelectList(viewModel.Students.OrderBy(s => s.StudentId), "StudentId", "StudentIdFullName");
            viewModel.UnitDropDownList = new SelectList(viewModel.Units.OrderBy(u => u.UnitId), "UnitId", "UnitIdTitle");
        }

        /// <summary>
        /// Passes data from the view model to the entity model.
        /// </summary>
        /// <param name="viewModel">UnitBaseViewModel</param>
        /// <param name="entityModel">Unit</param>
        private void PopulateEntityModel(UnitEnrolmentBaseViewModel viewModel, UnitEnrolment entityModel)
        {
            entityModel.unit_enrolment_id = viewModel.UnitEnrolmentId;
            entityModel.student_id = viewModel.StudentId;
            entityModel.unit_id = viewModel.UnitId;
            entityModel.year_sem = int.Parse(viewModel.YearSem);
            entityModel.mark = int.Parse(viewModel.Mark);

            // can't use dict in linq, substitute with string
            CourseRules courseRules = new CourseRules();
            string enrolled = courseRules.CourseStates["Enrolled"];
            // check for excluded too, user may edit mark for unit in excluded course, resulting in enrolled unit
            string excluded = courseRules.CourseStates["Excluded"];

            // IMPORTANT: Only want a single db record here, using SingleOrDefault instead of First/Where
            // WARNING: This MUST RETURN A SINGLE VALUE, else will cause error, readonly course status ensures this, DO NOT allow user to modify status!
            // ASSUMPTION: Excluded student is not able to re-enroll into another course without approval, see CourseEnrolmentController.Create() POST method

            // select course_enrolment_id where StudentId is match, and is (ENROLLED or EXCLUDED)
            var enrolment = _db.CourseEnrolments
                .SingleOrDefault(ce =>
                    ce.student_id == viewModel.StudentId &&
                    (ce.course_status == enrolled ||
                     ce.course_status == excluded));

            if (enrolment != null)
            {
                // pass value to viewModel, might need it
                viewModel.CourseEnrolmentId = enrolment.course_enrolment_id;
                // pass value to entitymodel
                entityModel.course_enrolment_id = viewModel.CourseEnrolmentId;
            }
        }

        /// <summary>
        /// Passes data from the entity model to the view model.
        /// </summary>
        /// <param name="viewModel">UnitBaseViewModel</param>
        /// <param name="entityModel">Unit</param>
        private void PopulateViewModel(UnitEnrolmentBaseViewModel viewModel, UnitEnrolment entityModel)
        {
            viewModel.UnitEnrolmentId = entityModel.unit_enrolment_id;

            viewModel.StudentId = entityModel.student_id;
            viewModel.UnitId = entityModel.unit_id;
            viewModel.YearSem = entityModel.year_sem.ToString();
            viewModel.Mark = entityModel.mark.ToString();

            viewModel.StudentFullName =
                $"{entityModel.Student.firstname} " +
                $"{ entityModel.Student.lastname}";

            viewModel.Title = entityModel.Unit.title;
            viewModel.Grade = GradeRules.GetGrade(entityModel.mark);
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

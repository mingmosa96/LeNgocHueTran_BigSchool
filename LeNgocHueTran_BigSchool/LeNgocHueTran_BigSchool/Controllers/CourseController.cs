using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeNgocHueTran_BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LeNgocHueTran_BigSchool.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.listCategory = context.Category.ToList();

            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext context = new BigSchoolContext();

            ModelState.Remove("LecturerId");
            if(!ModelState.IsValid)
            {
                objCourse.listCategory = context.Category.ToList();
                return View("Create", objCourse);
            }
            
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            context.Course.Add(objCourse);
            context.SaveChanges();

            return RedirectToAction("Index","Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendancces = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendancces)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                    .FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Course.Where(p => p.LecturerId == currentUser.Id && p.DateTime > DateTime.Now).ToList();
            foreach(Course i in courses)
            {
                i.LecturerId = currentUser.Name;
            }

            return View(courses);
        }

        public ActionResult Delete(int Id)
        {
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Course.Find(Id);
            context.Course.Remove(courses);
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
        public void setViewBag(int? selectedId = null)
        {
            var model = new Course();
            ViewBag.CategoryId = new SelectList(model.ListAll(), "Id", "Name", selectedId);
        }
        public ActionResult EditMine(int Id)
        {
            BigSchoolContext context = new BigSchoolContext();
            var model = context.Course.Find(Id);
            setViewBag(model.CategoryId);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditMine(Course model)
        {
            BigSchoolContext context = new BigSchoolContext();
            setViewBag(model.CategoryId);
            var updateCourses = context.Course.Find(model.Id);
            updateCourses.DateTime = model.DateTime;
            updateCourses.LectureName = model.LectureName;
            updateCourses.CategoryId = model.CategoryId;
            var id = context.SaveChanges();
            if (id > 0)
                return RedirectToAction("ListMine");
            else
            {
                ModelState.AddModelError("", "Can't save to database");
                return View(model);
            }
        }

        public ActionResult LectureImGoing()
        {
            ApplicationUser currentUser =
           System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
           .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();

            var listFollwee = context.Following.Where(p => p.FollowerId == currentUser.Id).ToList();

            var listAttendances = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}